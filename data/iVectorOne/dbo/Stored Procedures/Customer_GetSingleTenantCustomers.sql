CREATE PROCEDURE [dbo].[Customer_GetSingleTenantCustomers]
AS

SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
set nocount on

SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
set nocount on

select Customer.CustomerID,
		Customer.Name,
		Customer.BaseUrl,
		Account.AccountID,
		string_agg(Supplier.SupplierName, ',') Suppliers,
		isnull(p.PropertyIDs, '') PropertyIDs
	from Customer
		inner join Account
			on Account.CustomerID = Customer.CustomerID
		inner join AccountSupplier
			on AccountSupplier.AccountID = Account.AccountID
		inner join Supplier
			on AccountSupplier.SupplierID = Supplier.SupplierID
				and Supplier.SupplierName in ('Own', 'ChannelManager')
		outer apply
			(select string_agg(convert(varchar(max), Property.TPKey), ',') PropertyIDs
				from AccountProperty
					inner join Property
						on AccountProperty.PropertyID = Property.PropertyID
				where AccountProperty.AccountID = Account.AccountID) p
	group by Customer.CustomerID,
		Customer.Name,
		Customer.BaseUrl,
		Account.AccountID,
		p.PropertyIDs