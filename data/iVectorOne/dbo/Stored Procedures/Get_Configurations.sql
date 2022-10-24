CREATE PROCEDURE [dbo].[Get_Configurations]
AS
select (
	select AccountID, Login,
			Password,
			Environment,
			DummyResponses,
			PropertyTPRequestLimit As 'TPSettings.PropertyTPRequestLimit',
			SearchTimeoutSeconds As'TPSettings.SearchTimeoutSeconds',
			LogMainSearchError  As 'TPSettings.LogMainSearchError',
			CurrencyCode  As 'TPSettings.CurrencyCode',
			EncryptedPassword,

			(select SupplierName As 'Supplier',
				(select Attribute.AttributeName, isnull(AccountSupplierAttribute.Value, Attribute.DefaultValue) AttributeValue
					from SupplierAttribute
						inner join Attribute
							on SupplierAttribute.AttributeID = Attribute.AttributeID
						left join AccountSupplierAttribute
							on AccountSupplierAttribute.SupplierAttributeID = SupplierAttribute.SupplierAttributeID
								and AccountSupplierAttribute.AccountID = Account.AccountID
					where SupplierAttribute.SupplierID = Supplier.SupplierID
					for json path) Attributes
				from AccountSupplier
					inner join Supplier
						on Supplier.SupplierID = AccountSupplier.SupplierID
				where AccountSupplier.AccountID = Account.AccountID
				for json path) As 'Configurations'

		from Account
		for json path) Config