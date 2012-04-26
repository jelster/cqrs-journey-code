using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Common;
using Registration;
using Registration.Commands;
using Registration.Events;
using Registration.Handlers;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using Xunit;
using Moq;

using Payments.Contracts.Events;

namespace Conference.Specflow.Steps.Registration
{
    public static class TraceExtensions
    {
        public static T TraceCommand<T>(this T command, Func<T, string> projection) where T : ICommand
        {
            Debug.WriteLine(projection(command));
            return command;
        }
    }

    [Binding]
    public class RegistrationSteps
    {
        private readonly Guid _orderId = Guid.NewGuid();

        private AssignRegistrantDetails givenRegistrant;
        private RegisterToConference givenConference;

        private Order order { get; set; }
        private SeatsAvailability seats = new SeatsAvailability(Guid.NewGuid());
        private SeatsAvailabilityHandler seatsHandler;
        private ConferenceInfo conference;

        private RegistrationProcess RegistrationProcess { get; set; }
        private OrderCommandHandler orderCommandHandler;

        private IEventSourcedRepository<Order> orderRepos;

        private readonly MemoryCommandBus memoryCommandBus = new MemoryCommandBus();
        private readonly MemoryEventBus memoryEventBus = new MemoryEventBus();
        private MockRepository mre;
        private RegistrationProcessRouter registrationProcessRouter;

        private readonly List<SeatInfo> givenSeats = new List<SeatInfo>();
        private readonly List<SeatQuantity> givenOrderItems = new List<SeatQuantity>();
        private IEventSourcedRepository<SeatsAvailability> seatRepos;
        private IProcessDataContext<RegistrationProcess> processRepos;

        [BeforeScenario]
        public void SetupScenario()
        {
            conference = new ConferenceInfo()
                             {
                                 Id = Guid.NewGuid(),
                                 OwnerEmail = "owner@email.com",
                                 OwnerName = "Conference.SpecFlow"
                             };
           
            CreateAndSetupMocks();

            RegisterLocalInfrastructure();
        }

        [AfterScenario]
        public void VerifyExpectations()
        {
            mre.Verify();
        }
      
        // TODO: this should be replaced with a less brittle, more standardized init pattern that can be shared with other projects
        private void RegisterLocalInfrastructure()
        {
            memoryCommandBus.Register(orderCommandHandler);
            memoryCommandBus.Register(seatsHandler);
            memoryCommandBus.Register(registrationProcessRouter);
            memoryEventBus.Register(registrationProcessRouter);
        }

        private void CreateAndSetupMocks()
        {
            mre = new MockRepository(MockBehavior.Loose) { DefaultValue = DefaultValue.Empty, CallBase = false};
            
            var orderReposMock = mre.Create<IEventSourcedRepository<Order>>();
            orderReposMock.Setup(x => x.Save(It.IsAny<Order>())).Callback<Order>(ord =>
                                                                                     {
                                                                                         var closedOrder = ord;
                                                                                         var toPub = closedOrder.Events.Where(x => x.Version == ord.Version).ToList();
                                                                                         Debug.WriteLine("{0}***Publishing events for Order...{0}{1}{0}***{0}", Environment.NewLine, string.Join(Environment.NewLine, toPub.Select(pubEv => string.Format("SourceId: {0}, Version: {1}, Type: {2}", pubEv.SourceId, pubEv.Version, pubEv.GetType()))));
                                                                                         memoryEventBus.Publish(toPub);
                                                                                         order = closedOrder;
                                                                                     }).Verifiable();

            orderReposMock.Setup(x => x.Find(_orderId)).Returns(() =>
                                                                    {
                                                                        var o = this.order;
                                                                        return o;
                                                                    }).Verifiable();

            orderRepos = orderReposMock.Object;

            orderCommandHandler = new OrderCommandHandler(orderRepos);

            var mockSeatRepos = mre.Create<IEventSourcedRepository<SeatsAvailability>>();
            mockSeatRepos.Setup(x => x.Save(It.IsAny<SeatsAvailability>()))
                .Callback<SeatsAvailability>(x =>
                                                 {
                                                     var toPub = x.Events.Where(s => s.Version == x.Version).ToList();
                                                     this.seats = x;
                                                     Debug.WriteLine("{0}***{0}Publishing events for SeatsAvailability...{0}{1}{0}***{0}", Environment.NewLine, string.Join(Environment.NewLine, toPub.Select(pubEv => string.Format("SourceId: {0}, Version: {1}, Type: {2}", pubEv.SourceId, pubEv.Version, pubEv.GetType()))));
                                                     memoryEventBus.Publish(toPub);
                                                 }).Verifiable();

            mockSeatRepos.Setup(x => x.Find(It.IsAny<Guid>())).Returns(() => seats).Verifiable();

            seatRepos = mockSeatRepos.Object;
            seatsHandler = new SeatsAvailabilityHandler(seatRepos);

            var processReposMock = mre.Create<IProcessDataContext<RegistrationProcess>>();
            processReposMock.Setup(x => x.Find(It.IsAny<Expression<Func<RegistrationProcess, bool>>>()))
                .Returns(() =>
                             {
                                 var r = RegistrationProcess;
                                 return r;
                             })
                .Verifiable();

            processReposMock.Setup(x => x.Find(It.IsAny<Guid>())).Returns(() => RegistrationProcess);

            processReposMock.Setup(x => x.Save(It.IsAny<RegistrationProcess>()))
                .Callback<RegistrationProcess>(proj =>
                                                   {
                                                       var closedProcess = proj;
                                                       var toPub = closedProcess.Commands.ToList();
                                                       Debug.WriteLine("{0}***Sending Commands for RegistrationProcess...{0}{1}{0}***{0}", Environment.NewLine, string.Join(Environment.NewLine, toPub.Select(pubCmd => string.Format("Command: {0}, Delay: {1}", pubCmd.Body.GetType().Name, pubCmd.Delay))));
                                                       memoryCommandBus.Send(toPub);
                                                       RegistrationProcess = closedProcess;
                                                       RegistrationProcess.ClearCommands();
                                                   })
                .Verifiable();
            processRepos = processReposMock.Object;
            
            registrationProcessRouter = new RegistrationProcessRouter(() => processRepos);

        }

