CREATE TABLE [dbo].[Currency](
	[CurrencyID] int IDENTITY(1,1) NOT NULL,
	[Source] varchar(30) NOT NULL,
	[ThirdPartyCurrencyCode] varchar(20) NOT NULL,
	[CurrencyCode] varchar(20) NOT NULL,
 CONSTRAINT [PK_CurrencyLookup] PRIMARY KEY CLUSTERED ([CurrencyID] ASC)
);