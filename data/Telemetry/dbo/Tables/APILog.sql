CREATE TABLE [dbo].[APILog](
	[APILogID] int IDENTITY(1,1) NOT NULL,
	[Type] varchar(30) NOT NULL,
	[Time] datetime NOT NULL,
	[RequestLog] varchar(max) NOT NULL,
	[ResponseLog] varchar(max) NOT NULL,
	[AccountID] int NOT NULL,
	[Success] bit NOT NULL,
	[BookingID] INT NOT NULL default 0,
PRIMARY KEY CLUSTERED ([APILogID] ASC));