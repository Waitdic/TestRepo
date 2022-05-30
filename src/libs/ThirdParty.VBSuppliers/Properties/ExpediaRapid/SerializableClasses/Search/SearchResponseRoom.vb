Imports Newtonsoft.Json
Namespace ExpediaRapid.SerializableClasses.Search
    Public Class SearchResponseRoom

        <JsonProperty("id")>
        Public Property RoomID As String

        <JsonProperty("room_name")>
        Public Property RoomName As String

        <JsonProperty("rates")>
        Public Property Rates As New List(Of RoomRate)

    End Class

End Namespace
