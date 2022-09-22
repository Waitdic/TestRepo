CREATE TABLE [dbo].[AccountCurrency](
	[AccountID] [int] NOT NULL,
	[CurrencyCode] [varchar](10) NOT NULL,
	[ISOCurrencyID] [int] NULL,
	CONSTRAINT [AccountCurrencyCode] PRIMARY KEY CLUSTERED ([AccountID] ASC, [CurrencyCode] ASC)
) ON [PRIMARY]
GO