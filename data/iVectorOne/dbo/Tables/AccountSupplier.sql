CREATE TABLE [dbo].[AccountSupplier](
	[AccountSupplierID] [int] IDENTITY(1,1) NOT NULL,
	[AccountID] [int] NOT NULL,
	[SupplierID] [smallint] NOT NULL,
	[Enabled] BIT NOT NULL DEFAULT 0, 
	[Priority] [int] NOT NULL DEFAULT 1,
	[LogSearchRequests] [bit] NOT NULL DEFAULT 0,
	CONSTRAINT [PK_AccountSupplier] PRIMARY KEY NONCLUSTERED ([AccountSupplierID] ASC),
	CONSTRAINT [UN_AccountIDSupplierID] UNIQUE NONCLUSTERED ([AccountID] ASC, [SupplierID] ASC)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[AccountSupplier] ADD CONSTRAINT [FK_Account_AccountSupplier] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Account] ([AccountID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[AccountSupplier] CHECK CONSTRAINT [FK_Account_AccountSupplier]
GO

ALTER TABLE [dbo].[AccountSupplier] ADD CONSTRAINT [FK_Supplier_AccountSupplier] FOREIGN KEY([SupplierID])
REFERENCES [dbo].[Supplier] ([SupplierID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[AccountSupplier] CHECK CONSTRAINT [FK_Supplier_AccountSupplier]
GO