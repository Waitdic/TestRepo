Imports System.Net.Http
Imports System.Text
Imports Intuitive.Net
Imports Newtonsoft.Json
Imports ThirdParty.Abstractions.Constants
Imports ThirdParty.Abstractions.Models.Property.Booking
Imports ThirdParty.VBSuppliers.ExpediaRapid.SerializableClasses

Namespace ExpediaRapid
    Public Class ExpediaRapidAPI
        Implements IExpediaRapidAPI

        Private ReadOnly _httpclient As HttpClient

        Public Sub New(httpClient As HttpClient)
            _httpclient = httpClient
        End Sub

        Public Function GetDeserializedResponse(Of TResponse As {IExpediaRapidResponse, New})(propertyDetails As PropertyDetails, request As WebRequests.Request) As TResponse Implements IExpediaRapidAPI.GetDeserializedResponse

            Dim responseString As String = GetResponse(propertyDetails, request)
            Dim response As New TResponse

            If response.IsValid(request.ResponseString,
                                CInt(request.HTTPWebResponse.StatusCode)) Then

                Return JsonConvert.DeserializeObject(Of TResponse)(request.ResponseString)
            End If

            Return Nothing
        End Function

        Public Function GetResponse(propertyDetails As PropertyDetails, request As WebRequests.Request) As String Implements IExpediaRapidAPI.GetResponse
            request.Send(_httpclient)

            propertyDetails.Logs.AddNew(ThirdParties.EXPEDIARAPID, $"{request.LogFileName} Request", request.RequestString)
            propertyDetails.Logs.AddNew(ThirdParties.EXPEDIARAPID, $"{request.LogFileName} Response", request.ResponseString)

            Return request.ResponseString
        End Function

    End Class

End Namespace