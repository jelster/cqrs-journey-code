//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Linq.Expressions;
 
//using Infrastructure.EventSourcing;
//using Infrastructure.Messaging;
//using Infrastructure.Messaging.InMemory;
//using Infrastructure.Processes;
//using Payments;
//using Payments.Contracts.Commands;
//using Registration;
//using Registration.Commands;
//using Registration.Events;
//using Registration.Handlers;
//using Registration.ReadModel;
//using TechTalk.SpecFlow;
//using TechTalk.SpecFlow.Assist;
//using Xunit;
//using Moq;

//using Payments.Contracts.Events;

//namespace Conference.Specflow.Steps.Registration
//{
//    [Binding]
//    public class RegistrationSteps
//    {
//        private ConferenceDTO givenConference;

//        private readonly List<ConferenceSeatTypeDTO> givenSeats = new List<ConferenceSeatTypeDTO>();

//        private OrderDTO givenOrder;

//        private readonly List<OrderItemDTO> givenOrderItems = new List<OrderItemDTO>();

//        private readonly Guid _orderId = Guid.NewGuid();

//        private AssignRegistrantDetails assignRegistrantCommand;

//        private RegisterToConference registrationCommand;

//        private Order order;

//        private SeatsAvailability seats = new SeatsAvailability(Guid.NewGuid());

//        private SeatsAvailabilityHandler seatsHandler;

//        private RegistrationProcess registrationProcess;

//        private OrderCommandHandler orderCommandHandler;

//        private IEventSourcedRepository<Order> orderRepos;

//        private MockRepository mre;

//        private IEventSourcedRepository<SeatsAvailability> seatRepos;

//        private IProcessDataContext<RegistrationProcess> processRepos;

//        [BeforeScenario]
//        public void SetupScenario()
//        {
//            CreateAndSetupMocks();

//            RegisterLocalInfrastructure();
//        }

//        [AfterScenario]
//        public void VerifyExpectations()
//        {
//            mre.Verify();
//        }

//        // TODO: this should be replaced with a less brittle, more standardized init pattern that can be shared with other projects

//        private void RegisterLocalInfrastructure()
//        {
//            Func<Type, string, dynamic> typeProjector = ((t, filt) => new
//                                                                           {
//                                                                               t.Name, 
//                                                                               Handles = t.GetInterfaces()
//                                                                                            .Where(ifx => ifx.Name.Contains(filt))
//                                                                                            .SelectMany(i => i.GetGenericArguments(), (a, x) => x.Name)
//                                                                           });

//            var regCHand = typeof (Order).Assembly.GetTypes().Where(x => x.GetInterfaces()
//                                                                             .Any(z => z == typeof (ICommand))).Select(s => typeProjector(s, "ICommand"));

//            var regEHand = typeof (Order).Assembly.GetTypes().Where(x => x.GetInterfaces()
//                                                                             .Any(z => z == typeof (IEvent))).Select(s => typeProjector(s, "IEvent"));

//            var payCHand = typeof(InitiateInvoicePayment).Assembly.GetTypes()
//                .Where(x => x.GetInterfaces().Select(y => y.Name)
//                .Any(z => z == "ICommandHandler"))
//                .Select(s => typeProjector(s, "ICommand"));

//            var payEHand = typeof(PaymentAccepted).Assembly.GetTypes()
//                .Where(x => x.GetInterfaces().Select(y => y.Name)
//                .Any(z => z == "IEventHandler"))
//                .Select(s => typeProjector(s, "IEvent"));

            
//            //memoryCommandBus.Register(orderCommandHandler);
//            //memoryCommandBus.Register(seatsHandler);
//            //memoryCommandBus.Register(registrationProcessRouter);
//            //memoryEventBus.Register(registrationProcessRouter);
//        }

//        private void CreateAndSetupMocks()
//        {
//            //mre = new MockRepository(MockBehavior.Loose) { DefaultValue = DefaultValue.Empty, CallBase = false};
            
