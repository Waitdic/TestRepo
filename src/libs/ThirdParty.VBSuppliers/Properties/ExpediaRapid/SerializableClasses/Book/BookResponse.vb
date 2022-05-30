Imports Newtonsoft.Json.Linq
Imports Intuitive.Net
Imports Newtonsoft.Json

Namespace ExpediaRapid.SerializableClasses.Book

    Public Class BookResponse
        Implements IExpediaRapidResponse

        <JsonProperty("itinerary_id")>
        Public Property ItineraryID As String

        <JsonProperty("links")>
        Public Property Links As New Dictionary(Of String, Link)

        Public Function IsValid(responseString As String, statusCode As Integer) As Boolean Implements IExpediaRapidResponse.IsValid

            If Not String.IsNullOrWhiteSpace(responseString) Then
                Try
                    Dim bookResponse As BookResponse = JsonConvert.DeserializeObject(Of BookResponse)(responseString)

                    If String.IsNullOrWhiteSpace(bookResponse.ItineraryID) _
                        OrElse Not bookResponse.Links.ContainsKey("retrieve") Then Return False

                    Return True
                Catch ex As Exception
                    Return False
                End Try
            End If
            Return False
        End Function

    End Class

End Namespace
