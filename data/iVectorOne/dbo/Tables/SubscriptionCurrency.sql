CREATE TABLE [dbo].[SubscriptionCurrency](
	[SubscriptionID] [int] NOT NULL,
	[CurrencyCode] [varchar](10) NOT NULL,
	[ISOCurrencyID] [int] NULL,
	CONSTRAINT [SubscriptionCurrencyCode] PRIMARY KEY CLUSTERED ([SubscriptionID] ASC, [CurrencyCode] ASC)
) ON [PRIMARY]
GO