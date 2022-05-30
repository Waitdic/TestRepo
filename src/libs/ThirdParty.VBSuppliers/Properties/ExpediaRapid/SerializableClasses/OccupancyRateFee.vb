Imports Newtonsoft.Json
Namespace ExpediaRapid.SerializableClasses

    Public Class OccupancyRateFee

        <JsonProperty("billable_currency")>
        Public Property TotalInBillableCurrency As OccupancyRateAmount

        <JsonProperty("request_currency")>
        Public Property TotalInRequestCurrency As OccupancyRateAmount

        <JsonProperty("scope")>
        Public Property FeeScope As String

        <JsonProperty("frequency")>
        Public Property FeeFrequency As String

    End Class

End Namespace
