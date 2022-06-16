CREATE PROCEDURE [dbo].[Customer_GetSingleTenantCustomers]
AS

SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
set nocount on

SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
set nocount on

select Customer.CustomerID,
		Customer.Name,
		Customer.BaseUrl,
		Subscription.SubscriptionID,
		string_agg(Supplier.SupplierName, ',') Suppliers,
		isnull(p.PropertyIDs, '') PropertyIDs
	from Customer
		inner join Subscription
			on Subscription.CustomerID = Customer.CustomerID
		inner join SupplierSubscription
			on SupplierSubscription.SubscriptionID = Subscription.SubscriptionID
		inner join Supplier
			on SupplierSubscription.SupplierID = Supplier.SupplierID
				and Supplier.SupplierName in ('Own', 'ChannelManager')
		outer apply
			(select string_agg(convert(varchar(max), Property.TPKey), ',') PropertyIDs
				from SubscriptionProperty
					inner join Property
						on SubscriptionProperty.PropertyID = Property.PropertyID
				where SubscriptionProperty.SubscriptionID = Subscription.SubscriptionID) p
	group by Customer.CustomerID,
		Customer.Name,
		Customer.BaseUrl,
		Subscription.SubscriptionID,
		p.PropertyIDs