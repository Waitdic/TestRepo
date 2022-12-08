CREATE PROCEDURE [dbo].[SupplierAPILog_Insert]
	@accountId int,
	@supplierId int,
	@type varchar(30),
	@title varchar(200),
	@requestDateTime datetime,
	@responseTime int,
	@successful bit,
	@requestLog nvarchar(max),
	@responseLog nvarchar(max),
	@bookingId int
AS
INSERT INTO [SupplierAPILog]
		([AccountID],
		[SupplierID],
		[Type],
		[Title],
		[RequestDateTime],
		[ResponseTime],
		[Successful],
		[RequestLog],
		[ResponseLog],
		[BookingID])
	select @accountId,
			@supplierId,
			@type,
			@title,
			@requestDateTime,
			@responseTime,
			@successful,
			@requestLog,
			@responseLog,
			@bookingId
