/****** Object:  StoredProcedure [dbo].[Get_SearchesByHour]    Script Date: 26/10/2022 12:09:50 ******/

CREATE PROCEDURE [dbo].[Get_SearchesByHour]
	-- Add the parameters for the stored procedure here
		@AccountID int
		--@Date DATE,
		--@LastWeekDate DATE
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

			declare @Date DATE
	 DECLARE @LastWeekDate DATE

	SELECT @Date = GETDATE();
	SELECT @LastWeekDate = DATEADD(WEEK, -1, GETDATE());

	---- NB All data must be filtered for the logged in customer
	--SELECT @AccountID = @AccountID;
	
	SELECT	CONVERT(varchar(2), DATEPART(hour,SearchDateAndTime), 2) + ':00' AS [YValue]
			, case 
				WHEN DATEPART(week,SearchDateAndTime) = DATEPART(week,@Date) THEN 'Today'
				WHEN DATEPART(week,SearchDateAndTime) = DATEPART(week,@LastWeekDate) THEN 'Last Week'
				end AS [Series]
			, count(*) AS XValue
	FROM [ThirdPartyImport].[dbo].[SearchStoreApi]
	WHERE CONVERT(DATE,SearchDateAndTime) = (CONVERT(DATE,@Date))
			OR CONVERT(DATE,SearchDateAndTime) = (CONVERT(DATE,@LastWeekDate))
			AND [System] = 'Live'
			AND AccountID = @AccountID
	GROUP BY DATEPART(week,SearchDateAndTime), DATEPART(hour,SearchDateAndTime)
	ORDER BY DATEPART(week,SearchDateAndTime), DATEPART(hour,SearchDateAndTime)
END
