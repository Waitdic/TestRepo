CREATE PROCEDURE [dbo].[Search_GetThirdPartyData]
    @sCentralPropertyIDs varchar(max),
    @sSources varchar(max)
AS

SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED 
set nocount on

-- 1. Build base property table - property criteria based
create table #centralproperty (CentralPropertyID int)

insert into #centralproperty
	exec CSVToTable @sCentralPropertyIDs

create table #Sources (Source varchar(30))

insert into #Sources
	exec CSVToTableString @sSources, ','

select CentralPropertyID, PropertyID, Source, TPKey, Name, Code	
	from (select #centralproperty.CentralPropertyID, Property.PropertyID, Property.Source, Property.TPKey, Property.Name, Geography.Code,
				row_number() over(partition by property.Source, PropertyDedupe.CentralPropertyID order by Property.LastImportID desc, Property.HasImages desc) Seq
			from #centralproperty
				left join PropertyDedupe
					on PropertyDedupe.CentralPropertyID = #centralproperty.CentralPropertyID
				inner join Property
					on PropertyDedupe.PropertyID = Property.PropertyID
				inner join #Sources
					on #Sources.Source = Property.Source
				inner join Geography
					on Property.GeographyID = Geography.GeographyID) deduped
	where deduped.Seq = 1

drop table #centralproperty
drop table #Sources