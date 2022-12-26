Feature: TransferSearchApi

A short summary of the feature

@tag1
Scenario: Get All records
	Given Create request object for search
	When make a post request to "v2/transfers/search"
	Then the status code should be 200

Scenario: Prebook record
	Given Create request object for prebook
	When make a post request to "v2/transfers/preook"
	Then the status code should be 200
