CREATE PROCEDURE [dbo].[Booking_Get]
	@bookingReference varchar(400),
	@supplierBookingReference varchar(400),
	@accountId int
AS

declare @bookingId int = 0

select @bookingId = Booking.BookingID
	from Booking
	where (Booking.BookingReference = @bookingReference
			or Booking.SupplierBookingReference = @supplierBookingReference)
		and Booking.AccountID = @accountId

select *
	from Booking
	where Booking.BookingID = @bookingId

select *
	from [$(Telemetry)].dbo.APILog
	where BookingID = @bookingId

select *
	from [$(Telemetry)].dbo.SupplierAPILog
	where BookingID = @bookingId