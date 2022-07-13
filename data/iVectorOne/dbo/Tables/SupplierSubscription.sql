CREATE TABLE [dbo].[SupplierSubscription](
	[SupplierSubscriptionID] [int] IDENTITY(1,1) NOT NULL,
	[SupplierID] [smallint] NOT NULL,
	[SubscriptionID] [int] NOT NULL,
	[Enabled] BIT NOT NULL DEFAULT 0, 
	[Priority] [int] NOT NULL DEFAULT(1),
	CONSTRAINT [PK_SupplierSubscription_1] PRIMARY KEY NONCLUSTERED ([SupplierSubscriptionID] ASC)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[SupplierSubscription] ADD CONSTRAINT [FK_Subscription_SupplierSubscription] FOREIGN KEY([SubscriptionID])
REFERENCES [dbo].[Subscription] ([SubscriptionID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[SupplierSubscription] CHECK CONSTRAINT [FK_Subscription_SupplierSubscription]
GO

ALTER TABLE [dbo].[SupplierSubscription] ADD CONSTRAINT [FK_Supplier_SupplierSubscription] FOREIGN KEY([SupplierID])
REFERENCES [dbo].[Supplier] ([SupplierID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[SupplierSubscription] CHECK CONSTRAINT [FK_Supplier_SupplierSubscription]
GO