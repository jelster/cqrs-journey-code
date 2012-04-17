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
                    "ter and pay for a conference so that I can reserve spots at the conference", ProgrammingLanguage.CSharp, ((string[])(null)));
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
 testRunner.Given("that \'CQRS summit 2012 conference\' is the site conference");
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
 testRunner.And("the following Promotional Codes", ((string)(null)), table3);
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
        [Xunit.TraitAttribute("Description", "Making a reservation")]
        public virtual void MakingAReservation()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Making a reservation", ((string[])(null)));
#line 21
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line 22
 testRunner.When("the Registrant makes a reservation for an order");
#line 23
 testRunner.Then("all items on the order should be confirmed");
#line 24
 testRunner.And("the order should show an adjustment of $-5");
#line 25
 testRunner.And("the order total should be $224");
#line 26
 testRunner.And("the countdown should be active");
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
#line 29
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
#line 30
 testRunner.Given("the following registrant", ((string)(null)), table4);
#line 33
 testRunner.When("the Registrant begins the payment process for an order");
#line 34
 testRunner.And("the order reservation countdown has not expired");
#line 35
 testRunner.Then("the registrant should be able to enter a payment");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Xunit.FactAttribute()]
        [Xunit.TraitAttribute("FeatureTitle", "Self Registrant end to end scenario for making a Registration for a Conference si" +
            "te (happy path)")]
        [Xunit.TraitAttribute("Description", "Checkout:Payment and successful Order completed")]
        public virtual void CheckoutPaymentAndSuccessfulOrderCompleted()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Checkout:Payment and successful Order completed", ((string[])(null)));
#line 37
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line 38
 testRunner.Given("the registrant has entered details for payment of an order");
#line 39
 testRunner.And("the order reservation countdown has not expired");
#line 40
 testRunner.When("the Registrant enters a payment for processing");
#line 41
 testRunner.Then("a receipt indicating successful processing of payment should be created");
#line 42
 testRunner.And("a Registration confirmation with the Access code should be displayed");
#line 43
 testRunner.And("the registrant should receive an email containing the receipt and access code");
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
#line 46
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line 47
testRunner.Given("a succesfully confirmed order with an Order Access Code of 6789");
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
#line 48
testRunner.When("the Registrant assigns purchased seats to attendees as below", ((string)(null)), table5);
#line 52
testRunner.Then("the Registrant should receive message confirming the assignments");
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
#line 53
testRunner.And("the assigned Attendees should receive an email containing the following informati" +
                    "on", ((string)(null)), table6);
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
