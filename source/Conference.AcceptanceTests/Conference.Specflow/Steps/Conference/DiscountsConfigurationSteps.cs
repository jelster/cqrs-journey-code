﻿// ==============================================================================================================
// Microsoft patterns & practices
// CQRS Journey project
// ==============================================================================================================
// ©2012 Microsoft. All rights reserved. Certain content used with permission from contributors
// http://cqrsjourney.github.com/contributors/members
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance 
// with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software distributed under the License is 
// distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and limitations under the License.
// ==============================================================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace Conference.Specflow.Steps.Conference
{
    [Binding]
    public class DiscountsConfigurationSteps
    {
        [Given(@"the Seat Types configuration")]
        public void GivenTheSeatTypesConfiguration(Table table)
        {
            ScenarioContext.Current.Pending();
        }

        [Given(@"the Business Customer selects 'Add new Promotional code' option")]
        public void GivenTheBusinessCustomerSelectsAddNewPromotionalCodeOption()
        {
            ScenarioContext.Current.Pending();
        }

        [Given(@"the Business Customer enter the 'NEWCODE' Promotional Code and these attributes")]
        public void GivenTheBusinessCustomerEnterTheNEWCODEPromotionalCodeAndTheseAttributes(Table table)
        {
            ScenarioContext.Current.Pending();
        }

        [When(@"the 'Save' option is selected")]
        public void WhenTheSaveOptionIsSelected()
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"the new Promotional Code is added to the list of existing codes")]
        public void ThenTheNewPromotionalCodeIsAddedToTheListOfExistingCodes()
        {
            ScenarioContext.Current.Pending();
        }

    }
}
