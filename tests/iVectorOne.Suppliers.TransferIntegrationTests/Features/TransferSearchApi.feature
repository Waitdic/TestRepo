
Feature: TransferSearchApi

A short summary of the feature
@TransferSearch
Scenario: Search records
	Given Create request object for search for "GowaySydneyTransfers"
	| DepartureID | ArrivalID | 
	| 184		  | 187 |
		When make a post request to "v2/transfers/search"
	Then the status code should be 200
	And transfer results should have data

@TransferSearch
Scenario: Prebook record
	Given Create request object for prebook for "GowaySydneyTransfers"
	| DepartureID | ArrivalID | 
	| 184		  | 187 |
	When make a post request to prebook "v2/transfers/prebook"
	Then the status code should be 200
	And booking token and supplier reference are not empty

@TransferSearch
Scenario: Book record
	Given Create request object for book for "GowaySydneyTransfers"
	| DepartureID | ArrivalID | 
	| 184		  | 187 |
	When make a post request to book "v2/transfers/book"
	Then the status code should be 200
	And booking token and supplier reference are not empty

@TransferSearch
Scenario: Cancel record
	Given Create request object for cancel for "GowaySydneyTransfers"
	| DepartureID | ArrivalID | 
	| 184		  | 187 |
	When make a post request to cancel "v2/transfers/cancel"
	Then the status code should be 200
	And  Supplier Cancellation Reference should have data 

@GowayCompleteBookingJourney
Scenario: Goway Complete booking journey
	Given Create request object for "GowaySydneyTransfers"
	| DepartureID | ArrivalID | 
	| 184		  | 187 |
	When make a post request to each endpoint
	Then the status code should be 200
	And transfer results should have data
	And booking token and supplier reference are not empty
	And Supplier Cancellation Reference should have data 


@ExoToursCompleteBookingJourney
Scenario: ExoTours Complete booking journey
	Given Create request object for "EXOToursTransfers"
	| DepartureID | ArrivalID | 
	| 244		  | 257 |
	When make a post request to each endpoint
	Then the status code should be 200
	And transfer results should have data
	And booking token and supplier reference are not empty
	And Supplier Cancellation Reference should have data
	
@PacificDestinationCompleteBookingJourney
Scenario: PacificDestination Complete booking journey
	Given Create request object for "PacificDestinationsTransfers"
	| DepartureID | ArrivalID | 
	| 741		  | 737 |
	When make a post request to each endpoint
	Then the status code should be 200
	And transfer results should have data
	And booking token and supplier reference are not empty
	And Supplier Cancellation Reference should have data