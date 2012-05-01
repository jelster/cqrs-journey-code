﻿# ==============================================================================================================
# Microsoft patterns & practices
# CQRS Journey project
# ==============================================================================================================
# ©2012 Microsoft. All rights reserved. Certain content used with permission from contributors
# http://cqrsjourney.github.com/contributors/members
# Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance 
# with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
# Unless required by applicable law or agreed to in writing, software distributed under the License is 
# distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
# See the License for the specific language governing permissions and limitations under the License.
# ==============================================================================================================

# Next release
@Ignore
Feature: Promotional Code scenarios for applying Promotional Codes to Seat Types
	In order to apply a Promotional Code for one ore more Seat Types
	As a Registrant
	I want to be able to enter a Promotional Code and get the specified price reduction

Background: 
	Given the list of the available Order Items for the CQRS summit 2012 conference with the slug code
	| seat type                 | rate |
	| General admission         | $199 |
	| CQRS Workshop             | $500 |
	| Additional cocktail party | $50  |
	And the Promotional Codes
	| Promotional Code | Discount | Quota     | Scope                            | Cumulative |
	| SPEAKER123       | 100%     | Unlimited | All                              |            |
	| VOLUNTEER        | 100%     | Unlimited | General admission                |            |
	| COPRESENTER      | 10%      | Unlimited | Additional cocktail party        | Exclusive  |
	| WS10             | $20      | Unlimited | All                              | VOLUNTEER  |
	| 1TIMEPRECON      | 50%      | Single    | CQRS Workshop                    |            |
	| CONFONLY         | $50      | Single    | General admission, CQRS Workshop |            |


Scenario: Full Promotional Code for all selected items
	Given the selected available Order Items
	| seat type                 | quantity |
	| General admission         | 3        |
	| CQRS Workshop             | 1        |
	| Additional cocktail party | 2        |
	And the total amount should be of $1197
	When the Registrant apply the 'SPEAKER123' Promotional Code
	Then the 'SPEAKER123' Promo code should show a value of -$1197
	And the total amount should be of $0


Scenario: Partial Promotional Code for all selected items
	Given the selected available Order Items
	| seat type                 | quantity |
	| General admission         | 3        |
	| CQRS Workshop             | 1        |
	| Additional cocktail party | 2        |
	And the total amount should be of $1197
	When the Registrant apply the 'VOLUNTEER' Promotional Code
	Then the 'VOLUNTEER' Promo code should show a value of -$597
	And the total amount should be of $600


Scenario: Partial Promotional Code for none of the selected items
	Given the selected available Order Items
	| seat type                 | quantity |
	| CQRS Workshop             | 1        |
	| Additional cocktail party | 2        |
	And the total amount should be of $600
	When the Registrant apply the 'VOLUNTEER' Promotional Code
	Then the 'VOLUNTEER' Promo code will not be applied and an error message will inform about the problem
	And the total amount should be of $600


Scenario: Cumulative Promotional Codes
	Given the selected available Order Items
	| seat type                 | quantity |
	| General admission         | 3        |
	| CQRS Workshop             | 1        |
	| Additional cocktail party | 2        |
	And the total amount should be of $1197
	When the Registrant apply the 'COPRESENTER' Promotional Code
	And the Registrant apply the 'WS10' Promotional Code
	Then the 'COPRESENTER' Promotional Code item should show a value of -$10
	And the 'WS10' Promotional Code item should show a value of -$20
	And the total amount should be of $1167


Scenario: Single use Promotional Code
	Given the selected available Order Items
	| seat type                 | quantity |
	| CQRS Workshop             | 1        |
	| Additional cocktail party | 2        |
	And the total amount should be of $600
	And the Registrant apply the '1TIMEPRECON' Promotional Code
	And the total amount should be of $350
	And the Registrant proceed to complete the registration
	And the Registrant selects the Event Registration
	When the Registrant apply the '1TIMEPRECON' Promotional Code
	Then the '1TIMEPRECON' Promo code will not be applied and an error message will inform about the problem
	And the total amount should be of $600


Scenario: Mutually exclusive Promotional Code
	Given the selected available Order Items
	| seat type                 | quantity |
	| General admission         | 3        |
	| CQRS Workshop             | 1        |
	| Additional cocktail party | 2        |
	And the total amount should be of $1197
	When the Registrant apply the 'COPRESENTER' Promotional Code
	And the Registrant apply the 'VOLUNTEER' Promotional Code
	Then the 'VOLUNTEER' Promo code will not be applied and an error message will inform about the problem
	And the 'COPRESENTER' Promotional Code item should show a value of -$10
	And the total amount should be of $1187


Scenario: Combine only Promotional Code
	Given the selected available Order Items
	| seat type                 | quantity |
	| General admission         | 3        |
	| CQRS Workshop             | 1        |
	| Additional cocktail party | 2        |
	And the total amount should be of $1197
	When the Registrant apply the 'WS10' Promotional Code
	And the Registrant apply the 'VOLUNTEER' Promotional Code
	Then the 'VOLUNTEER' Promo code should show a value of -$597
	And the 'WS10' Promotional Code item should show a value of -$10
	And the total amount should be of $590


Scenario: Partial scope
	Given the selected available Order Items
	| seat type                 | quantity |
	| General admission         | 1        |
	| CQRS Workshop             | 1        |
	| Additional cocktail party | 1        |
	And the total amount should be of $749
	When the Registrant apply the 'CONFONLY' Promotional Code
	Then the 'CONFONLY' Promo code should show a value of -$50
	And the total amount should be of $699