CREATE TABLE [dbo].[BookingCountryLookup](
	[BookingCountryLookupID] int IDENTITY(1,1) NOT NULL,
	[Source] varchar(30) NOT NULL,
	[TPBookingCountryCode] varchar(30) NOT NULL,
	[BookingCountryCode] varchar(30) NOT NULL
);