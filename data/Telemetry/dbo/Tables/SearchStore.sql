CREATE TABLE [dbo].[SearchStore]
(
	[SearchStoreID] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	[AccountName] VARCHAR(255) NOT NULL,
	[AccountID] INT NOT NULL,
	[System] VARCHAR(255) NOT NULL,
	[Successful] BIT NOT NULL,
	[SearchDateAndTime] DATETIME NOT NULL,
	[PropertiesRequested] INT NOT NULL,
	[PropertiesReturned] INT NOT NULL,
	[PreProcessTime] INT NOT NULL,
	[MaxSupplierTime] INT NOT NULL,
	[MaxDedupeTime] INT NOT NULL,
	[PostProcessTime] INT NOT NULL,
	[TotalTime] INT NOT NULL
)
