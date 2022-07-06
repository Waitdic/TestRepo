CREATE TABLE [dbo].[SubscriptionCountry](
	[SubscriptionID] [int] NOT NULL,
	[CountryCode] [varchar](10) NOT NULL,
	[ISOCountryCode] [varchar](10) NOT NULL,
	CONSTRAINT [SubscriptionCountryCode] PRIMARY KEY CLUSTERED ([SubscriptionID] ASC, [CountryCode] ASC)
) ON [PRIMARY]