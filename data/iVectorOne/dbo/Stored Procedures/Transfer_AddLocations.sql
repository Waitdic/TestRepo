CREATE PROCEDURE [dbo].[Transfer_AddLocations]
	@source varchar(100),
	@locations varchar(max)
AS
BEGIN
    BEGIN TRY
INSERT INTO IVOLocation(Source,Description,Payload) select @source, value, value FROM STRING_SPLIT(@locations,',')
   SELECT 0
    END TRY
    BEGIN CATCH
        SELECT -1
    END CATCH
END
