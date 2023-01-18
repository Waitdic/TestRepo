Feature: PacificDestinationsTransfers

The feature for the PacificDestinationsTransfers

@PacificDestinationCompleteBookingJourney
Scenario: PacificDestination Complete booking journey
	Given Create request object for "PacificDestinationsTransfers"
		| DepartureID | ArrivalID | BookingReference                             |
		| 741         | 737       | PacificDestinationsTransfers-IntegrationTest |
	When make a post request to each endpoint
	Then the status code should be 200
	And transfer results should have data
	And booking token and supplier reference are not empty
	And Supplier Cancellation Reference should have data