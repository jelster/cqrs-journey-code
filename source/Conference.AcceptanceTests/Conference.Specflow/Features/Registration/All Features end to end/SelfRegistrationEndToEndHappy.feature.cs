﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:1.8.1.0
//      SpecFlow Generator Version:1.8.0.0
//      Runtime Version:4.0.30319.261
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace Conference.Specflow.Features.Registration.AllFeaturesEndToEnd
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "1.8.1.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public partial class SelfRegistrantEndToEndScenarioForMakingARegistrationForAConferenceSiteHappyPathFeature : Xunit.IUseFixture<SelfRegistrantEndToEndScenarioForMakingARegistrationForAConferenceSiteHappyPathFeature.FixtureData>, System.IDisposable
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "SelfRegistrationEndToEndHappy.feature"
#line hidden
        
        public SelfRegistrantEndToEndScenarioForMakingARegistrationForAConferenceSiteHappyPathFeature()
        {
            this.TestInitialize();
        }
        
        public static void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Self Registrant end to end scenario for making a Registration for a Conference si" +
                    "te (happy path)", "In order to register for a conference\r\nAs an Attendee\r\nI want to be able to regis" +
                    "ter for the conference, pay for the Registration Order and associate myself with" +
                    " the paid Order automatically", ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        public static void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        public virtual void TestInitialize()
        {
        }
        
        public virtual void ScenarioTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioSetup(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioStart(scenarioInfo);
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        public virtual void FeatureBackground()
        {
#line 6
#line 7
 testRunner.Given("the \'CQRS summit 2012 conference\' site conference");
#line hidden
            TechTalk.SpecFlow.Table table1 = new TechTalk.SpecFlow.Table(new string[] {
                        "seat type",
                        "rate"});
            table1.AddRow(new string[] {
                        "General admission",
                        "$199"});
            table1.AddRow(new string[] {
                        "Pre-con Workshop with Greg Young",
                        "$500"});
            table1.AddRow(new string[] {
                        "Additional cocktail party",
                        "$50"});
#line 8
 testRunner.And("the following seating types and prices", ((string)(null)), table1);
#line hidden
            TechTalk.SpecFlow.Table table2 = new TechTalk.SpecFlow.Table(new string[] {
                        "seat type",
                        "quantity"});
            table2.AddRow(new string[] {
                        "General admission",
                        "1"});
            table2.AddRow(new string[] {
                        "Additional cocktail party",
                        "1"});
#line 13
 testRunner.And("the following Order Items", ((string)(null)), table2);
#line hidden
            TechTalk.SpecFlow.Table table3 = new TechTalk.SpecFlow.Table(new string[] {
                        "Promotional Code",
                        "Discount",
                        "Quota",
                        "Scope",
                        "Cumulative"});
            table3.AddRow(new string[] {
                        "COPRESENTER",
                        "10%",
                        "Unlimited",
                        "Additional cocktail party",
                        "Exclusive"});
#line 17
 testRunner.And("the Promotional Codes", ((string)(null)), table3);
#line hidden
        }
        
        public virtual void SetFixture(SelfRegistrantEndToEndScenarioForMakingARegistrationForAConferenceSiteHappyPathFeature.FixtureData fixtureData)
        {
        }
        
        void System.IDisposable.Dispose()
        {
            this.ScenarioTearDown();
        }
        
        [Xunit.FactAttribute()]
        [Xunit.TraitAttribute("FeatureTitle", "Self Registrant end to end scenario for making a Registration for a Conference si" +
            "te (happy path)")]
        [Xunit.TraitAttribute("Description", "Make a reservation with the selected Order Items")]
        public virtual void MakeAReservationWithTheSelectedOrderItems()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Make a reservation with the selected Order Items", ((string[])(null)));
#line 21
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line 22
 testRunner.Given("the Registrant apply the \'COPRESENTER\' Promotional Code");
#line 23
 testRunner.And("the \'COPRESENTER\' Coupon item should show a value of -$5");
#line 24
 testRunner.When("the Registrant proceed to make the Reservation\tfor the selected Order Items");
#line 25
 testRunner.Then("the Reservation is confirmed for all the selected Order Items");
#line 26
 testRunner.And("the total should read $244");
#line 27
 testRunner.And("the countdown started");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Xunit.FactAttribute()]
        [Xunit.TraitAttribute("FeatureTitle", "Self Registrant end to end scenario for making a Registration for a Conference si" +
            "te (happy path)")]
        [Xunit.TraitAttribute("Description", "Checkout:Registrant Details")]
        public virtual void CheckoutRegistrantDetails()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Checkout:Registrant Details", ((string[])(null)));
