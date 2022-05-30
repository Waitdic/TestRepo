Imports ThirdParty.Abstractions.Constants
Imports ThirdParty.Abstractions.Lookups

Public Class TravelgateTravellanda
    Inherits Travelgate

    Public Sub New(settings As ITravelgateSettings, support As ITPSupport)
        MyBase.New(settings, support)
    End Sub

    Public Overrides Property Source As String
        Get
            Return ThirdParties.TRAVELGATETRAVELLANDA
        End Get
        Set
        End Set
    End Property

End Class