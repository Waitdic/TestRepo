
Feature: TransferSearchApi

Feature for testing individual apis

@TransferSearch
Scenario: Search records
	Given Create request object for search for "GowaySydneyTransfers"
		| DepartureID | ArrivalID |
		| 184         | 187       |
	When make a post request to "v2/transfers/search"
	Then the status code should be 200
	And transfer results should have data

@TransferPrebook
Scenario: Prebook record
	Given Create request object for prebook for "GowaySydneyTransfers"
		| DepartureID | ArrivalID | BookingReference                     |
		| 184         | 187       | GowaySydneyTransfers-IntegrationTest |
	When make a post request to prebook "v2/transfers/prebook"
	Then the status code should be 200
	And booking token and supplier reference are not empty

@TransferBook
Scenario: Book record
	Given Create request object for book for "GowaySydneyTransfers"
		| DepartureID | ArrivalID | BookingReference                     |
		| 184         | 187       | GowaySydneyTransfers-IntegrationTest |
	When make a post request to book "v2/transfers/book"
	Then the status code should be 200
	And booking token and supplier reference are not empty

@TransferCancel
Scenario: Cancel record
	Given Create request object for cancel for "GowaySydneyTransfers"
		| DepartureID | ArrivalID | BookingReference                     |
		| 184         | 187       | GowaySydneyTransfers-IntegrationTest |
	When make a post request to cancel "v2/transfers/cancel"
	Then the status code should be 200
	And Supplier Cancellation Reference should have data