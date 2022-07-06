CREATE PROCEDURE [dbo].[Property_List]
	@lastModified datetime,
	@suppliers varchar(max),
	@subscriptionId int = 0
as

SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED 
set nocount on

create table #Suppliers (Supplier varchar(30))

insert into #Suppliers
	exec CSVToTableString @suppliers, ','

IF (COALESCE(@lastModified, '' ) = '')  BEGIN
 set @lastModified = '2000-01-01 00:00:00.000'
END

select distinct CentralProperty.CentralPropertyID
	from CentralProperty
		inner join PropertyDedupe
			on PropertyDedupe.CentralPropertyID = CentralProperty.CentralPropertyID
		inner join Property
			on PropertyDedupe.PropertyID = Property.PropertyID
				and PropertyDedupe.Source = Property.Source
		inner join Import
			on Property.LastModifiedImportID = Import.ImportID
		inner join #Suppliers
			on #Suppliers.supplier = PropertyDedupe.Source
	where Import.Queue_EndDateTime > @lastModified
union all
select SubscriptionProperty.CentralPropertyID
	from SubscriptionProperty
		inner join Property
			on SubscriptionProperty.PropertyID = Property.PropertyID
		inner join #Suppliers
			on #Suppliers.supplier = Property.Source
	where SubscriptionProperty.SubscriptionID = @subscriptionId
	order by 1

drop table #Suppliers