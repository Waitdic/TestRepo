CREATE PROCEDURE [dbo].[TransferBooking_Upsert]
	@bookingReference varchar(400),
	@supplierBookingReference varchar(400),
	@accountId int,
	@supplierId int,
	@status varchar(10),
	@leadGuestName varchar(400),
	@departureDate datetime,
	@totalPrice decimal(14, 2),
	@isoCurrencyId int,
	@estimatedGBPPrice decimal(14, 2)
AS

merge TransferBooking target
	using (select @bookingReference BookingReference,
				@supplierBookingReference SupplierBookingReference,
				@accountId AccountID,
				@supplierId SupplierID,
				@status [Status],
				@leadGuestName LeadGuestName,
				getdate() BookingDateTime,
				@departureDate DepartureDate,
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
			Status,
			LeadGuestName,
			BookingDateTime,
			DepartureDate,
			TotalPrice,
			ISOCurrencyID,
			EstimatedGBPPrice)
		values (source.BookingReference,
				source.SupplierBookingReference,
				source.AccountID,
				source.SupplierID,
				source.Status,
				source.LeadGuestName,
				source.BookingDateTime,
				source.DepartureDate,
				source.TotalPrice,
				source.ISOCurrencyID,
				source.EstimatedGBPPrice)
	
	when matched then
		update
			set SupplierBookingReference = source.SupplierBookingReference,
				AccountID = source.AccountID,
				SupplierID = source.SupplierID,
				Status = source.Status,
				LeadGuestName = source.LeadGuestName,
				DepartureDate = source.DepartureDate,
				TotalPrice = source.TotalPrice,
				ISOCurrencyID = source.ISOCurrencyID,
				EstimatedGBPPrice = source.EstimatedGBPPrice;

select TransferBookingID
	from TransferBooking
	where BookingReference = @bookingReference;