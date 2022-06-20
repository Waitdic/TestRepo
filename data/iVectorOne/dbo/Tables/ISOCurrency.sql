CREATE TABLE [dbo].[ISOCurrency](
	[ISOCurrencyID] int IDENTITY(1,1) NOT NULL,
	[CurrencyCode] varchar(20) NOT NULL,
 CONSTRAINT [PK_ISOCurrencyLookup] PRIMARY KEY CLUSTERED ([ISOCurrencyID] ASC)
);