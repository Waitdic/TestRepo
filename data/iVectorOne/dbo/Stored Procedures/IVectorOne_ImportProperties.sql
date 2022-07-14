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
						order by SupplierSubscription.Priority, Property.PropertyID desc) [Priority]
		from Customer
			inner join Subscription
				on Subscription.CustomerID = Customer.CustomerID
			inner join SupplierSubscription
				on SupplierSubscription.SubscriptionID = Subscription.SubscriptionID
			inner join Supplier
				on SupplierSubscription.SupplierID = Supplier.SupplierID
			inner join Property
				on Property.Source = Supplier.SupplierName
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