//            //var orderReposMock = mre.Create<IEventSourcedRepository<Order>>();
//            //orderReposMock.Setup(x => x.Save(It.IsAny<Order>())).Callback<Order>(ord =>
//            //                                                                         {
//            //                                                                             var closedOrder = ord;
//            //                                                                             var toPub = closedOrder.Events.Where(x => x.Version == ord.Version).ToList();
//            //                                                                             Debug.WriteLine("{0}***Publishing events for Order...{0}{1}{0}***{0}", Environment.NewLine, string.Join(Environment.NewLine, toPub.Select(pubEv => string.Format("SourceId: {0}, Version: {1}, Type: {2}", pubEv.SourceId, pubEv.Version, pubEv.GetType()))));
//            //                                                                             memoryEventBus.Publish(toPub);
//            //                                                                             order = closedOrder;
//            //                                                                         }).Verifiable();

//            //orderReposMock.Setup(x => x.Find(_orderId)).Returns(() =>
//            //                                                        {
//            //                                                            var o = this.order;
//            //                                                            return o;
//            //                                                        }).Verifiable();

//            //orderRepos = orderReposMock.Object;

//            //orderCommandHandler = new OrderCommandHandler(orderRepos);

//            //var mockSeatRepos = mre.Create<IEventSourcedRepository<SeatsAvailability>>();
//            //mockSeatRepos.Setup(x => x.Save(It.IsAny<SeatsAvailability>()))
//            //    .Callback<SeatsAvailability>(x =>
//            //                                     {
//            //                                         var toPub = x.Events.Where(s => s.Version == x.Version).ToList();
//            //                                         this.seats = x;
//            //                                         Debug.WriteLine("{0}***{0}Publishing events for SeatsAvailability...{0}{1}{0}***{0}", Environment.NewLine, string.Join(Environment.NewLine, toPub.Select(pubEv => string.Format("SourceId: {0}, Version: {1}, Type: {2}", pubEv.SourceId, pubEv.Version, pubEv.GetType()))));
//            //                                         memoryEventBus.Publish(toPub);
//            //                                     }).Verifiable();

//            //mockSeatRepos.Setup(x => x.Find(It.IsAny<Guid>())).Returns(() => seats).Verifiable();

//            //seatRepos = mockSeatRepos.Object;
//            //seatsHandler = new SeatsAvailabilityHandler(seatRepos);

//            //var processReposMock = mre.Create<IProcessDataContext<RegistrationProcess>>();
//            //processReposMock.Setup(x => x.Find(It.IsAny<Expression<Func<RegistrationProcess, bool>>>()))
//            //    .Returns(() =>
//            //                 {
//            //                     var r = RegistrationProcess;
//            //                     return r;
//            //                 })
//            //    .Verifiable();

//            //processReposMock.Setup(x => x.Find(It.IsAny<Guid>())).Returns(() => RegistrationProcess);

//            //processReposMock.Setup(x => x.Save(It.IsAny<RegistrationProcess>()))
//            //    .Callback<RegistrationProcess>(proj =>
//            //                                       {
//            //                                           var closedProcess = proj;
//            //                                           var toPub = closedProcess.Commands.ToList();
//            //                                           Debug.WriteLine("{0}***Sending Commands for RegistrationProcess...{0}{1}{0}***{0}", Environment.NewLine, string.Join(Environment.NewLine, toPub.Select(pubCmd => string.Format("Command: {0}, Delay: {1}", pubCmd.Body.GetType().Name, pubCmd.Delay))));
//            //                                           memoryCommandBus.Send(toPub);
//            //                                           RegistrationProcess = closedProcess;
//            //                                           RegistrationProcess.ClearCommands();
//            //                                       })
//            //    .Verifiable();
//            //processRepos = processReposMock.Object;
            
//            //registrationProcessRouter = new RegistrationProcessRouter(() => processRepos);

//        }

//        [Given(@"that '(.*)' is the site conference having the following.*")]
//        public void GivenAConference(string conferenceName, IEnumerable<ConferenceSeatTypeDTO> givenSeatTypes)
//        {
//            var seatDto = givenSeatTypes.ToList();
//            givenConference = new ConferenceDTO(Guid.NewGuid(), "AAAA", conferenceName, "Test Conference for acceptance tests", DateTime.UtcNow.AddDays(10), seatDto);
//            givenSeats.Clear();
//            givenSeats.AddRange(seatDto);
//            foreach (var s in seatDto)
//            {
//                seats.AddSeats(s.Id, 10);
               
