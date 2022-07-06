CREATE PROCEDURE [dbo].[Search_GetThirdPartyData]
	@centralPropertyIDs varchar(max),
	@sources varchar(max),
	@subscriptionId int = 0
as

SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED 
set nocount on

-- 1. Build base property table - property criteria based
create table #centralproperty (CentralPropertyID int)

insert into #centralproperty
	exec CSVToTable @centralPropertyIDs

create table #Sources (Source varchar(30))

insert into #Sources
	exec CSVToTableString @sources, ','

select #centralproperty.CentralPropertyID,
		Property.PropertyID,
		Property.Source,
		Property.TPKey,
		Property.Name,
		Geography.Code
	from #centralproperty
		inner join PropertyDedupe
			on PropertyDedupe.CentralPropertyID = #centralproperty.CentralPropertyID
		inner join #Sources
			on #Sources.Source = PropertyDedupe.Source
		inner join Property
			on PropertyDedupe.PropertyID = Property.PropertyID
		inner join Geography
			on Property.GeographyID = Geography.GeographyID
union all
select #centralproperty.CentralPropertyID,
		Property.PropertyID,
		Property.Source,
		Property.TPKey,
		Property.Name,
		'' Code
	from #centralproperty
		inner join SubscriptionProperty
			on SubscriptionProperty.CentralPropertyID = #centralproperty.CentralPropertyID
				and SubscriptionProperty.SubscriptionID = @subscriptionId
		inner join Property
			on SubscriptionProperty.PropertyID = Property.PropertyID
		inner join #Sources
			on #Sources.Source = Property.Source

drop table #centralproperty
drop table #Sources