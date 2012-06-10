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

Feature: Self Registrant end to end scenario for making a Registration for a Conference site (happy path)
	In order to register for a conference
	As an Attendee
	I want to be able to register for the conference, pay for the Registration Order and associate myself with the paid Order automatically

Background: 
	Given the list of the available Order Items for the CQRS summit 2012 conference
	| seat type                 | rate | quota |
	| General admission         | $199 | 100   |
	| CQRS Workshop             | $500 | 100   |
	| Additional cocktail party | $50  | 100   |
	And the selected Order Items
	| seat type                 | quantity |
	| General admission         | 1        |
	| Additional cocktail party | 1        |


Scenario: Make a reservation with the selected Order Items
	When the Registrant proceed to make the Reservation		
	Then the Reservation is confirmed for all the selected Order Items
	And these Order Items should be reserved
		| seat type                 | quantity |
		| General admission         | 1        |
		| Additional cocktail party | 1        |
	And these Order Items should not be reserved
		| seat type     |
		| CQRS Workshop |
	And the total should read $249
	And the countdown started


Scenario: Checkout:Registrant Details
	Given the Registrant proceed to make the Reservation
	And the Registrant enter these details
	| first name | last name | email address            |
	| Gregory    | Weber     | gregoryweber@contoso.com |
	When the Registrant proceed to Checkout:Payment
	Then the payment options should be offered for a total of $249

Scenario: Checkout:Payment and sucessfull Order completed
	Given the Registrant proceed to make the Reservation
	And the Registrant enter these details
	| first name | last name | email address            |
	| Gregory    | Weber     | gregoryweber@contoso.com |
	And the Registrant proceed to Checkout:Payment
	When the Registrant proceed to confirm the payment
    Then the Registration process was successful
	And the Order should be created with the following Order Items
	| seat type                 | quantity |
	| General admission         | 1        |
	| Additional cocktail party | 1        |


Scenario: Allocate all purchased Seats
	Given the Registrant proceed to make the Reservation
	And the Registrant enter these details
	| first name | last name | email address            |
	| Gregory    | Weber     | gregoryweber@contoso.com |
	And the Registrant proceed to Checkout:Payment
	And the Registrant proceed to confirm the payment
    And the Registration process was successful
	And the Order should be created with the following Order Items
	| seat type                 | quantity |
	| General admission         | 1        |
	| Additional cocktail party | 1        |
	When the Registrant assign these seats
	| seat type                 | first name | last name | email address            |
	| General admission         | Gregory    | Weber     | gregoryweber@contoso.com |
	| Additional cocktail party | Gregory    | Weber     | gregoryweber@contoso.com |
	Then these seats are assigned
	| seat type                 | quantity |
	| General admission         | 1        |
	| Additional cocktail party | 1        |


