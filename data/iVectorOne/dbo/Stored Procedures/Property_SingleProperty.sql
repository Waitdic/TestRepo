CREATE PROCEDURE [dbo].[Property_SingleProperty]
	@propertyId int,
	@subscriptionId int = 0
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
select Property.PropertyID, SubscriptionProperty.CentralPropertyID, Property.Source, Property.TPKey, Property.Name, '' GeographyCode
	from Property
		inner join SubscriptionProperty
			on SubscriptionProperty.PropertyID = Property.PropertyID
				and SubscriptionProperty.SubscriptionID = @subscriptionId

