CREATE TABLE [dbo].[Extra] 
(
    [ExtraID] INT IDENTITY(1,1) NOT NULL,
    [Source] VARCHAR(30) NOT NULL,
    [ExtraName] VARCHAR(200) NOT NULL,
    [Payload] varchar(200) NULL,
    CONSTRAINT [PK_IVOExtraLookup] PRIMARY KEY CLUSTERED ([ExtraID] ASC)
)