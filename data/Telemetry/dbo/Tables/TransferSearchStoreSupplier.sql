﻿CREATE TABLE [dbo].[TransferSearchStoreSupplier]
(
	[TransferSearchStoreSupplierID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[TransferSearchStoreID] UNIQUEIDENTIFIER NOT NULL,
	[AccountName] VARCHAR(255) NOT NULL,
	[AccountID] INT NOT NULL,
	[System] VARCHAR(255) NOT NULL,
	[SupplierName] VARCHAR(255) NOT NULL,
	[SupplierID] INT NOT NULL,
	[Successful] BIT NOT NULL,
	[Timeout] BIT NOT NULL,
	[SearchDateAndTime] DATETIME NOT NULL,
	[ResultsReturned] INT NOT NULL,
	[PreProcessTime] INT NOT NULL,
	[SupplierTime] INT NOT NULL,
	[PostProcessTime] INT NOT NULL,
	[TotalTime] INT NOT NULL,
)
