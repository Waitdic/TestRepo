CREATE TABLE [dbo].[AccountSupplierAttribute](
	[AccountSupplierAttributeID] [int] IDENTITY(1,1) NOT NULL,
	[AccountID] [int] NOT NULL,
	[SupplierAttributeID] [int] NOT NULL,
	[Value] [nvarchar](max) NOT NULL,
	CONSTRAINT [PK_AccountSupplierAttribute] PRIMARY KEY NONCLUSTERED ([AccountSupplierAttributeID] ASC),
	CONSTRAINT [UN_AccountIDSupplierAttributeID] UNIQUE NONCLUSTERED ([AccountID] ASC, [SupplierAttributeID] ASC)
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[AccountSupplierAttribute] WITH CHECK ADD CONSTRAINT [FK_Account_AccountSupplierAttribute] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Account] ([AccountID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[AccountSupplierAttribute] CHECK CONSTRAINT [FK_Account_AccountSupplierAttribute]
GO

ALTER TABLE [dbo].[AccountSupplierAttribute] WITH CHECK ADD CONSTRAINT [FK_SupplierAttribute_AccountSupplierAttribute] FOREIGN KEY([SupplierAttributeID])
REFERENCES [dbo].[SupplierAttribute] ([SupplierAttributeID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[AccountSupplierAttribute] CHECK CONSTRAINT [FK_SupplierAttribute_AccountSupplierAttribute]
GO
