Feature: GowaySydneyTransfers

The feature for the GowaySydneyTransfers

@GowayCompleteBookingJourney
Scenario: Goway Complete booking journey
	Given Create request object for "GowaySydneyTransfers"
		| DepartureID | ArrivalID | BookingReference                     |
		| 184         | 187       | GowaySydneyTransfers-IntegrationTest |
	When a transfer search request is sent
	Then the status code should be 200
	And transfer results should be returned
	When a transfer prebook request is sent
	Then the status code should be 200
	And a booking token and supplier reference are returned
	When a transfer book request is sent
	Then the status code should be 200
	And a booking token and supplier reference are returned
	When a transfer precancel request is sent
	Then the status code should be 200
	When a transfer cancel request is sent
	Then the status code should be 200
	And Supplier Cancellation Reference should have data


