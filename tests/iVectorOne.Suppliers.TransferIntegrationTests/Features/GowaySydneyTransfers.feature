Feature: GowaySydneyTransfers

The feature for the GowaySydneyTransfers

@GowayCompleteBookingJourney
Scenario: Goway Complete booking journey
	Given Create request object for "GowaySydneyTransfers"
		| DepartureID | ArrivalID | BookingReference                     |
		| 184         | 187       | GowaySydneyTransfers-IntegrationTest |
	When make a post request to each endpoint
	Then the status code should be 200
	And transfer results should have data
	And booking token and supplier reference are not empty
	And Supplier Cancellation Reference should have data
