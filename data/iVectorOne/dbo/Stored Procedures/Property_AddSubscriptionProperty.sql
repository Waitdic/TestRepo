CREATE PROCEDURE [dbo].[Property_AddSubscriptionProperty]
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

if exists (select *
			from SubscriptionProperty
				inner join Property
					on SubscriptionProperty.PropertyID = Property.PropertyID
						and Property.Source = @source
						and Property.TPKey = @tpKey
			where SubscriptionProperty.SubscriptionID = @subscriptionId) begin
	return
end

insert into Property
	select @source,
			@tpKey,
			@name,
			0 GeographyID,
			@propertyDetails,
			@checksum,
			@ttiCode,
			0 AddedImportID,
			0 LastImportID,
			0 LastModifiedImportID,
			len(@name),
			@descriptionLength,
			@hasPostcode,
			@hasPhoneNumber,
			case when isnull(@rating, 0) = 0 then 0 else 1 end HasRating,
			0 HasImages,
			@rating

declare @propertyId int = @@identity,
		@centralPropertyId int = 0,
		@centralPropertyIdSeed int = 10000000

select @centralPropertyId = CentralProperty.CentralPropertyID
	from CentralProperty
	where CentralProperty.TTICode = @ttiCode
		and @ttiCode <> ''

if (isnull(@centralPropertyId, 0) = 0) begin

	select @centralPropertyId = isnull(max(SubscriptionProperty.CentralPropertyID) + 1, 0)
		from SubscriptionProperty

	if (@centralPropertyId < @centralPropertyIdSeed) begin
		set @centralPropertyId = @centralPropertyIdSeed + 1
	end

	set identity_insert CentralProperty on
	insert into CentralProperty (CentralPropertyID, TTICode)
		select @centralPropertyId, @ttiCode
	set identity_insert CentralProperty off

end

insert into SubscriptionProperty
	select @subscriptionId, @propertyId, @centralPropertyId