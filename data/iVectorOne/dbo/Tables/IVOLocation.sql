CREATE TABLE [dbo].[IVOLocation]
(
	[IVOLocationID] int IDENTITY(1,1) NOT NULL,
	[Source] varchar(30) NOT NULL,
	[Description] varchar(30) NOT NULL,
	[Payload] varchar(200) NULL,
	CONSTRAINT [PK_IVOLocationLookup] PRIMARY KEY CLUSTERED ([IVOLocationID] ASC)
)