        [Given(@"that '(.*)' is the site conference having the following.*")]
        public void GivenAConference(string conferenceName, IEnumerable<SeatInfo> givenSeatTypes)
        {
            conference.Name = conferenceName;
             
            foreach (var s in givenSeatTypes)
            {
                seats.AddSeats(s.Id, s.Quantity);
                givenSeats.Add(s);
            }
        }

        private string FormatDebugText<T>(T command, string source, Func<T, string> dataSelector = null)
        {
            var fmt = "{0}***Sending Commands from TEST...{0}{1}***{0}";
            return string.Format(fmt, Environment.NewLine,
                string.Format("Command: {0}, Source: {1}, Data: {2}", command.GetType().Name, source, dataSelector == null ? "N/A" : dataSelector(command)));
        }

        private string FormatDebugText(Envelope<ICommand> command)
        {
            return FormatDebugText(command.Body, command.Delay.ToString());
        }

        [Given(@"an|the following Order.*")]
        public void GivenTheFollowingOrderItems(IEnumerable<SeatQuantity> items)
        {
            givenConference = new RegisterToConference
            {
                ConferenceId = conference.Id,
                OrderId = _orderId,
                Seats = items.ToList()
            };

            givenOrderItems.AddRange(givenConference.Seats);
            Debug.WriteLine(FormatDebugText(givenConference, "TEST", 
                c => string.Format("Order: {0}, Conference: {1}, Seats: {2}", c.OrderId, c.ConferenceId, c.Seats.Count())));
            memoryCommandBus.SendInvoke(givenConference);
            
        }

        [Given(@"the following registrant")]
        public void GivenARegistrant(Table table)
        {
            givenRegistrant = table.CreateInstance(() => new AssignRegistrantDetails { OrderId = _orderId });
            conference.OwnerName = string.Format("{1}, {0}", givenRegistrant.FirstName, givenRegistrant.LastName);
            conference.OwnerEmail = givenRegistrant.Email;

            Debug.WriteLine(FormatDebugText(givenRegistrant, "TEST", x => string.Join(" ", x.Id, x.OrderId, x.FirstName, x.LastName, x.Email)));
            memoryCommandBus.SendInvoke(givenRegistrant);
        }

        [Given(@"the registrant has entered details for payment of the order")]
        public void GivenTheRegistrantHasEnteredDetailsForPaymentOfAnOrder()
        {
            // TODO: there isn't currently an appropriate command or event that captures this. 

            var cmd = new SetOrderPaymentDetails
            {
                OrderId = givenConference.OrderId,
                PaymentInformation = "test payment info"
            };

            Debug.WriteLine(FormatDebugText(cmd, "TEST", x => string.Join(", ", x.PaymentInformation, x.OrderId, x.Id)));
            memoryCommandBus.SendInvoke(cmd);
        }

        [Given(@"a succesfully confirmed order with an Order Access Code of (\d+)")]
        public void GivenASuccesfullyConfirmedOrderWithAnOrderAccessCode(int accessCode)
        {
            ScenarioContext.Current.Pending();
        }

