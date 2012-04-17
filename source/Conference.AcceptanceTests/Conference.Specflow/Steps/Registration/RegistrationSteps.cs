using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Registration;
using Registration.Events;
using Registration.ReadModel;
using TechTalk.SpecFlow;
using Xunit;
using Xunit.Sdk;

namespace Conference.Specflow.Steps.Registration
{
    [Binding]
    public class RegistrationSteps
    {
        private Registrant registrant;
        private Order order;
        private ConferenceDTO conference;
        
        [Given(@"that '(.*)' is the site conference")]
        public void GivenAConferenceNamed(string conferenceName)
        {
            conference = new ConferenceDTO(Guid.NewGuid(), "AAA", conferenceName, string.Empty, Enumerable.Empty<ConferenceSeatTypeDTO>());
            order = new Order(Guid.NewGuid(), conference.Id, Enumerable.Empty<OrderItem>());
        }

        [Given(@"the following seating types and prices")]
        public void GivenTheFollowingSeatingTypesAndPrices(IEnumerable<ConferenceSeatTypeDTO> givenSeatTypes)
        {
            conference.Seats.AddRange(givenSeatTypes);
        }

        [Given(@"the following Order Items")]
        public void GivenTheFollowingOrderItems(IEnumerable<OrderItem> orderItems)
        {
           order.UpdateSeats(orderItems);
        }

        [Given(@"the following registrant")]
        public void GivenARegistrant(Table table)
        {
            registrant = new Registrant
                             {
                                 FirstName = table.Rows[0][0], 
                                 LastName = table.Rows[0][1], 
                                 Email = table.Rows[0][2]
                             };
        }

        [Given(@"the registrant has entered details for payment of an order")]
        public void GivenTheRegistrantHasEnteredDetailsForPaymentOfAnOrder()
        {
            WhenTheRegistrantBeginsThePaymentProcessForAnOrder();
        }

        [Given(@"a succesfully confirmed order with an Order Access Code of (\d+)")]
        public void GivenASuccesfullyConfirmedOrderWithAnOrderAccessCode(int accessCode)
        {
            ScenarioContext.Current.Pending();
        }

        [When(@"the Registrant makes a reservation for an order")]
        public void WhenTheRegistrantMakesAReservationForAnOrder()
        {
            order.MarkAsReserved(DateTime.UtcNow.AddSeconds(120), order.Items.Select(x => new SeatQuantity(x.SeatType, x.Quantity)));
        }

        [When(@"the Registrant assigns purchased seats to attendees as below")]
        public void WhenTheRegistrantAssignsPurchasedSeatsToAttendeesAsBelow(Table table)
        {
            ScenarioContext.Current.Pending();
        }

        [When(@"the Registrant confirms an order")]
        public void WhenTheRegistrantConfirmsAnOrder()
        {
            order.ConfirmPayment();
        }

        [When(@"the Registrant begins the payment process for an order")]
        public void WhenTheRegistrantBeginsThePaymentProcessForAnOrder()
        {
            order.AssignRegistrant(registrant.FirstName, registrant.LastName, registrant.Email);
            WhenTheRegistrantMakesAReservationForAnOrder();
        }

        [When(@"the Registrant enters a payment for processing")]
        public void WhenTheRegistrantEntersAPaymentForProcessing()
        {
            WhenTheRegistrantMakesAReservationForAnOrder();
            WhenTheRegistrantConfirmsAnOrder();
        }

        [StepDefinition(@"the order reservation countdown has not expired")]
        public void TheOrderReservationCountdownHasNotExpired()
        {
            Assert.True(order.ReservationExpirationDate > DateTime.UtcNow);
        }

        [Then(@"all order items should be confirmed")]
        public void ThenAllOrderItemsShouldBeConfirmed()
        {
            Assert.True(order.State == Order.States.Confirmed);
        }

        [Then(@"the order total should be \$(\d+)")]
        public void ThenTheOrderTotalShouldBe(decimal totalAmount)
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"the countdown should be active")]
        public void ThenTheCountdownShouldBeActive()
        {
            TheOrderReservationCountdownHasNotExpired();
        }

        [Then(@"the registrant should be able to enter a payment")]
        public void TheRegistrantShouldBeAbleToEnterAPayment()
        {
           Assert.True(order.Events.OfType<OrderRegistrantAssigned>().Any());
        }

        [Then(@"a receipt indicating successful processing of payment should be created")]
        public void ThenAReceiptIndicatingSuccessfulProcessingOfPaymentShouldBeCreated()
        {
            Assert.True(order.Events.OfType<OrderReservationCompleted>().Any());
            Assert.True(order.State == Order.States.Confirmed);
        }

        [Then(@"a Registration confirmation with the Access code should be displayed")]
        public void ThenARegistrationConfirmationWithTheAccessCodeShouldBeDisplayed()
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

        [Then(@"all items on the order should be confirmed")]
        public void ThenAllItemsOnTheOrderShouldBeConfirmed()
        {
            Assert.True(order.Events.OfType<OrderReservationCompleted>().Any());
        }

        [Then(@"the order should show an adjustment of \$([\-?]\d+)")]
        public void ThenTheOrderShouldShowAnAdjustmentOf(decimal adjustment)
        {
            ScenarioContext.Current.Pending();
        }

        [StepArgumentTransformation(@"following order items")]
        public IEnumerable<OrderItem> OrderItemStepTransformer(Table table)
        {
            var inter = table.Rows.Select(x => new OrderItem(conference.Seats
                .Where(conf => conf.Description.ToLower() == x[0].ToLower())
                .Select(guid => guid.Id).First(), Convert.ToInt32(x[1])));
            return inter;
        }

        [StepArgumentTransformation(@"following [seat|ing]+ types")]
        public IEnumerable<ConferenceSeatTypeDTO> SeatTypeTransformer(Table table)
        {
            return table.Rows.Select(tableRow => 
                new ConferenceSeatTypeDTO(Guid.NewGuid(), tableRow["SeatType"], Convert.ToDouble(tableRow["Rate"].Replace(CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol, ""))));
        }

        [StepArgumentTransformation(@"\$(\w+)")]
        public decimal MoneyStepTransformer(string input)
        {
            return decimal.Parse(input);
        }


    }
}
