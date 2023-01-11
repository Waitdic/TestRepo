CREATE TABLE [dbo].[TransferSearchStoreApi]
(
	[TransferSearchStoreID] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	[AccountName] VARCHAR(255) NOT NULL,
	[AccountID] INT NOT NULL,
	[System] VARCHAR(255) NOT NULL,
	[Successful] BIT NOT NULL,
	[SearchDateAndTime] DATETIME NOT NULL,
	[ResultsReturned] INT NOT NULL,
	[PreProcessTime] INT NOT NULL,
	[MaxSupplierTime] INT NOT NULL,
	[PostProcessTime] INT NOT NULL,
	[TotalTime] INT NOT NULL
)
