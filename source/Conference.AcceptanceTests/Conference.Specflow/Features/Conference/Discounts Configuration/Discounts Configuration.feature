﻿Feature:  Promotional Codes Configuration scenarios for creating and editing Promotional Codes settings
	In order to create or update a Promotional Code
	As a Business Customer
	I want to be able to create or update a Promotional Code and set its properties

Background: 
Given the following Promotional Codes
		 | Promotional Code | Discount | Quota     | Scope | Cumulative |
		 | SPEAKER123       | 100%     | Unlimited | All   |            |
And the following Seat Types
	| seat type                        | quota |
	| General admission                | 500   |
	| Pre-con Workshop with Greg Young | 100   |
	| Additional cocktail party        | 600   |



# New Promo Code  Happy path
Scenario: Create a new Promotional Code
When the following Promotional Code is entered
	| Promotional Code | Discount | Quota     | Scope             | Cumulative |
	| NEWCODE          | 10%      | Unlimited | General admission | SPEAKER123 |
And the 'Save' option is selected
Then the new Promotional Code is added to the list of existing codes


# New Promo Code Sad path
Scenario: Create a new Promotional Code with with exceeding quota
Given the Business Customer selects 'Add new Promotional code' option
And the following Promotional Code is entered
	| Promotional Code | Discount | Quota | Scope             | Cumulative |
	| NEWCODE          | 10%      | 1000  | General admission | SPEAKER123 |
When the 'Save' option is selected
Then an error message will show up describing that the quota value exceeds the total seats for the specified Seat Type


# Update Promo Code Happy path
Scenario: Update an existing Promotional Code
Given the Business Customer selects 'SPEAKER123' Promotional Code
And the Scope is updated with value 'Pre-con Workshop with Greg Young'
And the Quota is updated with the value '50'
When the 'Save' option is selected
Then updated values are reflected in the selected Promotional Code


# Update Promo Code Sad path
Scenario: Update an existing Promotional Code with exceeding quota
Given the Business Customer selects 'SPEAKER123' Promotional Code
And the Scope is updated with value 'Pre-con Workshop with Greg Young'
And the Quota is updated with the value '200'
When the 'Save' option is selected
Then an error message will show up describing that the quota value exceeds the total seats for the specified Seat Type

