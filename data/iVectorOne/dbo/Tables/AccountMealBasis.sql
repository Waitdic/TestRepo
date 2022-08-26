CREATE TABLE [dbo].[AccountMealBasis](
	[AccountID] [int] NOT NULL,
	[MealBasisCode] [varchar](50) NOT NULL,
	[MealBasis] [varchar](50) NOT NULL,
	[AccountMealBasisID] [int] IDENTITY(1,1) NOT NULL,
	CONSTRAINT [PK_AccountMealBasis] PRIMARY KEY CLUSTERED ([AccountID] ASC, [MealBasisCode] ASC)
) ON [PRIMARY]
GO

