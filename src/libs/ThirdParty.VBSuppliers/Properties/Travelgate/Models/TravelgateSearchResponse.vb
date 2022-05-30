Imports System.Xml.Serialization

<Serializable(), XmlRoot("Envelope")>
Public Class TravelgateResponseEnvelope
    <XmlElement("Body")> Public Property Body As TravelgateSearchEncodedResponse
End Class

<Serializable(), XmlType("Body")>
Public Class TravelgateSearchEncodedResponse
    <XmlElement("AvailResponse")> Public Property Response As ResponseDetails

    Public Class ResponseDetails
        <XmlElement("AvailResult")> Public Property Result As ProviderRSs
    End Class

    Public Class ProviderRSs
        <XmlElement("providerRSs")> Public Property ProviderResults As New List(Of ProviderResultsDetails)
    End Class

    Public Class ProviderResultsDetails
        <XmlElement("ProviderRS")> Public Property Results As ResultDetails
    End Class

    Public Class ResultDetails
        <XmlElement("rs")> Public Property Result As String
    End Class

End Class

<Serializable(), XmlRoot("AvailRS")>
Public Class TravelgateSearchResponse
    Public Property Hotels As New List(Of Hotel)
    <XmlArray("DailyRatePlans")>
    <XmlArrayItem("DailyRatePlan")>
    Public Property DailyRatePlans As New List(Of RatePlan)

    Public Class Hotel
        <XmlAttribute("code")> Public Property TPKey As String
        <XmlAttribute("name")> Public Property Name As String
        <XmlArray("MealPlans")>
        <XmlArrayItem("MealPlan")>
        Public Property Meals As New List(Of Meal)
    End Class

    Public Class Meal
        <XmlAttribute("code")> Public Property MealBaisCode As String
        <XmlArray("Options")>
        <XmlArrayItem("Option")>
        Public Property Options As New List(Of OptionDetails)
    End Class

    Public Class OptionDetails
        <XmlAttribute("type")> Public Property Type As String
        <XmlAttribute("paymentType")> Public Property PaymentType As String
        <XmlAttribute("status")> Public Property Status As String
        Public Property Rooms As New List(Of Room)
        <XmlElement("Price")> Public Property Price As PriceDetails
        Public Property Parameters As New List(Of Parameter)
        Public Property RateRules As New List(Of RateRule)
    End Class

    Public Class Room
        <XmlAttribute("id")> Public Property ID As String
        <XmlAttribute("code")> Public Property RoomTypeCode As String
        <XmlAttribute("description")> Public Property RoomType As String
        <XmlAttribute("roomCandidateRefId")> Public Property PropertyRoomBookingID As String
        <XmlAttribute("nonRefundable")> Public Property NonRefunfable As String = String.Empty
    End Class

    Public Class PriceDetails
        <XmlAttribute("currency")> Public Property Currency As String
        <XmlAttribute("amount")> Public Property Amount As String
        <XmlAttribute("binding")> Public Property Binding As String
        <XmlAttribute("commission")> Public Property Commission As String
    End Class

    Public Class Parameter
        <XmlAttribute("key")> Public Property Key As String
        <XmlAttribute("value")> Public Property Value As String
    End Class

    Public Class RateRule
        <XmlElement("Rules")> Public Property Rules As New List(Of Rule)
    End Class

    Public Class Rule
        <XmlAttribute("type")> Public Property RateType As String
    End Class

    Public Class RatePlan
        <XmlAttribute("code")> Public Property TPRateCode As String = String.Empty
    End Class
End Class

