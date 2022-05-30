Imports Intuitive.Net
Imports ThirdParty.Abstractions.Models.Property.Booking
Imports ThirdParty.VBSuppliers.ExpediaRapid.SerializableClasses

Namespace ExpediaRapid

    Public Interface IExpediaRapidAPI
        Function GetDeserializedResponse(Of TResponse As {IExpediaRapidResponse, New})(propertyDetails As PropertyDetails, request As WebRequests.Request) As TResponse
        Function GetResponse(propertyDetails As PropertyDetails, request As WebRequests.Request) As String
    End Interface

End Namespace