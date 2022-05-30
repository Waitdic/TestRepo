Imports Newtonsoft.Json
Imports ThirdParty.VBSuppliers.ExpediaRapid.SerializableClasses.Search

Namespace ExpediaRapid.SerializableClasses.BookingItinerary
    Public Class BookingItineraryResponseRoomRate

        <JsonProperty("id")>
        Public Property RateID As String

        <JsonProperty("refundable")>
        Public Property IsRefundable As Boolean

        <JsonProperty("cancel_refund")>
        Public Property CancelRefund As CancelRefund

        <JsonProperty("merchant_of_record")>
        Public Property MerchantOfRecord As String

        <JsonProperty("amenities")>
        Public Property Amenities As New List(Of String)

        <JsonProperty("links")>
        Public Property RateLinks As New Dictionary(Of String, Link)

        <JsonProperty("cancel_penalties")>
        Public Property CancelPenalities As New List(Of CancelPenalty)

        <JsonProperty("deposit_policies")>
        Public Property DepositPolicies As New List(Of DepositPolicy)

        <JsonProperty("nightly")>
        Public Property NightlyRates As New List(Of List(Of Rate))

        <JsonProperty("stay")>
        Public Property StayRates As New List(Of Rate)

        <JsonProperty("fees")>
        Public Property OccupancyRateFees As New List(Of Fee)

        <JsonProperty("promotions")>
        Public Property Promotions As Promotions

    End Class
End Namespace
