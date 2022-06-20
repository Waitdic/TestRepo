CREATE PROCEDURE [dbo].[Property_SingleProperty]
    @PropertyID int
AS

SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED 
set nocount on

select   Property.PropertyID, PropertyDedupe.CentralPropertyID, Property.Source, Property.TPKey
	from Property
		inner join PropertyDedupe
            on Property.PropertyID = PropertyDedupe.PropertyID

where Property.PropertyID = @PropertyID