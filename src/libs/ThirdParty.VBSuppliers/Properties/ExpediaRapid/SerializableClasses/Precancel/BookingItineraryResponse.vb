Imports Newtonsoft.Json.Linq
Imports Intuitive.Net
Imports Newtonsoft.Json
Imports ThirdParty.VBSuppliers.ExpediaRapid.SerializableClasses.Book

Namespace ExpediaRapid.SerializableClasses.BookingItinerary
    Public Class BookingItineraryResponse
        Implements IExpediaRapidResponse

        <JsonProperty("itinerary_id")>
        Public Property ItineraryID As String

        <JsonProperty("property_id")>
        Public Property PropertyID As String

        <JsonProperty("links")>
        Public Property Links As New Dictionary(Of String, Link)

        <JsonProperty("rooms")>
        Public Property Rooms As List(Of BookingItineraryResponseRoom)

        <JsonProperty("billing_contact")>
        Public Property BillingContact As BillingContact

        <JsonProperty("adjustment")>
        Public Property Adjustment As Rate

        <JsonProperty("creation_date_time")>
        Public Property CreationDateTime As Date

        <JsonProperty("affiliate_reference_id")>
        Public Property AffiliateReferenceID As String

        <JsonProperty("affiliate_metadata")>
        Public Property AffiliateMetadata As String

        <JsonProperty("conversations")>
        Public Property Conversations As Conversations

        Public Function IsValid(responseString As String, statusCode As Integer) As Boolean Implements IExpediaRapidResponse.IsValid

            If Not String.IsNullOrWhiteSpace(responseString) Then
                Try

                    Dim precancelResponse As BookingItineraryResponse = JsonConvert.DeserializeObject(Of BookingItineraryResponse)(responseString)

                    If precancelResponse.Rooms.First Is Nothing Then Return False
                    If precancelResponse.Rooms.First.Rate Is Nothing Then Return False

                    Return True
                Catch ex As Exception
                    Return False
                End Try
            End If
            Return False
        End Function

    End Class

End Namespace