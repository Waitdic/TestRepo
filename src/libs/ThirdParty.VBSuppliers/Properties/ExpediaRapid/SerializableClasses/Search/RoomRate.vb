Imports Newtonsoft.Json
Namespace ExpediaRapid.SerializableClasses.Search

    Public Class RoomRate

        <JsonProperty("id")>
        Public Property RateID As String

        <JsonProperty("available_rooms")>
        Public Property AvailableRooms As Integer

        <JsonProperty("refundable")>
        Public Property IsRefundable As Boolean

        <JsonProperty("fenced_deal")>
        Public Property IsFencedDeal As Boolean

        <JsonProperty("fenced_deal_available")>
        Public Property HasFencedDealAvailable As Boolean

        <JsonProperty("deposit_required")>
        Public Property DepositRequired As Boolean

        <JsonProperty("merchant_of_record")>
        Public Property MerchantOfRecord As String

        <JsonProperty("amenities")>
        Public Property Amenities As New Dictionary(Of String, Amenities)

        <JsonProperty("links")>
        Public Property RateLinks As New Dictionary(Of String, Link)

        <JsonProperty("bed_groups")>
        Public Property BedGroupAvailabilities As New Dictionary(Of String, BedGroupAvailability)

        <JsonProperty("cancel_penalties")>
        Public Property CancelPenalities As New List(Of CancelPenalty)

        <JsonProperty("occupancy_pricing")>
        Public Property OccupancyRoomRates As New Dictionary(Of String, OccupancyRoomRate)

        <JsonProperty("promotions")>
        Public Property Promotions As Promotions

    End Class

End Namespace
