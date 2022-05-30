Imports Newtonsoft.Json
Namespace ExpediaRapid.SerializableClasses

    Public Class OccupancyRoomRate

        <JsonProperty("nightly")>
        Public Property NightlyRates As New List(Of List(Of Rate))

        <JsonProperty("stay")>
        Public Property StayRates As New List(Of Rate)

        <JsonProperty("totals")>
        Public Property OccupancyRateTotals As New Dictionary(Of String, OccupancyRateTotal)

        <JsonProperty("fees")>
        Public Property OccupancyRateFees As New Dictionary(Of String, OccupancyRateFee)

    End Class

End Namespace
