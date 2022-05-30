Imports Intuitive.Net
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Namespace ExpediaRapid.SerializableClasses.Search

    Public Class SearchResponse
        Inherits List(Of PropertyAvailablility)
        Implements IExpediaRapidResponse

        Public Function IsValid(responseString As String, statusCode As Integer) As Boolean Implements IExpediaRapidResponse.IsValid

            If Not String.IsNullOrWhiteSpace(responseString) Then
                Dim token As JToken = JToken.Parse(responseString)
                Try

                    JsonConvert.DeserializeObject(Of SearchResponse)(responseString)
                    Return True

                Catch ex As Exception
                    Return False
                End Try
            End If
            Return False
        End Function
    End Class

End Namespace