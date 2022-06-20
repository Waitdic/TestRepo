CREATE PROCEDURE [dbo].[Get_TPMealBasisCache]
AS

 WITH added_row_number AS (
   SELECT
    *,
	ROW_NUMBER() OVER(PARTITION BY MealBasisCode, Source ORDER BY MealBasisID DESC) AS row_number
	From  MealBasis
 )

SELECT
  *
FROM added_row_number
WHERE row_number = 1;