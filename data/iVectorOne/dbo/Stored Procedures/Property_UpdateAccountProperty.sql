CREATE PROCEDURE [dbo].[Property_UpdateAccountProperty]
	@accountId int,
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
		inner join AccountProperty
			on AccountProperty.PropertyID = Property.PropertyID
				and AccountProperty.AccountID = @accountId
	where Property.Source = @source
		and Property.TPKey = @tpKey