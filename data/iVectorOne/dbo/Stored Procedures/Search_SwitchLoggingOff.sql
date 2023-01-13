CREATE PROCEDURE [dbo].[Search_SwitchLoggingOff]
AS

update AccountSupplier
	set LogSearchRequests = 0
	where LogSearchRequests = 1;