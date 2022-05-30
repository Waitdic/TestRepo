Imports Newtonsoft.Json
Namespace ExpediaRapid.SerializableClasses.Book
    Public Class Payment

        <JsonProperty("type")>
        Public Property Type As String

        <JsonProperty("billing_contact")>
        Public Property BillingContact As BillingContact

    End Class

End Namespace