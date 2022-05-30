Imports Newtonsoft.Json
Namespace ExpediaRapid.SerializableClasses.Search

    Public Class BedConfiguration

        <JsonProperty("type")>
        Public Property Type As String

        <JsonProperty("size")>
        Public Property Size As String

        <JsonProperty("quantity")>
        Public Property Quantity As Integer
    End Class

End Namespace
