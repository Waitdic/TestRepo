CREATE TABLE [dbo].[SupplierAttribute](
	[SupplierAttributeID] [int] IDENTITY(1,1) NOT NULL,
	[SupplierID] [smallint] NOT NULL,
	[AttributeID] [int] NOT NULL,
	CONSTRAINT [PK_SupplierAttribute] PRIMARY KEY NONCLUSTERED ([SupplierAttributeID] ASC)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[SupplierAttribute]  WITH CHECK ADD  CONSTRAINT [FK_Attribute_SupplierAttribute] FOREIGN KEY([AttributeID])
REFERENCES [dbo].[Attribute] ([AttributeID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[SupplierAttribute] CHECK CONSTRAINT [FK_Attribute_SupplierAttribute]
GO

ALTER TABLE [dbo].[SupplierAttribute]  WITH CHECK ADD  CONSTRAINT [FK_Supplier_SupplierAttribute] FOREIGN KEY([SupplierID])
REFERENCES [dbo].[Supplier] ([SupplierID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[SupplierAttribute] CHECK CONSTRAINT [FK_Supplier_SupplierAttribute]
GO