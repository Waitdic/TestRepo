CREATE TABLE [dbo].[PrebookLog]
(
	[ID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[AccountName] VARCHAR(255) NOT NULL,
	[AccountID] INT NOT NULL,
	[System] VARCHAR(255) NOT NULL,
	[SupplierName] VARCHAR(255) NOT NULL,
	[SupplierID] INT NOT NULL,
	[PrebookDateAndTime] DATETIME NOT NULL,
	[ResponseTime] INT NOT NULL,
	[Successful] BIT NOT NULL,
	[RequestPayload] [nvarchar](max) NOT NULL,
	[ResponsePayload] [nvarchar](max) NOT NULL
)
