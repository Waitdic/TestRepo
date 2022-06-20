CREATE TABLE [dbo].[SupplierSubscriptionAttribute](
	[SupplierSubscriptionAttributeID] [int] IDENTITY(1,1) NOT NULL,
	[SubscriptionID] [int] NOT NULL,
	[SupplierAttributeID] [int] NOT NULL,
	[Value] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_SupplierSubscriptionAttribute] PRIMARY KEY NONCLUSTERED 
(
	[SupplierSubscriptionAttributeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[SupplierSubscriptionAttribute]  WITH CHECK ADD  CONSTRAINT [FK_Subscription_SupplierSubscriptionAttribute] FOREIGN KEY([SubscriptionID])
REFERENCES [dbo].[Subscription] ([SubscriptionID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[SupplierSubscriptionAttribute] CHECK CONSTRAINT [FK_Subscription_SupplierSubscriptionAttribute]
GO

ALTER TABLE [dbo].[SupplierSubscriptionAttribute]  WITH CHECK ADD  CONSTRAINT [FK_SupplierAttribute_SupplierSubscriptionAttribute] FOREIGN KEY([SupplierAttributeID])
REFERENCES [dbo].[SupplierAttribute] ([SupplierAttributeID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[SupplierSubscriptionAttribute] CHECK CONSTRAINT [FK_SupplierAttribute_SupplierSubscriptionAttribute]
GO
