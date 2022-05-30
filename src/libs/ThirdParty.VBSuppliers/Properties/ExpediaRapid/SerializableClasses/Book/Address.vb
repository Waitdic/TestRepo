Imports Newtonsoft.Json

Namespace ExpediaRapid.SerializableClasses.Book
    Public Class Address

        <JsonProperty("line_1")>
        Public Property Line1 As String

        <JsonProperty("line_2")>
        Public Property Line2 As String

        <JsonProperty("line_3")>
        Public Property Line3 As String

        <JsonProperty("city")>
        Public Property City As String

        <JsonProperty("state_province_code")>
        Public Property StateProvinceCode As String

        <JsonProperty("postal_code")>
        Public Property PostalCode As String

        <JsonProperty("country_code")>
        Public Property CountryCode As String

    End Class

End Namespace