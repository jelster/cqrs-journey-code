using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace Conference.Specflow.Steps.Registration
{
    [Binding]
    public class RegistrationSteps
    {
        private string conferenceName;

        [Given(@"that '(.*)' is the site conference")]
        public void GivenAConferenceNamed(string conference)
        {
            conferenceName = conference;
        }

        [Given(@"the following seating types and prices")]
        public void GivenTheFollowingSeatingTypesAndPrices(Table table)
        {
            ScenarioContext.Current.Pending();
        }

        [Given(@"the following Order Items")]
        public void GivenTheFollowingOrderItems(Table table)
        {
            ScenarioContext.Current.Pending();
        }

        [Given(@"the following registrant")]
        public void GivenARegistrant(Table table)
        {
            ScenarioContext.Current.Pending();
        }

        [Given(@"the registrant has entered details for payment of an order")]
        public void GivenTheRegistrantHasEnteredDetailsForPaymentOfAnOrder()
        {
            ScenarioContext.Current.Pending();
        }

        [Given(@"a succesfully confirmed order with an Order Access Code of (\d+)")]
        public void GivenASuccesfullyConfirmedOrderWithAnOrderAccessCode(int accessCode)
        {
            ScenarioContext.Current.Pending();
        }

        [When(@"the Registrant makes a reservation for an order")]
        public void WhenTheRegistrantMakesAReservationForAnOrder()
        {
            ScenarioContext.Current.Pending();
        }

        [When(@"the Registrant assigns purchased seats to attendees as below")]
        public void WhenTheRegistrantAssignsPurchasedSeatsToAttendeesAsBelow(Table table)
        {
            ScenarioContext.Current.Pending();
        }

        [When(@"the Registrant confirms an order")]
        public void WhenTheRegistrantConfirmsAnOrder()
        {
            ScenarioContext.Current.Pending();
        }

        [When(@"the Registrant begins the payment process for an order")]
        public void WhenTheRegistrantBeginsThePaymentProcessForAnOrder()
        {
            ScenarioContext.Current.Pending();
        }

        [When(@"the Registrant enters a payment for processing")]
        public void WhenTheRegistrantEntersAPaymentForProcessing()
        {
            ScenarioContext.Current.Pending();
        }

        [StepDefinition(@"the order reservation countdown has not expired")]
        public void TheOrderReservationCountdownHasNotExpired()
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"all order items should be confirmed")]
        public void ThenAllOrderItemsShouldBeConfirmed()
        {
            ScenarioContext.Current.Pending();
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
            ScenarioContext.Current.Pending();
        }

        [Then(@"a receipt indicating successful processing of payment should be created")]
        public void ThenAReceiptIndicatingSuccessfulProcessingOfPaymentShouldBeCreated()
        {
            ScenarioContext.Current.Pending();
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

        [Then(@"the Registrant should receive message confirming the assignments")]
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
            ScenarioContext.Current.Pending();
        }

        [Then(@"the order should show an adjustment of \$([\-?]\d+)")]
        public void ThenTheOrderShouldShowAnAdjustmentOf(decimal adjustment)
        {
            ScenarioContext.Current.Pending();
        }


    }
}
