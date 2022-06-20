CREATE TABLE [dbo].[Tenant](
	[TenantID] [int] IDENTITY(1,1) NOT NULL,
	[CompanyName] [nvarchar](100) NOT NULL,
	[ContactName] [nvarchar](100) NULL,
	[ContactTelephone] [varchar](50) NULL,
	[ContactEmail] [nvarchar](200) NULL,
	[Status] [varchar](8) NOT NULL,
 CONSTRAINT [PK_Tenant] PRIMARY KEY CLUSTERED 
(
	[TenantID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [CK_Unique_CompanyName] UNIQUE NONCLUSTERED 
(
	[CompanyName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
