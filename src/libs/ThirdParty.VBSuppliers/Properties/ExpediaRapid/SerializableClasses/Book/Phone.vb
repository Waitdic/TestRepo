Imports Newtonsoft.Json
Namespace ExpediaRapid.SerializableClasses.Book

    Public Class Phone

        <JsonProperty("country_code")>
        Public Property CountryCode As String

        <JsonProperty("area_code")>
        Public Property AreaCode As String

        <JsonProperty("number")>
        Public Property Number As String

        Public Sub New()
        End Sub

        Public Sub New(phone As String)
            If String.IsNullOrWhiteSpace(phone) Then Throw New ArgumentException("Phone number not supplied")

            phone = phone.TrimStart({"+"c, "0"c})

            Me.CountryCode = phone.Remove(2)
            phone = phone.TrimStart(Me.CountryCode.ToCharArray)

            Me.AreaCode = phone.Remove(2)
            Me.Number = phone.TrimStart(Me.AreaCode.ToCharArray)

        End Sub

    End Class

End Namespace