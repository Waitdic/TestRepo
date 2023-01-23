
Feature: TransferApi

Feature for testing individual apis

@TransferSearch
Scenario: Search records
	Given Create request object for search for "GowaySydneyTransfers"
		| DepartureID | ArrivalID | BookingReference                     |
		| 184         | 187       | GowaySydneyTransfers-IntegrationTest |
	When a transfer search request is sent
	Then the status code should be 200
	And transfer results should be returned

@TransferPrebook
Scenario: Prebook record
	Given Create request object for search for "GowaySydneyTransfers"
		| DepartureID | ArrivalID | BookingReference                     |
		| 184         | 187       | GowaySydneyTransfers-IntegrationTest |
	When a transfer search request is sent
	And a transfer prebook request is sent
	Then the status code should be 200
	And a booking token and supplier reference are returned

@TransferBook
Scenario: Book record
	Given Create request object for search for "GowaySydneyTransfers"
		| DepartureID | ArrivalID | BookingReference                     |
		| 184         | 187       | GowaySydneyTransfers-IntegrationTest |
	When a transfer search request is sent
	And a transfer prebook request is sent
	And a transfer book request is sent
	Then the status code should be 200
	And a booking token and supplier reference are returned

@TransferCancel
Scenario: Cancel record
	Given Create request object for search for "GowaySydneyTransfers"
		| DepartureID | ArrivalID | BookingReference                     |
		| 184         | 187       | GowaySydneyTransfers-IntegrationTest |
	When a transfer search request is sent
	And a transfer prebook request is sent
	And a transfer book request is sent
	And a transfer precancel request is sent
	And a transfer cancel request is sent
	Then the status code should be 200
	And Supplier Cancellation Reference should have data