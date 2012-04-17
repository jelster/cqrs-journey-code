Feature: Self Registrant end to end scenario for making a Registration for a Conference site (happy path)
	In order to register for a conference
	As an Attendee
	I want to be able to register and pay for a conference so that I can reserve spots at the conference

Background: 
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

# checkout scenarios could belong in a different feature
Scenario: Checkout:Registrant Details
	When the Registrant begins the payment process for an order
	Then the registrant should be able to enter a payment 

Scenario: Checkout:Payment and successful Order completed
	Given the registrant has entered details for payment of an order
	And the order reservation countdown has not expired	
	When the Registrant enters a payment for processing
	Then a receipt indicating successful processing of payment should be created
	And a Registration confirmation with the Access code should be displayed
	And the registrant should receive an email containing the receipt and access code 


Scenario: AllocateSeats
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
