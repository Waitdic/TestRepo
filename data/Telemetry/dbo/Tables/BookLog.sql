CREATE TABLE [dbo].[BookLog]
(
	[ID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[AccountName] VARCHAR(255) NOT NULL,
	[AccountID] INT NOT NULL,
	[System] VARCHAR(255) NOT NULL,
	[SupplierName] VARCHAR(255) NOT NULL,
	[SupplierID] INT NOT NULL,
	[PropertyID] INT NOT NULL,
	[BookDateAndTime] DATETIME NOT NULL,
	[ResponseTime] INT NOT NULL,
	[Successful] BIT NOT NULL,
	[RequestPayload] [nvarchar](max) NOT NULL,
	[ResponsePayload] [nvarchar](max) NOT NULL,
	[BookingReference] VARCHAR(255) NOT NULL,
	[SupplierBookingReference] VARCHAR(255) NOT NULL,
	[LeadGuestName] VARCHAR(255) NOT NULL,
	[DepartureDate] DATETIME NOT NULL,
	[Duration] INT NOT NULL,
	[TotalPrice] DECIMAL NOT NULL,
	[Currency] VARCHAR(255) NOT NULL,
	[EstimatedGBPPrice] DECIMAL NOT NULL
)
