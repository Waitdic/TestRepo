Imports Newtonsoft.Json.Linq
Imports Intuitive.Net
Imports Newtonsoft.Json

Namespace ExpediaRapid.SerializableClasses.Prebook

    Public Class PrebookResponse
        Implements IExpediaRapidResponse

        <JsonProperty("occupancy_pricing")>
        Public Property OccupancyRoomRates As Dictionary(Of String, OccupancyRoomRate)

        <JsonProperty("links")>
        Public Property Links As New Dictionary(Of String, Link)

        Public Function IsValid(responseString As String, statusCode As Integer) As Boolean Implements IExpediaRapidResponse.IsValid

            If Not String.IsNullOrWhiteSpace(responseString) Then
                Try

                    JsonConvert.DeserializeObject(Of PrebookResponse)(responseString)
                    Return True

                Catch ex As Exception
                    Return False
                End Try
            End If
            Return False
        End Function
    End Class

End Namespace