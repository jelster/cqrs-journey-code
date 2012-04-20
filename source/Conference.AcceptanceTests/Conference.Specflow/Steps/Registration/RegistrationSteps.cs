using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Azure;
using Azure.Messaging;
using Common;
using Microsoft.Practices.Unity;
using Microsoft.ServiceBus.Messaging;
using Registration;
using Registration.Commands;
using Registration.Events;
using Registration.Handlers;
using Registration.ReadModel;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using Xunit;
using Moq;

namespace Conference.Specflow.Steps.Registration
{
    [Binding]
    public class RegistrationSteps
    {
        private AssignRegistrantDetails registrant;
        private OrderViewModelGenerator orderViewModelGenerator;
        private RegisterToConference conferenceRegistration;
        private SetOrderPaymentDetails paymentDetails;

        private Order order { get; set; }


        private ConferenceInfo conference;
        private MemoryCommandBus commandBus;
        private MemoryEventBus eventBus;
        private readonly RegistrationProcess registrationProcess = new RegistrationProcess();
        private RegistrationProcessRouter registrationProcessRouter;
        private OrderCommandHandler orderCommandHandler;
         
 
        private IRepository<Order> orderRepos;
        private IProcessRepository processRepo;
      
        private IViewRepository viewRepo;
        private MockRepository mre;

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

        [After]
        public void VerifyExpectations()
        {
            mre.Verify();
        }

        // TODO: this should be replaced with a less brittle, more standardized init pattern that can be shared with other projects
        private void RegisterLocalInfrastructure()
        {
            commandBus.Register(orderCommandHandler);
            commandBus.Register(registrationProcessRouter);
            eventBus.Register(registrationProcessRouter);
        }

        private void CreateAndSetupMocks()
        {
            mre = new MockRepository(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock, CallBase = true};

            var orderReposMock = mre.Create<IRepository<Order>>();
            orderReposMock
                .Setup(x => x.Save(It.IsAny<Order>()))
                .Callback<Order>(o => order = o)
                .Verifiable();

            orderReposMock
                .Setup(x => x.Find(It.IsAny<Guid>()))
                .Returns(order).Verifiable();

            orderRepos = orderReposMock.Object;

            var repoMock = mre.Create<IProcessRepository>();
            var processRef = registrationProcess;
            repoMock
                .Setup(x => x.Query<RegistrationProcess>())
                .Returns(() => (new[] { processRef }).AsQueryable()).Verifiable();

            repoMock.Setup(x => x.Save(It.IsAny<RegistrationProcess>()))
                .Callback<RegistrationProcess>(proc => commandBus.Send(registrationProcess.Commands)).Verifiable();

            processRepo = repoMock.Object;
            registrationProcessRouter = new RegistrationProcessRouter(() => processRepo);
            orderCommandHandler = new OrderCommandHandler(orderRepos);

            commandBus = new MemoryCommandBus();
            eventBus = new MemoryEventBus();
        }

        [Given(@"that '(.*)' is the site conference having the following.*")]
        public void GivenAConference(string conferenceName, IEnumerable<SeatInfo> givenSeatTypes)
        {
            conference.Name = conferenceName;
            conference.SeatInfos = givenSeatTypes.ToList();
        }

        [Given(@"the following Order.*")]
        public void GivenTheFollowingOrderItems(IEnumerable<SeatQuantity> items)
        {
            conferenceRegistration = new RegisterToConference
            {
                ConferenceId = conference.Id,
                OrderId = Guid.NewGuid(),
                Seats = items.ToList()
            };

            commandBus.Send(conferenceRegistration); //soonest possible moment we can send this command. If called in wrong order, it should fail messily. Make that explicit?
        }

        [Given(@"the following registrant")]
        public void GivenARegistrant(Table table)
        {
            registrant = table.CreateInstance(() => new AssignRegistrantDetails {OrderId = conferenceRegistration.OrderId});
            conference.OwnerName = string.Format("{1}, {0}", registrant.FirstName, registrant.LastName);
            conference.OwnerEmail = registrant.Email;
            commandBus.Send(registrant);
        }

        [Given(@"the registrant has entered details for payment of an order")]
        public void GivenTheRegistrantHasEnteredDetailsForPaymentOfAnOrder()
        {
            var cmd = new SetOrderPaymentDetails 
            {
                OrderId = conferenceRegistration.OrderId, 
                PaymentInformation = "test payment info"
            };
            commandBus.Send(cmd);
        }

        [Given(@"a succesfully confirmed order with an Order Access Code of (\d+)")]
        public void GivenASuccesfullyConfirmedOrderWithAnOrderAccessCode(int accessCode)
        {
            ScenarioContext.Current.Pending();
        }

        [When(@"the Registrant makes a reservation for an order")]
        public void WhenTheRegistrantMakesAReservationForAnOrder()
        {
            var cmd = new MakeSeatReservation
                          {
                              ConferenceId = conferenceRegistration.Id, 
                              Seats = conferenceRegistration.Seats.ToList()
                          };

            commandBus.Send(cmd);
        }

        [When(@"the Registrant confirms an order")]
        public void WhenTheRegistrantConfirmsAnOrder()
        {
            var cmd = new ConfirmOrderPayment() {OrderId = order.Id};
            commandBus.Send(cmd);
        }

        [When(@"the Registrant begins the payment process for an order")]
        public void WhenTheRegistrantBeginsThePaymentProcessForAnOrder()
        {
            WhenTheRegistrantMakesAReservationForAnOrder();
        }

        [When(@"the Registrant enters a payment for processing")]
        public void WhenTheRegistrantEntersAPaymentForProcessing()
        {
            WhenTheRegistrantMakesAReservationForAnOrder();
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
            var ord = orderRepos.Find(conferenceRegistration.OrderId);
            Assert.False(ord.Events.OfType<OrderExpired>().Any());
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
           var process = processRepo.Find<RegistrationProcess>(conferenceRegistration.OrderId);
           Assert.True(process.State == RegistrationProcess.ProcessState.AwaitingPayment);
        }

        [Then(@"a receipt indicating successful processing of payment should be created")]
        public void ThenAReceiptIndicatingSuccessfulProcessingOfPaymentShouldBeCreated()
        {
            Assert.True(order.Events.OfType<OrderReservationCompleted>().Any());
            Assert.True(order.Events.OfType<OrderPaymentConfirmed>().Any());
        }

        [Then(@"all items on the order should be confirmed")]
        public void ThenAllItemsOnTheOrderShouldBeConfirmed()
        {
            Assert.True(order.Events.OfType<OrderReservationCompleted>().Any());
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

        [StepArgumentTransformation(@"following order items")]
        public IEnumerable<SeatQuantity> SeatQuantityStepTransformer(Table table)
        {
            return table.CreateSet(() => new SeatQuantity(Guid.Empty, 0));
        }

        [StepArgumentTransformation(@"following [seat|ing]+ types, prices, and availability")]
        public IEnumerable<SeatInfo> SeatTypeTransformer(Table table)
        {
            return table.CreateSet<SeatInfo>();
        }

        [StepArgumentTransformation(@"\$(\w+)")]
        public decimal MoneyStepTransformer(string input)
        {
            return decimal.Parse(input);
        }

        private void SetupRegistrationContext()
        {
            Assert.NotNull(conferenceRegistration);
            Assert.NotNull(registrant);

            

        }

    }
}
