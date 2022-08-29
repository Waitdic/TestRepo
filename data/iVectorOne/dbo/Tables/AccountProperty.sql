CREATE TABLE [dbo].[AccountProperty](
	[AccountID] [int] NOT NULL,
	[PropertyID] [int] NOT NULL,
	[CentralPropertyID] [int] NOT NULL,
	CONSTRAINT [PK_AccountProperty] PRIMARY KEY CLUSTERED ([AccountID] ASC, [PropertyID] ASC)
) ON [PRIMARY]
GO