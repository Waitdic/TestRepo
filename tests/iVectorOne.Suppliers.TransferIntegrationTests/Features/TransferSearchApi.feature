@TransferSearch
Feature: TransferSearchApi

A short summary of the feature

Scenario: Search records
	Given Create request object for search
	When make a post request to "v2/transfers/search"
	Then the status code should be 200

Scenario: Prebook record
	Given Create request object for prebook
	When make a post request to prebook "v2/transfers/prebook"
	Then the status code should be 200

Scenario: Book record
	Given Create request object for book
	When make a post request to book "v2/transfers/book"
	Then the status code should be 200

Scenario: Cancel record
	Given Create request object for cancel
	When make a post request to cancel "v2/transfers/cancel"
	Then the status code should be 200
