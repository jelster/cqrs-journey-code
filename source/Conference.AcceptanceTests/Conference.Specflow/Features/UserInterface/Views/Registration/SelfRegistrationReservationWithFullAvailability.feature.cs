﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:1.8.1.0
//      SpecFlow Generator Version:1.8.0.0
//      Runtime Version:4.0.30319.269
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace Conference.Specflow.Features.UserInterface.Views.Registration
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "1.8.1.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public partial class SelfRegistrantScenariosForMakingAReservationForAConferenceSiteWithAllOrderItemsInitiallyAvailableFeature : Xunit.IUseFixture<SelfRegistrantScenariosForMakingAReservationForAConferenceSiteWithAllOrderItemsInitiallyAvailableFeature.FixtureData>, System.IDisposable
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "SelfRegistrationReservationWithFullAvailability.feature"
#line hidden
        
        public SelfRegistrantScenariosForMakingAReservationForAConferenceSiteWithAllOrderItemsInitiallyAvailableFeature()
        {
            this.TestInitialize();
        }
        
        public static void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Self Registrant scenarios for making a Reservation for a Conference site with all" +
                    " Order Items initially available", "In order to reserve Seats for a conference\r\nAs an Attendee\r\nI want to be able to " +
                    "select an Order Item from one or many of the available Order Items and make a Re" +
                    "servation", ProgrammingLanguage.CSharp, ((string[])(null)));
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
#line 20
#line hidden
            TechTalk.SpecFlow.Table table1 = new TechTalk.SpecFlow.Table(new string[] {
                        "seat type",
                        "rate",
                        "quota"});
            table1.AddRow(new string[] {
                        "General admission",
                        "$199",
                        "100"});
            table1.AddRow(new string[] {
                        "CQRS Workshop",
                        "$500",
                        "100"});
            table1.AddRow(new string[] {
                        "Additional cocktail party",
                        "$50",
                        "100"});
#line 21
 testRunner.Given("the list of the available Order Items for the CQRS summit 2012 conference", ((string)(null)), table1);
#line hidden
            TechTalk.SpecFlow.Table table2 = new TechTalk.SpecFlow.Table(new string[] {
                        "seat type",
                        "quantity"});
            table2.AddRow(new string[] {
                        "General admission",
                        "1"});
            table2.AddRow(new string[] {
                        "CQRS Workshop",
                        "1"});
            table2.AddRow(new string[] {
                        "Additional cocktail party",
                        "1"});
#line 26
 testRunner.And("the selected Order Items", ((string)(null)), table2);
#line hidden
        }
        
        public virtual void SetFixture(SelfRegistrantScenariosForMakingAReservationForAConferenceSiteWithAllOrderItemsInitiallyAvailableFeature.FixtureData fixtureData)
        {
        }
        
        void System.IDisposable.Dispose()
        {
            this.ScenarioTearDown();
        }
        
        [Xunit.FactAttribute()]
        [Xunit.TraitAttribute("FeatureTitle", "Self Registrant scenarios for making a Reservation for a Conference site with all" +
            " Order Items initially available")]
        [Xunit.TraitAttribute("Description", "All the Order Items are offered and all get reserved")]
        public virtual void AllTheOrderItemsAreOfferedAndAllGetReserved()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("All the Order Items are offered and all get reserved", ((string[])(null)));
#line 35
this.ScenarioSetup(scenarioInfo);
#line 20
this.FeatureBackground();
#line 36
 testRunner.Given("the total should read $749");
#line 37
 testRunner.When("the Registrant proceed to make the Reservation");
#line 38
 testRunner.Then("the Reservation is confirmed for all the selected Order Items");
#line hidden
            TechTalk.SpecFlow.Table table3 = new TechTalk.SpecFlow.Table(new string[] {
                        "seat type",
                        "quantity"});
            table3.AddRow(new string[] {
                        "General admission",
                        "1"});
            table3.AddRow(new string[] {
                        "CQRS Workshop",
                        "1"});
            table3.AddRow(new string[] {
                        "Additional cocktail party",
                        "1"});
#line 39
 testRunner.And("these Order Items should be reserved", ((string)(null)), table3);
#line 44
 testRunner.And("the total should read $749");
#line 45
 testRunner.And("the countdown started");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Xunit.FactAttribute()]
        [Xunit.TraitAttribute("FeatureTitle", "Self Registrant scenarios for making a Reservation for a Conference site with all" +
            " Order Items initially available")]
        [Xunit.TraitAttribute("Description", "All the Order Items are offered and all get unavailable")]
        public virtual void AllTheOrderItemsAreOfferedAndAllGetUnavailable()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("All the Order Items are offered and all get unavailable", ((string[])(null)));
#line 50
this.ScenarioSetup(scenarioInfo);
#line 20
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table4 = new TechTalk.SpecFlow.Table(new string[] {
                        "seat type"});
            table4.AddRow(new string[] {
                        "General admission"});
            table4.AddRow(new string[] {
                        "CQRS Workshop"});
            table4.AddRow(new string[] {
                        "Additional cocktail party"});
#line 51
 testRunner.Given("these Seat Types becomes unavailable before the Registrant make the reservation", ((string)(null)), table4);
#line 56
 testRunner.When("the Registrant proceed to make the Reservation with seats already reserved");
