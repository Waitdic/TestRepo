Feature: ExoToursTransfers

The feature for the ExoToursTransfers

@ExoToursCompleteBookingJourney
Scenario: ExoTours Complete booking journey
	Given Create request object for "EXOToursTransfers"
		| DepartureID | ArrivalID | BookingReference                  |
		| 244         | 257       | ExoToursTransfers-IntegrationTest |
	When make a post request to each endpoint
	Then the status code should be 200
	And transfer results should have data
	And booking token and supplier reference are not empty
	And Supplier Cancellation Reference should have data