Imports Newtonsoft.Json

Namespace ExpediaRapid.SerializableClasses.BookingItinerary
    Public Class CancelRefund

        <JsonProperty("amount")>
        Public Property Amount As String

        <JsonProperty("currency")>
        Public Property CurrencyCode As String

    End Class

End Namespace