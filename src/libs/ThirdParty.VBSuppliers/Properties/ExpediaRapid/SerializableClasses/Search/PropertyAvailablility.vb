Imports Newtonsoft.Json
Namespace ExpediaRapid.SerializableClasses.Search
    Public Class PropertyAvailablility

        <JsonProperty("property_id")>
        Public Property PropertyID As String

        <JsonProperty("rooms")>
        Public Property Rooms As New List(Of SearchResponseRoom)

        <JsonProperty("links")>
        Public Property AvailabilityLinks As New Dictionary(Of String, Link)

        <JsonProperty("score")>
        Public Property Score As Integer

    End Class
End Namespace