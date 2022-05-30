Imports Newtonsoft.Json
Namespace ExpediaRapid.SerializableClasses.Search

    Public Class Promotions

        <JsonProperty("value_adds")>
        Public Property ValueAdds As New Dictionary(Of String, Promotion)

        <JsonProperty("deal")>
        Public Property Deal As Promotion

    End Class

End Namespace
