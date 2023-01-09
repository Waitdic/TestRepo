CREATE PROCEDURE [dbo].[Transfer_GetLocationMapping]
	@departureLocationID int,
	@arrivalLocationID int,
	@source varchar(30)
AS

SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
set nocount on

SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
set nocount on

declare @departureData varchar(200), @arrivalData varchar(200)

select @departureData = Payload 
	from IVOLocation
	where IVOLocationID = @departureLocationID
		and Source = @source

select @arrivalData = Payload 
	from IVOLocation
	where IVOLocationID = @arrivalLocationID
		and Source = @source

select isnull(@departureData, '') DepartureData, isnull(@arrivalData, '') ArrivalData