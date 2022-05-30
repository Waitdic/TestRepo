Imports Newtonsoft.Json
Namespace ExpediaRapid.SerializableClasses
    Public Class Rate


        <JsonProperty("type")>
        Public Property RateType As String

        <JsonProperty("value")>
        Public Property Amount As Decimal

        <JsonProperty("currency")>
        Public Property CurrencyCode As String


    End Class
End Namespace
