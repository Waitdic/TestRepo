Imports ThirdParty.Abstractions.Constants
Imports ThirdParty.Abstractions.Lookups

Public Class TravelgateDingusSpringHotelsSearch
    Inherits TravelgateSearch

    Public Sub New(settings As ITravelgateSettings, support As ITPSupport)
        MyBase.New(settings, support)
    End Sub

    Public Overrides Property Source As String
        Get
            Return ThirdParties.TRAVELGATEDINGUSSPRINGHOTELS
        End Get
        Set
        End Set
    End Property
End Class