//            }
//        }

//        [Given(@"an|the following Order.*")]
//        public void GivenTheFollowingOrderItems(IEnumerable<SeatQuantity> items)
//        {
//            registrationCommand = new RegisterToConference
//            {
//                ConferenceId = conference.Id,
//                OrderId = _orderId,
//                Seats = items.ToList()
//            };

//            givenOrderItems.AddRange(registrationCommand.Seats);
//            Debug.WriteLine(FormatDebugText(registrationCommand, "TEST", 
//                c => string.Format("Order: {0}, Conference: {1}, Seats: {2}", c.OrderId, c.ConferenceId, c.Seats.Count())));
//            memoryCommandBus.SendInvoke(registrationCommand);
            
//        }

//        [Given(@"the following registrant")]
//        public void GivenARegistrant(Table table)
//        {
//            assignRegistrantCommand = table.CreateInstance(() => new AssignRegistrantDetails { OrderId = _orderId });
//            conference.OwnerName = string.Format("{1}, {0}", assignRegistrantCommand.FirstName, assignRegistrantCommand.LastName);
//            conference.OwnerEmail = assignRegistrantCommand.Email;

//            Debug.WriteLine(FormatDebugText(assignRegistrantCommand, "TEST", x => string.Join(" ", x.Id, x.OrderId, x.FirstName, x.LastName, x.Email)));
//            memoryCommandBus.SendInvoke(assignRegistrantCommand);
//        }

//        [Given(@"the registrant has entered details for payment of the order")]
//        public void GivenTheRegistrantHasEnteredDetailsForPaymentOfAnOrder()
//        {
//            // TODO: there isn't currently an appropriate command or event that captures this. 

//            var cmd = new SetOrderPaymentDetails
//            {
//                OrderId = registrationCommand.OrderId,
//                PaymentInformation = "test payment info"
//            };

//            Debug.WriteLine(FormatDebugText(cmd, "TEST", x => string.Join(", ", x.PaymentInformation, x.OrderId, x.Id)));
//            memoryCommandBus.SendInvoke(cmd);
//        }

//        [Given(@"a succesfully confirmed order with an Order Access Code of (\d+)")]
//        public void GivenASuccesfullyConfirmedOrderWithAnOrderAccessCode(int accessCode)
//        {
//            ScenarioContext.Current.Pending();
//        }

//        [Given(@"the registrant makes a reservation for an order")]
//        [When(@"the registrant places a reservation")]
//        public void WhenTheRegistrantMakesAReservationForAnOrder()
//        {
//            var cmd = new MarkSeatsAsReserved
//                          {
//                              OrderId = order.Id,
//                              Id = Guid.NewGuid(),
//                              Seats = givenOrderItems.ToList(),
//                              Expiration = DateTime.UtcNow.AddMinutes(15)
//                          };
//            //Debug.WriteLine(FormatDebugText(cmd, "TEST", x => string.Format("OrderId: {0}, Id: {1}, Seats: {2}, Expiration: {3}", x.OrderId, x.Id, x.Seats.Count, x.Expiration)));
//            //memoryCommandBus.SendInvoke(cmd);
//        }

//        [When(@"the Registrant confirms (?:an|the) order")]
//        public void WhenTheRegistrantConfirmsAnOrder()
//        {
//            var cmd = new PaymentCompleted { SourceId = conference.Id, PaymentSourceId = _orderId};

//            Debug.WriteLine(FormatDebugText(cmd, "TEST", x => string.Format("PaymentSourceId: {0}, SourceId: {1}", x.PaymentSourceId, x.SourceId)));
//            memoryEventBus.PublishInvoke(cmd);
//        }

//        [When(@"the Registrant begins the payment process for (?:an|the) order")]
//        public void WhenTheRegistrantBeginsThePaymentProcessForAnOrder()
//        {
            
//        }

//        [When(@"the Registrant enters a payment for processing")]
//        public void WhenTheRegistrantEntersAPaymentForProcessing()
//        {
//            WhenTheRegistrantConfirmsAnOrder();
//        }

