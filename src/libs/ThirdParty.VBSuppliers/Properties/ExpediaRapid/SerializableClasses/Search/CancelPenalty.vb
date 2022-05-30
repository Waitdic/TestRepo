Imports Newtonsoft.Json
Namespace ExpediaRapid.SerializableClasses.Search

    Public Class CancelPenalty

        <JsonProperty("currency")>
        Public Property CurrencyCode As String

        <JsonProperty("start")>
        Public Property CancelStartDate As Date

        <JsonProperty("end")>
        Public Property CancelEndDate As Date

        <JsonProperty("amount")>
        Public Property Amount As Decimal

        <JsonProperty("nights")>
        Public Property Nights As Integer

        <JsonProperty("percent")>
        Public Property Percent As String

    End Class

End Namespace
