CREATE TABLE [dbo].[SupplierSubscription](
	[SupplierSubscriptionID] [int] IDENTITY(1,1) NOT NULL,
	[SupplierID] [smallint] NOT NULL,
	[SubscriptionID] [int] NOT NULL,
 CONSTRAINT [PK_SupplierSubscription_1] PRIMARY KEY NONCLUSTERED 
(
	[SupplierSubscriptionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[SupplierSubscription]  WITH CHECK ADD  CONSTRAINT [FK_Subscription_SupplierSubscription] FOREIGN KEY([SubscriptionID])
REFERENCES [dbo].[Subscription] ([SubscriptionID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[SupplierSubscription] CHECK CONSTRAINT [FK_Subscription_SupplierSubscription]
GO

ALTER TABLE [dbo].[SupplierSubscription]  WITH CHECK ADD  CONSTRAINT [FK_Supplier_SupplierSubscription] FOREIGN KEY([SupplierID])
REFERENCES [dbo].[Supplier] ([SupplierID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[SupplierSubscription] CHECK CONSTRAINT [FK_Supplier_SupplierSubscription]
GO