CREATE TABLE [dbo].[Account](
	[AccountID] [int] IDENTITY(1,1) NOT NULL,
	[Login] [varchar](100) NOT NULL,
	[Password] [varchar](500) NOT NULL,
	[DummyResponses] [bit] NULL,
	[PropertyTPRequestLimit] [smallint] NOT NULL,
	[SearchTimeoutSeconds] [smallint] NOT NULL,
	[LogMainSearchError] [bit] NOT NULL,
	[CurrencyCode] [varchar](3) NOT NULL,
	[Environment] [varchar](7) NOT NULL,
	[TenantID] [int] NULL,
	[CustomerID] [int] NULL,
	[Status] VARCHAR(8) NOT NULL DEFAULT 'active',
	CONSTRAINT [PK_Account] PRIMARY KEY NONCLUSTERED ([AccountID] ASC),
	CONSTRAINT [CK_Unique_AccountLogin] UNIQUE NONCLUSTERED ([Login] ASC)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Account] ADD  CONSTRAINT [DF_Account_TenantID]  DEFAULT ((0)) FOR [TenantID]
GO

ALTER TABLE [dbo].[Account]  WITH CHECK ADD  CONSTRAINT [FK_Account_Tenant] FOREIGN KEY([TenantID])
REFERENCES [dbo].[Tenant] ([TenantID])
GO

ALTER TABLE [dbo].[Account] CHECK CONSTRAINT [FK_Account_Tenant]
GO