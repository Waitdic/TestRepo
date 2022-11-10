CREATE PROCEDURE [dbo].[APILog_Insert]
	@logType varchar(30),
	@time datetime,
	@requestLog varchar(max),
	@responseLog varchar(max),
	@accountId int,
	@success bit,
	@bookingId int
AS

insert into APILog (Type, Time, RequestLog, ResponseLog, AccountID, Success, BookingID)
	select @logType, @time, @requestLog, @responseLog, @accountId, @success, @bookingId