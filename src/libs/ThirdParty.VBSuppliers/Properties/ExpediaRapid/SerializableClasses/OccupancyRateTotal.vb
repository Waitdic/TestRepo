Imports Newtonsoft.Json
Namespace ExpediaRapid.SerializableClasses
    Public Class OccupancyRateTotal

        <JsonProperty("billable_currency")>
        Public Property TotalInBillableCurrency As OccupancyRateAmount

        <JsonProperty("request_currency")>
        Public Property TotalInRequestCurrency As OccupancyRateAmount

    End Class
End Namespace
