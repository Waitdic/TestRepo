CREATE PROCEDURE [dbo].[TransferBooking_GetData]
	@supplierBookingReference VARCHAR(400)
as

SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
set nocount on

select TransferBookingID, SupplierName Source, Supplier.SupplierID, DepartureDate, ISOCurrencyID
	from TransferBooking
		inner join Supplier
			on TransferBooking.SupplierID = Supplier.SupplierID
	where TransferBooking.SupplierBookingReference = @supplierBookingReference