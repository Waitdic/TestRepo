CREATE PROCEDURE [dbo].[Transfer_AddNewLocations]
	@source varchar(30),
	@locations varchar(max)
AS

select StringValue LocationDescription
	into #tempLocation
	from CSVToStringTable(@locations)
	group by StringValue

insert into IVOLocation
select @source, LocationDescription, '' 
	from #tempLocation
	where not exists (select * from IVOLocation where Description = LocationDescription and Source = @source)

declare @count int = @@ROWCOUNT

select @count

drop table #tempLocation
