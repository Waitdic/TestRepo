Feature: PacificDestinationsTransfers

The feature for the PacificDestinationsTransfers

@PacificDestinationCompleteBookingJourney
Scenario: PacificDestination Complete booking journey
	Given Create request object for "PacificDestinationsTransfers"
		| DepartureID | ArrivalID | BookingReference                             |
		| 741         | 737       | PacificDestinationsTransfers-IntegrationTest |
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