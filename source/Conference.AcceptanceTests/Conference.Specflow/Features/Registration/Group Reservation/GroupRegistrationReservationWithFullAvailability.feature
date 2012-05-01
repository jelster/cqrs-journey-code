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

Feature: Registrant scenarios for registering a group of Attendees for a conference when all Seats are available in all the Seat Types
	In order to register for conference a group of Attendees
	As a Registrant
	I want to be able to select Order Items from one or many available Order Items and make a Reservation

#General preconditions for all the scenarios
Background: 
	Given the list of the available Order Items for the CQRS summit 2012 conference with the slug code GroupRegFull
	| seat type                 | rate | quota |
	| General admission         | $199 | 100   |
	| CQRS Workshop             | $500 | 100   |
	| Additional cocktail party | $50  | 100   |

#1
#Initial state	: 3 available items, 3 selected
#End state		: 3 reserved	
Scenario: All the Order Items are available and all get selected, then all get reserved
	Given the selected Order Items
	| seat type                 | quantity |
	| General admission         | 3        |
	| CQRS Workshop             | 1        |
	| Additional cocktail party | 2        |
	When the Registrant proceed to make the Reservation
	Then the Reservation is confirmed for all the selected Order Items
	And the total should read $1197

	
#2
#Initial state	: 3 available items, 2 selected 
#End state		: 2 reserved	
Scenario: All the Order Items are available and some get selected, then only the selected get reserved
	Given the selected Order Items
	| seat type                 | quantity |
	| General admission         | 2        |
	| Additional cocktail party | 2        |
	When the Registrant proceed to make the Reservation
	Then the Reservation is confirmed for all the selected Order Items
	And the total should read $498


#3
#Initial state	: 3 available items, 2 selected
#End state		: 2 offered waitlisted

# Next release
@Ignore
Scenario: All the Order Items are available and all get waitlisted
	Given the selected Order Items
	| seat type                 | quantity |
	| General admission         | 2        |
	| Additional cocktail party | 2        |
	And these Seat Types becomes unavailable before the Registrant make the reservation
	| seat type                 |
	| General admission         |
	| Additional cocktail party |
	When the Registrant proceed to make the Reservation			
	Then the Registrant is offered to be waitlisted for these Order Items
	| seat type                 | quantity |
	| General admission         | 2        |
	| Additional cocktail party | 2        |


#4
#Initial state	: 3 available items, 3 selected
#End state		: 1 reserved, 1 partially reserved, 1 waitlisted

# Next release
@Ignore
Scenario: All the Order Items are available, 1 becomes partially available, 1 becomes unavailable and 1 is available,
	      then 2 are partially offered to get waitlisted and 1 get reserved
	Given the selected Order Items
	| seat type                 | quantity |
	| General admission         | 2        |
	| CQRS Workshop             | 1        |
	| Additional cocktail party | 2        |
	And these Seat Types becomes partially unavailable before the Registrant make the reservation
	| seat type         |
	| General admission |
	And these Seat Types becomes unavailable before the Registrant make the reservation
	| seat type                 |
	| Additional cocktail party |
	When the Registrant proceed to make the Reservation			
	Then the Registrant is offered to be waitlisted for these Order Items
	| seat type                 | quantity |
	| General admission         | 1        |
	| Additional cocktail party | 2        |
	And These other Order Items get reserved
	| seat type         | quantity |
	| General admission | 1        |
	| CQRS Workshop     | 1        |
	And the total should read $699


