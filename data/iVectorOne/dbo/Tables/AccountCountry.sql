CREATE TABLE [dbo].[AccountCountry](
	[AccountID] [int] NOT NULL,
	[CountryCode] [varchar](10) NOT NULL,
	[ISOCountryCode] [varchar](10) NOT NULL,
	[Country] VARCHAR(50) NOT NULL, 
    CONSTRAINT [AccountCountryCode] PRIMARY KEY CLUSTERED ([AccountID] ASC, [CountryCode] ASC)
) ON [PRIMARY]