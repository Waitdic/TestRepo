Imports Intuitive.Net

Namespace ExpediaRapid.SerializableClasses.Cancel

    Public Class CancelResponse
        Implements IExpediaRapidResponse

        Public Function IsValid(responseString As String, statusCode As Integer) As Boolean Implements IExpediaRapidResponse.IsValid

            Select Case statusCode
                Case 202
                    Return True
                Case 204
                    Return True
            End Select

            Return False
        End Function
    End Class

End Namespace
