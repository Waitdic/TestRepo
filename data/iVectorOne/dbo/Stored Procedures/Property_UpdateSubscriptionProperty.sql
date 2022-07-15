CREATE PROCEDURE [dbo].[Property_UpdateSubscriptionProperty]
	@subscriptionId int,
	@source varchar(30),
	@tpKey varchar(50),
	@name nvarchar(320),
	@propertyDetails nvarchar(max),
	@checksum int,
	@ttiCode varchar(50),
	@descriptionLength bigint,
	@hasPostcode bit,
	@hasPhoneNumber bit,
	@rating decimal(5, 1)
AS

update Property
	set Name = @name,
		PropertyDetails = @propertyDetails,
		Checksum = @checksum,
		TTICode = @ttiCode,
		NameLength = len(@name),
		DescriptionLength = @descriptionLength,
		HasPostcode = @hasPhoneNumber,
		HasPhoneNumber = @hasPostcode,
		HasRating = case when isnull(@rating, 0) = 0 then 0 else 1 end,
		Rating = @rating
	from Property
		inner join SubscriptionProperty
			on SubscriptionProperty.PropertyID = Property.PropertyID
				and SubscriptionProperty.SubscriptionID = @subscriptionId
	where Property.Source = @source
		and Property.TPKey = @tpKey