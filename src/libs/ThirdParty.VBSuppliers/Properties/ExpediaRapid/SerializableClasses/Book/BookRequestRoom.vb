Imports Newtonsoft.Json
Namespace ExpediaRapid.SerializableClasses.Book

    Public Class BookRequestRoom

        <JsonProperty("title")>
        Public Property Title As String

        <JsonProperty("given_name")>
        Public Property GivenName As String

        <JsonProperty("family_name")>
        Public Property FamilyName As String

        <JsonProperty("email")>
        Public Property Email As String

        <JsonProperty("phone")>
        Public Property Phone As Phone

        <JsonProperty("smoking")>
        Public Property Smoking As Boolean

        <JsonProperty("special_request")>
        Public Property SpecialRequest As String

    End Class

End Namespace
