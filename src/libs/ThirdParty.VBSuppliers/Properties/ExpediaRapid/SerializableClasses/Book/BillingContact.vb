Imports Newtonsoft.Json

Namespace ExpediaRapid.SerializableClasses.Book

    Public Class BillingContact

        <JsonProperty("given_name")>
        Public Property GivenName As String

        <JsonProperty("family_name")>
        Public Property FamilyName As String

        <JsonProperty("email")>
        Public Property Email As String

        <JsonProperty("phone")>
        Public Property Phone As Phone

        <JsonProperty("address")>
        Public Property Address As Address
    End Class

End Namespace