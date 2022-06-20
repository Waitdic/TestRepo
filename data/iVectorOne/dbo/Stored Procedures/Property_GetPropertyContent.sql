CREATE PROCEDURE [dbo].[Property_GetPropertyContent]
    @sCentralPropertyIDs varchar(max),
    @Suppliers varchar(max)
AS

SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED 
set nocount on

create table #centralproperty (CentralPropertyID int)

insert into #centralproperty
	exec CSVToTable @sCentralPropertyIDs

create table #Suppliers (Supplier varchar(30))

insert into #Suppliers
	exec CSVToTableString @Suppliers, ','


select Name, TTICode, Source, PropertyDetails, CentralPropertyID, Country, Region, Resort
	from (select Property.Name, Property.TTICode,  Property.Source, Property.PropertyDetails, #centralproperty.CentralPropertyID, Geography.Country, Geography.Region, Geography.Resort,
				row_number() over(partition by property.Source, PropertyDedupe.CentralPropertyID order by Property.LastImportID desc, Property.HasImages desc) Seq
			from Property
				inner join PropertyDedupe
					on PropertyDedupe.PropertyID = Property.PropertyID
				inner join #centralproperty
					on #centralproperty.CentralPropertyID = PropertyDedupe.CentralPropertyID
				inner join  #Suppliers
					on #Suppliers.Supplier = Property.Source
				inner join Geography
					on Property.GeographyID = Geography.GeographyID) deduped
	where deduped.Seq = 1

drop table #centralproperty
drop table #Suppliers