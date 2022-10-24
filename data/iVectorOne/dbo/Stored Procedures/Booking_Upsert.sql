CREATE PROCEDURE [dbo].[Booking_Upsert]
	@bookingReference varchar(400),
	@supplierBookingReference varchar(400),
	@accountId int,
	@supplierId int,
	@propertyId int,
	@status varchar(10),
	@leadGuestName varchar(400),
	@departureDate datetime,
	@duration int,
	@totalPrice decimal(14, 2),
	@isoCurrencyId int,
	@estimatedGBPPrice decimal(14, 2)
AS

merge Booking target
	using (select @bookingReference BookingReference,
				@supplierBookingReference SupplierBookingReference,
				@accountId AccountID,
				@supplierId SupplierID,
				@propertyId PropertyID,
				@status [Status],
				@leadGuestName LeadGuestName,
				getdate() BookingDateTime,
				@departureDate DepartureDate,
				@duration Duration,
				@totalPrice TotalPrice,
				@isoCurrencyId ISOCurrencyID,
				@estimatedGBPPrice EstimatedGBPPrice) source
		on target.BookingReference = source.BookingReference
			and target.AccountID = source.AccountID

	when not matched by target then
		insert (BookingReference,
			SupplierBookingReference,
			AccountID,
			SupplierID,
			PropertyID,
			Status,
			LeadGuestName,
			BookingDateTime,
			DepartureDate,
			Duration,
			TotalPrice,
			ISOCurrencyID,
			EstimatedGBPPrice)
		values (source.BookingReference,
				source.SupplierBookingReference,
				source.AccountID,
				source.SupplierID,
				source.PropertyID,
				source.Status,
				source.LeadGuestName,
				source.BookingDateTime,
				source.DepartureDate,
				source.Duration,
				source.TotalPrice,
				source.ISOCurrencyID,
				source.EstimatedGBPPrice)
	
	when matched then
		update
			set SupplierBookingReference = source.SupplierBookingReference,
				AccountID = source.AccountID,
				SupplierID = source.SupplierID,
				PropertyID = source.PropertyID,
				Status = source.Status,
				LeadGuestName = source.LeadGuestName,
				DepartureDate = source.DepartureDate,
				Duration = source.Duration,
				TotalPrice = source.TotalPrice,
				ISOCurrencyID = source.ISOCurrencyID,
				EstimatedGBPPrice = source.EstimatedGBPPrice;

select BookingID
	from Booking
	where BookingReference = @bookingReference;