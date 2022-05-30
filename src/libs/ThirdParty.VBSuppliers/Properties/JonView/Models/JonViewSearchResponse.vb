Imports System.Xml.Serialization

<Serializable(), XmlRoot("message")>
Public Class JonViewSearchResponse
    <XmlElement("alternatelistseg")> Public Property Response As ResponseDetails

    <Serializable(), XmlType("alternatelistseg")>
    Public Class ResponseDetails
        <XmlElement("listrecord")> Public Property Rooms As New List(Of Room)
    End Class

    <Serializable()>
    Public Class Room
        Public Property prodcode As String
        Public Property status As String
        Public Property productname As String
        Public Property currencycode As String
        Public Property dayprice As String
        Public Property suppliercode As String
        <XmlElement("productnamedetails")> Public Property roomDetails As New Details
        <XmlElement("cancellationpolicy")> Public Property cancellationPolicy As CancellationPolicy
    End Class

    <Serializable()>
    Public Class Details
        Public Property board As String
        Public Property roomtype As String
    End Class

    Public Class CancellationPolicy
        <XmlElement("canpolicyitem")> Public Property item As New CancellationDetails
    End Class

    Public Class CancellationDetails
        Public Property fromdays As String
    End Class

End Class