//        [When(@"the Registrant assigns purchased seats to attendees as below")]
//        public void WhenTheRegistrantAssignsPurchasedSeatsToAttendeesAsBelow(Table table)
//        {
//            ScenarioContext.Current.Pending();
//        }

//        [StepDefinition(@"the order reservation countdown has not expired")]
//        public void TheOrderReservationCountdownHasNotExpired()
//        {
//            Assert.False(order.Events.OfType<OrderExpired>().Any());
//        }

//        [Then(@"all order items should be confirmed")]
//        public void ThenAllOrderItemsShouldBeConfirmed()
//        {
//            Assert.True(order.Events.OfType<OrderReservationCompleted>().Any());
//        }

//        [Then(@"the countdown should be active")]
//        public void ThenTheCountdownShouldBeActive()
//        {
//            TheOrderReservationCountdownHasNotExpired();
//        }

//        [Then(@"the registrant should be able to enter a payment")]
//        public void TheRegistrantShouldBeAbleToEnterAPayment()
//        {
//            var process = processRepos.Find(x => x.OrderId == _orderId);
//            Assert.True(process.State == RegistrationProcess.ProcessState.AwaitingPayment);
//        }

//        [Then(@"a receipt indicating successful processing of payment should be created")]
//        public void ThenAReceiptIndicatingSuccessfulProcessingOfPaymentShouldBeCreated()
//        {
//            Assert.True(order.Events.OfType<OrderPaymentConfirmed>().Any());
//        }

//        [Then(@"all items on the order should be confirmed")]
//        public void ThenAllItemsOnTheOrderShouldBeConfirmed()
//        {
//          //  Assert.True(order.Events.OfType<OrderReservationCompleted>().Any());
//        }

//        [Then(@"a Registration confirmation with the Access code should be displayed")]
//        public void ThenARegistrationConfirmationWithTheAccessCodeShouldBeDisplayed()
//        {
//            ScenarioContext.Current.Pending();
//        }

//        [Then(@"the order total should be \$(\d+)")]
//        public void ThenTheOrderTotalShouldBe(decimal totalAmount)
//        {
//            ScenarioContext.Current.Pending();
//        }

//        [Then(@"the registrant should receive an email containing the receipt and access code")]
//        public void ThenTheRegistrantShouldReceiveAnEmailContainingTheReceiptAndAccessCode()
//        {
//            ScenarioContext.Current.Pending();
//        }

//        [Then(@"the Registrant should receive a message confirming the assignments")]
//        public void ThenTheRegistrantShouldReceiveMessageConfirmingTheAssignments()
//        {
//            ScenarioContext.Current.Pending();
//        }

//        [Then(@"the assigned Attendees should receive an email containing the following information")]
//        public void ThenTheAssignedAttendeesShouldReceiveAnEmailContainingTheFollowingInformation(Table table)
//        {
//            ScenarioContext.Current.Pending();
//        }

//        [Then(@"the order should show an adjustment of \$([\-?]\d+)")]
//        public void ThenTheOrderShouldShowAnAdjustmentOf(decimal adjustment)
//        {
//            ScenarioContext.Current.Pending();
//        }

//        [StepArgumentTransformation(@"following order+")]
//        public IEnumerable<SeatQuantity> SeatQuantityStepTransformer(Table table)
//        {
//            return table.Rows.Select(x => new SeatQuantity(givenSeats.First(s => s.Name.ToLower() == x["SeatType"].ToLower()).Id, Convert.ToInt32(x[1])));
//        }

//        [StepArgumentTransformation(@"following [seat|ing]+ types, prices, and availability")]
//        public IEnumerable<ConferenceSeatTypeDTO> SeatTypeTransformer(Table table)
//        {
//            var items = table.CreateSet<SeatInfo>();
//            return items.Select(x => new ConferenceSeatTypeDTO(x.Id, x.Name, x.Description, x.Price));
//        }

//        [StepArgumentTransformation(@"\$([0-9]+)")]
//        public decimal MoneyStepTransformer(string input)
//        {
//            return decimal.Parse(input);
//        }

//        private void SetupRegistrationState()
//        {
//            Assert.NotNull(registrationCommand);
//            Assert.NotNull(assignRegistrantCommand);
//        }

//    }
//}
