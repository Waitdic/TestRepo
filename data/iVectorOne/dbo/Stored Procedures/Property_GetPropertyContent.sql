CREATE PROCEDURE [dbo].[Property_GetPropertyContent]
	@centralPropertyIds varchar(max),
	@suppliers varchar(max),
	@subscriptionId int = 0
as

SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
set nocount on

create table #centralproperty (CentralPropertyID int)

insert into #centralproperty
	exec CSVToTable @centralPropertyIds

create table #Suppliers (Supplier varchar(30))

insert into #Suppliers
	exec CSVToTableString @suppliers, ','

select Property.Name,
		Property.TTICode,
		Property.Source,
		Property.PropertyDetails,
		#centralproperty.CentralPropertyID,
		Geography.Country,
		Geography.Region,
		Geography.Resort
	from Property
		inner join PropertyDedupe
			on PropertyDedupe.PropertyID = Property.PropertyID
		inner join #centralproperty
			on #centralproperty.CentralPropertyID = PropertyDedupe.CentralPropertyID
		inner join #Suppliers
			on #Suppliers.Supplier = Property.Source
		inner join Geography
			on Property.GeographyID = Geography.GeographyID
union all
select Property.Name,
		Property.TTICode,
		Property.Source,
		Property.PropertyDetails,
		#centralproperty.CentralPropertyID,
		'' Country,
		'' Region,
		'' Resort
	from Property
		inner join SubscriptionProperty
			on SubscriptionProperty.PropertyID = Property.PropertyID
				and SubscriptionProperty.SubscriptionID = @subscriptionId
		inner join #centralproperty
			on #centralproperty.CentralPropertyID = SubscriptionProperty.CentralPropertyID
		inner join #Suppliers
			on #Suppliers.Supplier = Property.Source

drop table #centralproperty
drop table #Suppliers