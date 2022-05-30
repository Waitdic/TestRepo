Imports Newtonsoft.Json

Namespace ExpediaRapid.SerializableClasses.Book
    Public Class BookRequest

        <JsonProperty("affiliate_reference_id")>
        Public Property AffiliateReferenceId As String

        <JsonProperty("hold")>
        Public Property Hold As Boolean

        <JsonProperty("rooms")>
        Public Property Rooms As New List(Of BookRequestRoom)

        <JsonProperty("payments")>
        Public Property Payments As New List(Of Payment)

        <JsonProperty("affiliate_metadata")>
        Public Property AffiliateMetadata As String

    End Class

End Namespace