#line hidden
            TechTalk.SpecFlow.Table table5 = new TechTalk.SpecFlow.Table(new string[] {
                        "seat type",
                        "selected",
                        "message"});
            table5.AddRow(new string[] {
                        "General admission",
                        "",
                        "Sold out"});
            table5.AddRow(new string[] {
                        "CQRS Workshop",
                        "",
                        "Sold out"});
            table5.AddRow(new string[] {
                        "Additional cocktail party",
                        "",
                        "Sold out"});
#line 57
 testRunner.Then("the Registrant is offered to select any of these available seats", ((string)(null)), table5);
#line hidden
            this.ScenarioCleanup();
        }
        
        [Xunit.FactAttribute()]
        [Xunit.TraitAttribute("FeatureTitle", "Self Registrant scenarios for making a Reservation for a Conference site with all" +
            " Order Items initially available")]
        [Xunit.TraitAttribute("Description", "All Seat Types are offered, one get reserved and two get unavailable")]
        public virtual void AllSeatTypesAreOfferedOneGetReservedAndTwoGetUnavailable()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("All Seat Types are offered, one get reserved and two get unavailable", ((string[])(null)));
#line 66
this.ScenarioSetup(scenarioInfo);
#line 20
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table6 = new TechTalk.SpecFlow.Table(new string[] {
                        "seat type"});
            table6.AddRow(new string[] {
                        "CQRS Workshop"});
            table6.AddRow(new string[] {
                        "Additional cocktail party"});
#line 67
 testRunner.Given("these Seat Types becomes unavailable before the Registrant make the reservation", ((string)(null)), table6);
#line 71
 testRunner.When("the Registrant proceed to make the Reservation with seats already reserved");
#line hidden
            TechTalk.SpecFlow.Table table7 = new TechTalk.SpecFlow.Table(new string[] {
                        "seat type",
                        "selected",
                        "message"});
            table7.AddRow(new string[] {
                        "CQRS Workshop",
                        "",
                        "Sold out"});
            table7.AddRow(new string[] {
                        "Additional cocktail party",
                        "",
                        "Sold out"});
#line 72
 testRunner.Then("the Registrant is offered to select any of these available seats", ((string)(null)), table7);
#line hidden
            TechTalk.SpecFlow.Table table8 = new TechTalk.SpecFlow.Table(new string[] {
                        "seat type",
                        "quantity"});
            table8.AddRow(new string[] {
                        "General admission",
                        "1"});
#line 76
 testRunner.And("the selected Order Items", ((string)(null)), table8);
#line hidden
            TechTalk.SpecFlow.Table table9 = new TechTalk.SpecFlow.Table(new string[] {
                        "seat type",
                        "quantity"});
            table9.AddRow(new string[] {
                        "General admission",
                        "1"});
#line 79
 testRunner.And("these Order Items should be reserved", ((string)(null)), table9);
#line 82
 testRunner.And("the total should read $199");
#line 83
 testRunner.And("the countdown started");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Xunit.FactAttribute()]
        [Xunit.TraitAttribute("FeatureTitle", "Self Registrant scenarios for making a Reservation for a Conference site with all" +
            " Order Items initially available")]
        [Xunit.TraitAttribute("Description", "Find a purchased Order")]
        public virtual void FindAPurchasedOrder()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Find a purchased Order", ((string[])(null)));
#line 86
this.ScenarioSetup(scenarioInfo);
#line 20
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table10 = new TechTalk.SpecFlow.Table(new string[] {
                        "seat type",
                        "quantity"});
            table10.AddRow(new string[] {
                        "General admission",
                        "3"});
            table10.AddRow(new string[] {
                        "CQRS Workshop",
                        "1"});
            table10.AddRow(new string[] {
                        "Additional cocktail party",
                        "2"});
#line 87
 testRunner.Given("the selected Order Items", ((string)(null)), table10);
#line 92
 testRunner.And("the Registrant proceed to make the Reservation");
#line hidden
            TechTalk.SpecFlow.Table table11 = new TechTalk.SpecFlow.Table(new string[] {
                        "first name",
                        "last name",
                        "email address"});
            table11.AddRow(new string[] {
                        "William",
                        "Weber",
                        "William@Weber.com"});
#line 93
 testRunner.And("the Registrant enter these details", ((string)(null)), table11);
#line 96
 testRunner.And("the Registrant proceed to Checkout:Payment");
#line 97
 testRunner.When("the Registrant proceed to confirm the payment");
#line 98
    testRunner.Then("the Registration process was successful");
#line hidden
            TechTalk.SpecFlow.Table table12 = new TechTalk.SpecFlow.Table(new string[] {
                        "seat type",
                        "quantity"});
            table12.AddRow(new string[] {
                        "General admission",
                        "3"});
            table12.AddRow(new string[] {
                        "CQRS Workshop",
                        "1"});
            table12.AddRow(new string[] {
                        "Additional cocktail party",
                        "2"});
#line 99
 testRunner.And("the Order should be found with the following Order Items", ((string)(null)), table12);
#line hidden
            this.ScenarioCleanup();
        }
        
        [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "1.8.1.0")]
        [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
        public class FixtureData : System.IDisposable
        {
            
            public FixtureData()
            {
                SelfRegistrantScenariosForMakingAReservationForAConferenceSiteWithAllOrderItemsInitiallyAvailableFeature.FeatureSetup();
            }
            
            void System.IDisposable.Dispose()
            {
                SelfRegistrantScenariosForMakingAReservationForAConferenceSiteWithAllOrderItemsInitiallyAvailableFeature.FeatureTearDown();
            }
        }
    }
}
#pragma warning restore
#endregion
