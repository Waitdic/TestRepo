Imports Newtonsoft.Json
Namespace ExpediaRapid.SerializableClasses

    Public Class Fee

        <JsonProperty("type")>
        Public Property Type As String

        <JsonProperty("value")>
        Public Property Value As Decimal

        <JsonProperty("currency")>
        Public Property Currency As String

        <JsonProperty("scope")>
        Public Property Scope As String

        <JsonProperty("frequency")>
        Public Property Frequency As String

    End Class

End Namespace
