Imports Intuitive.Net

Namespace ExpediaRapid.SerializableClasses

    Public Interface IExpediaRapidResponse

        Function IsValid(responseString As String, statusCode As Integer) As Boolean

    End Interface

End Namespace
