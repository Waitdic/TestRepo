CREATE TABLE [dbo].[Booking]
(
    [BookingID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [BookingReference] VARCHAR(400) NOT NULL,
    [SupplierBookingReference] VARCHAR(400) NOT NULL,
    [AccountID] INT NOT NULL,
    [SupplierID] INT NOT NULL,
    [PropertyID] INT NOT NULL,
    [Status] varchar(10) NOT NULL,
    [LeadGuestName] VARCHAR(400) NOT NULL,
    [BookingDateTime] DATETIME NOT NULL,
    [DepartureDate] DATETIME NOT NULL,
    [Duration] INT NOT NULL,
    [TotalPrice] DECIMAL (14, 2) NOT NULL,
    [ISOCurrencyID] INT NOT NULL,
    [EstimatedGBPPrice] DECIMAL (14, 2) NOT NULL
    CONSTRAINT UC_AccountReference UNIQUE (BookingReference, AccountID)
)