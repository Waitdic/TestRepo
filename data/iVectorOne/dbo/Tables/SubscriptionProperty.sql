CREATE TABLE [dbo].[SubscriptionProperty](
	[SubscriptionID] [int] NOT NULL,
	[PropertyID] [int] NOT NULL,
	[CentralPropertyID] [int] NOT NULL,
	CONSTRAINT [PK_SubscriptionProperty] PRIMARY KEY CLUSTERED ([SubscriptionID] ASC, [PropertyID] ASC)
) ON [PRIMARY]
GO