        [Given(@"the registrant makes a reservation for an order")]
        [When(@"the registrant places a reservation")]
        public void WhenTheRegistrantMakesAReservationForAnOrder()
        {
            var cmd = new MarkSeatsAsReserved
                          {
                              OrderId = order.Id,
                              Id = Guid.NewGuid(),
                              Seats = givenOrderItems.ToList(),
                              Expiration = DateTime.UtcNow.AddMinutes(15)
                          };
            //Debug.WriteLine(FormatDebugText(cmd, "TEST", x => string.Format("OrderId: {0}, Id: {1}, Seats: {2}, Expiration: {3}", x.OrderId, x.Id, x.Seats.Count, x.Expiration)));
            //memoryCommandBus.SendInvoke(cmd);
        }

        [When(@"the Registrant confirms (?:an|the) order")]
        public void WhenTheRegistrantConfirmsAnOrder()
        {
            var cmd = new PaymentCompleted { SourceId = conference.Id, PaymentSourceId = _orderId};

            Debug.WriteLine(FormatDebugText(cmd, "TEST", x => string.Format("PaymentSourceId: {0}, SourceId: {1}", x.PaymentSourceId, x.SourceId)));
            memoryEventBus.PublishInvoke(cmd);
        }

        [When(@"the Registrant begins the payment process for (?:an|the) order")]
        public void WhenTheRegistrantBeginsThePaymentProcessForAnOrder()
        {
            
        }

        [When(@"the Registrant enters a payment for processing")]
        public void WhenTheRegistrantEntersAPaymentForProcessing()
        {
            WhenTheRegistrantConfirmsAnOrder();
        }

        [When(@"the Registrant assigns purchased seats to attendees as below")]
        public void WhenTheRegistrantAssignsPurchasedSeatsToAttendeesAsBelow(Table table)
        {
            ScenarioContext.Current.Pending();
        }

        [StepDefinition(@"the order reservation countdown has not expired")]
        public void TheOrderReservationCountdownHasNotExpired()
        {
            Assert.False(order.Events.OfType<OrderExpired>().Any());
        }

        [Then(@"all order items should be confirmed")]
        public void ThenAllOrderItemsShouldBeConfirmed()
        {
            Assert.True(order.Events.OfType<OrderReservationCompleted>().Any());
        }

        [Then(@"the countdown should be active")]
        public void ThenTheCountdownShouldBeActive()
        {
            TheOrderReservationCountdownHasNotExpired();
        }

        [Then(@"the registrant should be able to enter a payment")]
        public void TheRegistrantShouldBeAbleToEnterAPayment()
        {
            var process = processRepos.Find(x => x.OrderId == _orderId);
            Assert.True(process.State == RegistrationProcess.ProcessState.AwaitingPayment);
        }

        [Then(@"a receipt indicating successful processing of payment should be created")]
        public void ThenAReceiptIndicatingSuccessfulProcessingOfPaymentShouldBeCreated()
        {
            var process = processRepos.Find(x => x.OrderId == _orderId);
            Assert.True(process.State == RegistrationProcess.ProcessState.Completed);
        }

        [Then(@"all items on the order should be confirmed")]
        public void ThenAllItemsOnTheOrderShouldBeConfirmed()
        {
          //  Assert.True(order.Events.OfType<OrderReservationCompleted>().Any());
        }

        [Then(@"a Registration confirmation with the Access code should be displayed")]
        public void ThenARegistrationConfirmationWithTheAccessCodeShouldBeDisplayed()
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"the order total should be \$(\d+)")]
        public void ThenTheOrderTotalShouldBe(decimal totalAmount)
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"the registrant should receive an email containing the receipt and access code")]
        public void ThenTheRegistrantShouldReceiveAnEmailContainingTheReceiptAndAccessCode()
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"the Registrant should receive a message confirming the assignments")]
        public void ThenTheRegistrantShouldReceiveMessageConfirmingTheAssignments()
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"the assigned Attendees should receive an email containing the following information")]
        public void ThenTheAssignedAttendeesShouldReceiveAnEmailContainingTheFollowingInformation(Table table)
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"the order should show an adjustment of \$([\-?]\d+)")]
        public void ThenTheOrderShouldShowAnAdjustmentOf(decimal adjustment)
        {
            ScenarioContext.Current.Pending();
        }

        [StepArgumentTransformation(@"following order+")]
        public IEnumerable<SeatQuantity> SeatQuantityStepTransformer(Table table)
        {
            return table.Rows.Select(x => new SeatQuantity(givenSeats.First(s => s.Name.ToLower() == x["SeatType"].ToLower()).Id, Convert.ToInt32(x[1])));
        }

        [StepArgumentTransformation(@"following [seat|ing]+ types, prices, and availability")]
        public IEnumerable<SeatInfo> SeatTypeTransformer(Table table)
        {
            return table.CreateSet<SeatInfo>();
        }

        [StepArgumentTransformation(@"\$([0-9]+)")]
        public decimal MoneyStepTransformer(string input)
        {
            return decimal.Parse(input);
        }

        private void SetupRegistrationState()
        {
            Assert.NotNull(givenConference);
            Assert.NotNull(givenRegistrant);
        }

    }
}
