Imports Newtonsoft.Json

Namespace ExpediaRapid.SerializableClasses.BookingItinerary
    Public Class Conversations

        <JsonProperty("links")>
        Public Property Links As Dictionary(Of String, Link)

    End Class

End Namespace