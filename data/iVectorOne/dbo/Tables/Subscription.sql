CREATE TABLE [dbo].[Subscription](
	[SubscriptionID] [int] IDENTITY(1,1) NOT NULL,
	[Login] [varchar](100) NOT NULL,
	[Password] [varchar](500) NOT NULL,
	[DummyResponses] [bit] NULL,
	[PropertyTPRequestLimit] [smallint] NOT NULL,
	[SearchTimeoutSeconds] [smallint] NOT NULL,
	[LogMainSearchError] [bit] NOT NULL,
	[CurrencyCode] [varchar](3) NOT NULL,
	[Environment] [varchar](7) NOT NULL,
	[TenantID] [int] NULL
 CONSTRAINT [PK_Subscription] PRIMARY KEY NONCLUSTERED 
(
	[SubscriptionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 [Status] VARCHAR(8) NOT NULL DEFAULT 'active', 
    CONSTRAINT [CK_Unique_SubscriptionLogin] UNIQUE NONCLUSTERED 
(
	[Login] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Subscription] ADD  CONSTRAINT [DF_Subscription_TenantID]  DEFAULT ((0)) FOR [TenantID]
GO

ALTER TABLE [dbo].[Subscription]  WITH CHECK ADD  CONSTRAINT [FK_Subscription_Tenant] FOREIGN KEY([TenantID])
REFERENCES [dbo].[Tenant] ([TenantID])
GO

ALTER TABLE [dbo].[Subscription] CHECK CONSTRAINT [FK_Subscription_Tenant]
GO