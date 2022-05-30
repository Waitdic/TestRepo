Imports Newtonsoft.Json

Namespace ExpediaRapid.SerializableClasses.BookingItinerary
    Public Class ConfirmationID

        <JsonProperty("expedia")>
        Public Property ExpediaConfirmationID As String

        <JsonProperty("property")>
        Public Property PropertyConfirmationID As String

    End Class

End Namespace