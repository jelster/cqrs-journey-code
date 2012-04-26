Feature: Self Registrant end to end scenario for making a Registration for a Conference site (happy path)
	In order to register for a conference
	As an Attendee
	I want to be able to register and pay for a conference so that I can reserve spots at the conference

Background: 
	Given that 'CQRS summit 2012 conference' is the site conference having the following seating types, prices, and availability
	| Name                             | Price | Quantity |
	| General admission                | $199  | 10       |
	| Pre-con Workshop with Greg Young | $500  | 10       |
	| Additional cocktail party        | $50   | 10       |
	And the following Order
	| SeatType                 | Quantity |
	| General admission         | 1        |
	| Additional cocktail party | 1        |
	And the following Promotional Codes
	| Promotional Code | Discount | Quota     | Scope                     | Cumulative |
	| COPRESENTER      | 10%      | Unlimited | Additional cocktail party | Exclusive  |
	And the following registrant
	| FirstName | LastName | Email                 |
	| John      | Smith    | johnsmith@contoso.com |
	And the registrant makes a reservation for an order	
	

Scenario: Making a reservation	 
	Then all items on the order should be confirmed
	And the order should show an adjustment of $-5
	And the order total should be $224
	And the countdown should be active 

# checkout scenarios could belong in a different feature
Scenario: Checkout:Registrant Details	 
	Given the registrant has entered details for payment of the order
	When the Registrant begins the payment process for the order
	Then the registrant should be able to enter a payment 

Scenario: Checkout:Payment and successful Order completed	
	When the Registrant enters a payment for processing
	Then a receipt indicating successful processing of payment should be created
	And a Registration confirmation with the Access code should be displayed
	And the registrant should receive an email containing the receipt and access code 
	And the order reservation countdown has not expired


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
