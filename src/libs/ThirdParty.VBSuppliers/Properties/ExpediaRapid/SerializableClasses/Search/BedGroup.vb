Imports Newtonsoft.Json
Namespace ExpediaRapid.SerializableClasses.Search

    Public Class BedGroupAvailability

        <JsonProperty("links")>
        Public Property Links As BedGroupAvailabilityLink

        <JsonProperty("id")>
        Public Property BedGroupID As String

        <JsonProperty("description")>
        Public Property Description As String

        <JsonProperty("configuration")>
        Public Property BedConfigurations As New List(Of BedConfiguration)

    End Class

End Namespace
