CREATE PROCEDURE [dbo].[Property_SingleProperty]
	@propertyId int,
	@accountId int = 0
as

SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED 
set nocount on

select Property.PropertyID, PropertyDedupe.CentralPropertyID, Property.Source, Property.TPKey, Property.Name, Geography.Code GeographyCode
	from Property
		inner join PropertyDedupe
			on Property.PropertyID = PropertyDedupe.PropertyID
		inner join Geography
			on Property.GeographyID = Geography.GeographyID
	where Property.PropertyID = @propertyId
union all
select Property.PropertyID, AccountProperty.CentralPropertyID, Property.Source, Property.TPKey, Property.Name, '' GeographyCode
	from Property
		inner join AccountProperty
			on AccountProperty.PropertyID = Property.PropertyID
				and AccountProperty.AccountID = @accountId
				and AccountProperty.PropertyID = @propertyId