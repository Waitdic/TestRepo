CREATE PROCEDURE [dbo].[Customer_GetSupplierSpecificCustomer]
	@supplierName varchar(400)
AS

SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
set nocount on

select Customer.CustomerID,
		Customer.Name,
		Customer.BaseUrl,
		Subscription.SubscriptionID,
		isnull(string_agg(convert(varchar(max), Property.TPKey), ','), '') PropertyIDs
	from Customer
		inner join Subscription
			on Subscription.CustomerID = Customer.CustomerID
		inner join SupplierSubscription
			on SupplierSubscription.SubscriptionID = Subscription.SubscriptionID
		inner join Supplier
			on SupplierSubscription.SupplierID = Supplier.SupplierID
				and Supplier.SupplierName = @supplierName
		left join SubscriptionProperty
			on SubscriptionProperty.SubscriptionID = Subscription.SubscriptionID
		left join Property
			on SubscriptionProperty.PropertyID = Property.PropertyID
	group by Customer.CustomerID,
		Customer.Name,
		Customer.BaseUrl,
		Subscription.SubscriptionID