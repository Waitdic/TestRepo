CREATE PROCEDURE [dbo].[Property_List]
    @LastModified datetime,
    @Suppliers varchar(max)
AS
	
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED 
set nocount on

create table #Suppliers (Supplier varchar(30))

insert into #Suppliers
	exec CSVToTableString @Suppliers, ','

IF (COALESCE(@LastModified, '' ) = '')  BEGIN
 set @LastModified = '2000-01-01 00:00:00.000'
END

select distinct centralproperty.CentralPropertyID
	from centralproperty
		inner join PropertyDedupe
			on PropertyDedupe.CentralPropertyID = centralproperty.CentralPropertyID
		inner join Property
			on PropertyDedupe.PropertyID = Property.PropertyID
				and PropertyDedupe.Source = property.Source
		inner join import
			on property.LastModifiedImportID = import.ImportID
		inner join #Suppliers
			on #Suppliers.supplier = PropertyDedupe.Source
	where 
	(Select coalesce (Import.Queue_EndDateTime, Import.Queue_StartDateTime)) > @LastModified
	order by centralproperty.CentralPropertyID


drop table #Suppliers