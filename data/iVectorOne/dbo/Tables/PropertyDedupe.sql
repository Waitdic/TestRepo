CREATE TABLE [dbo].[PropertyDedupe](
	[PropertyDedupeID] int IDENTITY(1,1) NOT NULL,
	[CentralPropertyID] int NOT NULL,
	[PropertyID] int NOT NULL,
	[Source] varchar(30) NOT NULL,
 CONSTRAINT [PK_PropertyDedupe] PRIMARY KEY CLUSTERED ([PropertyDedupeID] ASC)
);

Go

CREATE NONCLUSTERED INDEX [Source-PropertyID]
    ON [dbo].[PropertyDedupe]([Source] ASC, [PropertyID] ASC);

GO

CREATE NONCLUSTERED INDEX [fk_Source]
    ON [dbo].[PropertyDedupe]([Source] ASC)
    INCLUDE ([CentralPropertyID], [PropertyID]);

GO

CREATE NONCLUSTERED INDEX [fk_CentralPropertyID]
    ON [dbo].[PropertyDedupe]([CentralPropertyID] ASC)
    INCLUDE ([PropertyID])