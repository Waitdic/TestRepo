CREATE PROCEDURE [dbo].[IVectorOne_ImportProperties]
AS

set transaction isolation level read uncommitted

drop table if exists #iVectorOneProps

create table #iVectorOneProps (CustomerID int, CentralPropertyID int, PropertyID int, Priority int)

insert into #iVectorOneProps
	select Customer.CustomerID,
			PropertyDedupe.CentralPropertyID,
			Property.PropertyID,
			row_number()
				over (partition by PropertyDedupe.CentralPropertyID 
						order by AccountSupplier.Priority, Property.PropertyID desc) [Priority]
		from Customer
			inner join Account
				on Account.CustomerID = Customer.CustomerID
			inner join AccountSupplier
				on AccountSupplier.AccountID = Account.AccountID
			inner join Supplier
				on AccountSupplier.SupplierID = Supplier.SupplierID
			inner join Property
				on Property.Source = Supplier.SupplierName
			inner join PropertyDedupe
				on PropertyDedupe.PropertyID = Property.PropertyID
			left join CustomerSpecificSource
				on CustomerSpecificSource.CustomerID = Customer.CustomerID
					and CustomerSpecificSource.Source = Supplier.SupplierName
		where CustomerSpecificSource.CustomerSpecificSourceID is null 
	union all
	select CustomerSpecificSource.CustomerID,
			PropertyDedupe.CentralPropertyID,
			Property.PropertyID,
			row_number()
				over (partition by PropertyDedupe.CentralPropertyID 
						order by AccountSupplier.Priority, Property.PropertyID desc) [Priority]
		from CustomerSpecificSource
			inner join Account
				on Account.CustomerID = CustomerSpecificSource.CustomerID
			inner join AccountSupplier
				on AccountSupplier.AccountID = Account.AccountID
			inner join Supplier
				on AccountSupplier.SupplierID = Supplier.SupplierID
					and CustomerSpecificSource.Source = Supplier.SupplierName
			inner join Property
				on Property.Source = Supplier.SupplierName
			inner join CustomerSpecificProperty
				on CustomerSpecificProperty.CustomerID = CustomerSpecificSource.CustomerID
					and CustomerSpecificProperty.PropertyID = Property.PropertyID
			inner join PropertyDedupe
				on PropertyDedupe.PropertyID = Property.PropertyID;

merge into IVectorOneProperty target
	using (select CustomerID, CentralPropertyID, max(PropertyID) ContentSourcePropertyID
			from #iVectorOneProps
			where [Priority] = 1
			group by CustomerID, CentralPropertyID) source
		on target.CustomerID = source.CustomerID
			and target.CentralPropertyID = source.CentralPropertyID
	when not matched by target then
		insert (CustomerID, CentralPropertyID, ContentSourcePropertyID)
			values (source.CustomerID, source.CentralPropertyID, source.ContentSourcePropertyID)
	when matched and target.ContentSourcePropertyID <> source.ContentSourcePropertyID then
		update
			set ContentSourcePropertyID = source.ContentSourcePropertyID
	when not matched by source then
		delete;