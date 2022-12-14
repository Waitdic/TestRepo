CREATE PROCEDURE [dbo].[Transfer_AddLocations]
	@source varchar(100),
	@locations varchar(max)
AS
BEGIN

	;with newLocations as (
		select value as [Location]
		from string_split(@locations,',')
	)
	
	insert into IVOLocation(Source, [Description], Payload)
	select @source, SUBSTRING([Location], 0, 30), [Location] 
		from newLocations
		where [Location] not in (select Payload from IVOLocation where Source = @source)
	
END
