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
using Conference.Specflow.Support;
using TechTalk.SpecFlow;
using Xunit;

namespace Conference.Specflow.Steps
{
    [Binding]
    public class SeatAssignmentSteps : StepDefinition
    {
        [When(@"the Registrant assign these seats")]
        public void WhenTheRegistrantAssignTheseSeats(Table table)
        {
            try
            {
                Browser.ClickAndWait(Constants.UI.SeatAssignementId, Constants.UI.SeatAssignmentPage);
            }
            catch(Exception e)
            {
                Assert.Throws(e.GetType(), () => { });
            }

            foreach (var row in table.Rows)
            {
                Browser.SetRowCells(row["seat type"], row["first name"], row["last name"], row["email address"]);
            }

            Browser.Click(Constants.UI.NextStepId);
        }
    }
}
