Imports Newtonsoft.Json
Namespace ExpediaRapid.SerializableClasses

    Public Class Link

        <JsonProperty("method")>
        Public Property Method As String

        <JsonProperty("href")>
        Public Property HRef As String

    End Class

End Namespace