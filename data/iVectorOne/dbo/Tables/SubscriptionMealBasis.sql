CREATE TABLE [dbo].[SubscriptionMealBasis](
	[SubscriptionID] [int] NOT NULL,
	[MealBasisCode] [varchar](50) NOT NULL,
	[MealBasis] [varchar](50) NOT NULL,
	[SubscriptionMealBasisID] [int] IDENTITY(1,1) NOT NULL,
	CONSTRAINT [PK_SubscriptionMealBasis] PRIMARY KEY CLUSTERED ([SubscriptionID] ASC, [MealBasisCode] ASC)
) ON [PRIMARY]
GO

