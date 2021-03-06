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

Feature: Self Registrant scenarios for making a Reservation for a Conference site with all Order Items initially available
	In order to reserve Seats for a conference
	As an Attendee
	I want to be able to select an Order Item from one or many of the available Order Items and make a Reservation

#General preconditions for all the scenarios
Background: 
	Given the list of the available Order Items for the CQRS summit 2012 conference
	| seat type                 | rate | quota |
	| General admission         | $199 | 100   |
	| CQRS Workshop             | $500 | 100   |
	| Additional cocktail party | $50  | 100   |
	And the selected Order Items
	| seat type                 | quantity |
	| General admission         | 1        |
	| CQRS Workshop             | 1        |
	| Additional cocktail party | 1        |


#Initial state	: 3 available
#End state		: 3 reserved	
Scenario: All the Order Items are offered and all get reserved
	Given the total should read $749
	When the Registrant proceed to make the Reservation		
	Then the Reservation is confirmed for all the selected Order Items
	And these Order Items should be reserved
		| seat type                 | quantity |
		| General admission         | 1        |
		| CQRS Workshop             | 1        |
		| Additional cocktail party | 1        |
	And the total should read $749
	And the countdown started


#Initial state	: 3 available
#End state		: 3 unavailable
Scenario: All the Order Items are offered and all get unavailable
	Given these Seat Types becomes unavailable before the Registrant make the reservation
	| seat type                 |
	| General admission         |
	| CQRS Workshop             |
	| Additional cocktail party |
	When the Registrant proceed to make the Reservation with seats already reserved 		
	Then the Registrant is offered to select any of these available seats
	| seat type                 | selected | message                                    |
	| General admission         | 0        | Could not reserve all the requested seats. |
	| CQRS Workshop             | 0        | Could not reserve all the requested seats. |
	| Additional cocktail party | 0        | Could not reserve all the requested seats. |


#Initial state	: 3 available
#End state		: 2 unavailable, 1 reserved
Scenario: All Seat Types are offered, one get reserved and two get unavailable
	Given these Seat Types becomes unavailable before the Registrant make the reservation
	| seat type                 |
	| CQRS Workshop             |
	| Additional cocktail party |
	When the Registrant proceed to make the Reservation with seats already reserved 		
	Then the Registrant is offered to select any of these available seats
	| seat type                 | selected | message                                    |
	| CQRS Workshop             | 0        | Could not reserve all the requested seats. |
	| Additional cocktail party | 0        | Could not reserve all the requested seats. |
	And these Order Items should be reserved
	| seat type         | quantity |
	| General admission | 1        |
	And the total should read $199
	And the countdown started


Scenario: Find a purchased Order
	Given the selected Order Items
	| seat type                 | quantity |
	| General admission         | 3        |
	| CQRS Workshop             | 1        |
	| Additional cocktail party | 2        |
	And the Registrant proceed to make the Reservation
	And the Registrant enter these details
	| first name | last name | email address         |
	| William    | Weber     | William@Weber.com     |
	And the Registrant proceed to Checkout:Payment
	When the Registrant proceed to confirm the payment
    Then the Registration process was successful
	And the Order should be found with the following Order Items
	| seat type                 | quantity |
	| General admission         | 3        |
	| CQRS Workshop             | 1        |
	| Additional cocktail party | 2        |
