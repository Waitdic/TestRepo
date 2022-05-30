Imports Newtonsoft.Json
Namespace ExpediaRapid.SerializableClasses.Search
    Public Class CancelRefund

        <JsonProperty("amount")>
        Public Property Amount As Decimal

        <JsonProperty("currency")>
        Public Property CurrencyCode As String

    End Class
End Namespace