#line 30
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table4 = new TechTalk.SpecFlow.Table(new string[] {
                        "First name",
                        "Last name",
                        "email address"});
            table4.AddRow(new string[] {
                        "John",
                        "Smith",
                        "johnsmith@contoso.com"});
#line 31
 testRunner.Given("the Registrant enter these details", ((string)(null)), table4);
#line 34
 testRunner.And("the Registrant details are valid");
#line 36
 testRunner.When("the Registrant proceed to Checkout:Payment");
#line 37
 testRunner.Then("the payment options shoule be offered");
#line 38
 testRunner.And("the countdown has decreased within the allowed timeslot for holding the Reservati" +
                    "on");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Xunit.FactAttribute()]
        [Xunit.TraitAttribute("FeatureTitle", "Self Registrant end to end scenario for making a Registration for a Conference si" +
            "te (happy path)")]
        [Xunit.TraitAttribute("Description", "Checkout:Payment and sucessfull Order completed")]
        public virtual void CheckoutPaymentAndSucessfullOrderCompleted()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Checkout:Payment and sucessfull Order completed", ((string[])(null)));
#line 40
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line 41
 testRunner.Given("Checkout:Registrant Details completed");
#line 42
 testRunner.And("the countdown has decreased within the allowed timeslot for holding the Reservati" +
                    "on");
#line 43
 testRunner.And("the Registrant select one of the offered payment options");
#line 44
 testRunner.When("the Registrant proceed to confirm the payment");
#line 45
    testRunner.Then("a receipt will be received from the payment provider indicating success with some" +
                    " transaction id");
#line 46
 testRunner.And("a Registration confirmation with the Access code should be displayed");
#line 47
 testRunner.And("an email with the Access Code will be send to the registered email.");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Xunit.FactAttribute()]
        [Xunit.TraitAttribute("FeatureTitle", "Self Registrant end to end scenario for making a Registration for a Conference si" +
            "te (happy path)")]
        [Xunit.TraitAttribute("Description", "AllocateSeats")]
        public virtual void AllocateSeats()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("AllocateSeats", ((string[])(null)));
#line 50
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line 51
testRunner.Given("the ConfirmSuccessfulRegistration for the selected Order Items");
#line 52
testRunner.And("the Order Access code is 6789");
#line hidden
            TechTalk.SpecFlow.Table table5 = new TechTalk.SpecFlow.Table(new string[] {
                        "First name",
                        "Last name",
                        "email address",
                        "Seat type"});
            table5.AddRow(new string[] {
                        "John",
                        "Smith",
                        "johnsmith@contoso.com",
                        "General admission"});
            table5.AddRow(new string[] {
                        "John",
                        "Smith",
                        "johnsmith@contoso.com",
                        "Additional cocktail party"});
#line 53
testRunner.And("the Registrant assign the purchased seats to attendees as following", ((string)(null)), table5);
#line 57
testRunner.Then("the Regsitrant should be get a Seat Assignment confirmation");
#line hidden
            TechTalk.SpecFlow.Table table6 = new TechTalk.SpecFlow.Table(new string[] {
                        "Access code",
                        "email address",
                        "Seat type"});
            table6.AddRow(new string[] {
                        "6789-1",
                        "johnsmith@contoso.com",
                        "General admission"});
            table6.AddRow(new string[] {
                        "6789-2",
                        "johnsmith@contoso.com",
                        "Additional cocktail party"});
#line 58
testRunner.And("the Attendees should get an email informing about the conference and the Seat Typ" +
                    "e with Seat Access Code", ((string)(null)), table6);
#line hidden
            this.ScenarioCleanup();
        }
        
        [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "1.8.1.0")]
        [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
        public class FixtureData : System.IDisposable
        {
            
            public FixtureData()
            {
                SelfRegistrantEndToEndScenarioForMakingARegistrationForAConferenceSiteHappyPathFeature.FeatureSetup();
            }
            
            void System.IDisposable.Dispose()
            {
                SelfRegistrantEndToEndScenarioForMakingARegistrationForAConferenceSiteHappyPathFeature.FeatureTearDown();
            }
        }
    }
}
#pragma warning restore
#endregion
