Imports Newtonsoft.Json
Imports ThirdParty.VBSuppliers.ExpediaRapid.SerializableClasses.Book

Namespace ExpediaRapid.SerializableClasses.BookingItinerary
    Public Class BookingItineraryResponseRoom
        Inherits BookRequestRoom

        <JsonProperty("id")>
        Public Property RoomID As String

        <JsonProperty("confirmation_id")>
        Public Property ConfirmationID As ConfirmationID

        <JsonProperty("bed_group_id")>
        Public Property BedGroupID As String

        <JsonProperty("checkin")>
        Public Property CheckIn As Date

        <JsonProperty("checkout")>
        Public Property CheckOut As Date

        <JsonProperty("number_of_adults")>
        Public Property NumberOfAdults As Integer

        <JsonProperty("child_ages")>
        Public Property ChildAges As New List(Of Integer)

        <JsonProperty("status")>
        Public Property Status As String

        <JsonProperty("rate")>
        Public Property Rate As BookingItineraryResponseRoomRate

        <JsonProperty("links")>
        Public Property Links As New Dictionary(Of String, Link)

    End Class

End Namespace
