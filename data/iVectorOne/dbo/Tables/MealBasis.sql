CREATE TABLE [dbo].[MealBasis](
	[MealBasisID] int IDENTITY(1,1) NOT NULL,
	[Source] varchar(30) NOT NULL,
	[MealBasisCode] varchar(50) NOT NULL,
	[MealBasis] varchar(50) NULL,
 CONSTRAINT [PK_MealBasisLookup] PRIMARY KEY CLUSTERED ([MealBasisID] ASC)
);