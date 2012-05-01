# ==============================================================================================================
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
	I want to be able to register and pay for a conference so that I can reserve spots at the conference

Background: 
<<<<<<< HEAD
	Given that 'CQRS summit 2012 conference' is the site conference
	And the following seating types and prices
	| SeatType                         | Rate |
	| General admission                | $199 |
	| Pre-con Workshop with Greg Young | $500 |
	| Additional cocktail party        | $50  |	
	And the following Order Items
	| SeatType                 | quantity |
	| General admission         | 1        |
	| Additional cocktail party | 1        |
	And the following Promotional Codes
	| Promotional Code | Discount | Quota     | Scope                     | Cumulative |
	| COPRESENTER      | 10%      | Unlimited | Additional cocktail party | Exclusive  |
	And the following registrant
	| First name | Last name | email address         |
	| John       | Smith     | johnsmith@contoso.com |	
	

Scenario: Making a reservation
	When the Registrant makes a reservation for an order
	Then all items on the order should be confirmed
	And the order should show an adjustment of $-5
	And the order total should be $224
	And the countdown should be active 
=======
	Given the list of the available Order Items for the CQRS summit 2012 conference with the slug code SelfRegE2Ehappy
	| seat type                 | rate | quota |
	| General admission         | $199 | 100   |
	| CQRS Workshop             | $500 | 100   |
	| Additional cocktail party | $50  | 100   |
	And the selected Order Items
	| seat type                 | quantity |
	| General admission         | 1        |
	| Additional cocktail party | 1        |
#	And the Promotional Codes
#	| Promotional Code | Discount | Quota     | Scope                     | Cumulative |
#	| COPRESENTER      | 10%      | Unlimited | Additional cocktail party | Exclusive  |


Scenario: Make a reservation with the selected Order Items
	When the Registrant proceed to make the Reservation		
	Then the Reservation is confirmed for all the selected Order Items
	And these Order Items should be reserved
		| seat type                 |
		| General admission         |
		| Additional cocktail party |
	And these Order Items should not be reserved
		| seat type     |
		| CQRS Workshop |
	And the total should read $249
	And the countdown started

>>>>>>> dev

# checkout scenarios could belong in a different feature
Scenario: Checkout:Registrant Details
<<<<<<< HEAD
	When the Registrant begins the payment process for an order
	Then the registrant should be able to enter a payment 

Scenario: Checkout:Payment and successful Order completed
	Given the registrant has entered details for payment of an order
	And the order reservation countdown has not expired	
	When the Registrant enters a payment for processing
	Then a receipt indicating successful processing of payment should be created
	And a Registration confirmation with the Access code should be displayed
	And the registrant should receive an email containing the receipt and access code 

=======
	Given the Registrant proceed to make the Reservation
	And the Registrant enter these details
	| First name | Last name | email address            |
	| Gregory    | Weber     | gregoryweber@contoso.com |
	When the Registrant proceed to Checkout:Payment
	Then the payment options should be offered for a total of $249

Scenario: Checkout:Payment and sucessfull Order completed
	Given the Registrant proceed to make the Reservation
	And the Registrant enter these details
	| First name | Last name | email address            |
	| Gregory    | Weber     | gregoryweber@contoso.com |
	And the Registrant proceed to Checkout:Payment
	When the Registrant proceed to confirm the payment
    Then the message 'You will receive a confirmation e-mail in a few minutes.' will show up
	And the Order should be created with the following Order Items
		| seat type                 | quantity |
		| General admission         | 1        |
		| Additional cocktail party | 1        |
>>>>>>> dev

# Next release
@Ignore
Scenario: AllocateSeats
<<<<<<< HEAD
Given a succesfully confirmed order with an Order Access Code of 6789
When the Registrant assigns purchased seats to attendees as below
	| First name | Last name | email address         | Seat type                 |
	| John       | Smith     | johnsmith@contoso.com | General admission         |
	| John       | Smith     | johnsmith@contoso.com | Additional cocktail party |
Then the Registrant should receive a message confirming the assignments
And the assigned Attendees should receive an email containing the following information
	| Access code | email address         | Seat type                 |
	| 6789-1      | johnsmith@contoso.com | General admission         |
	| 6789-2      | johnsmith@contoso.com | Additional cocktail party |
=======
Given the ConfirmSuccessfulRegistration for the selected Order Items
And the Order Access code is 6789
And the Registrant assign the purchased seats to attendees as following
	| First name | Last name | email address            | Seat type                 |
	| Gregory    | Weber     | gregoryweber@contoso.com | General admission         |
	| Gregory    | Weber     | gregoryweber@contoso.com | Additional cocktail party |
Then the Regsitrant should be get a Seat Assignment confirmation
And the Attendees should get an email informing about the conference and the Seat Type with Seat Access Code
	| Access code | email address            | Seat type                 |
	| 6789-1      | gregoryweber@contoso.com | General admission         |
	| 6789-2      | gregoryweber@contoso.com | Additional cocktail party |


# Next release
@Ignore
Scenario: Make a reservation with the selected Order Items and a Promo Code
	Given the Registrant apply the 'COPRESENTER' Promotional Code
	And the 'COPRESENTER' Promo code should show a value of -$5
	When the Registrant proceed to make the Reservation		
	Then the Reservation is confirmed for all the selected Order Items
	And the total should read $244
	And the countdown started
>>>>>>> dev
