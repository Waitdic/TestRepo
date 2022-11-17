CREATE TABLE [dbo].[TransferSupplierAPILog]
(
	[TransferSupplierAPILogID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[AccountID] INT NOT NULL,
	[SupplierID] INT NOT NULL,
	[Type] varchar(30) NOT NULL,
	[Title] varchar(200) NOT NULL,
	[RequestDateTime] DATETIME NOT NULL,
	[ResponseTime] INT NOT NULL,
	[Successful] BIT NOT NULL,
	[RequestLog] [nvarchar](max) NOT NULL,
	[ResponseLog] [nvarchar](max) NOT NULL,
	[TransferBookingID] INT NOT NULL default 0,
)