Imports System.Text
Imports System.Text.RegularExpressions
Imports Intuitive.Helpers.Extensions
Imports ThirdParty.VBSuppliers.ExpediaRapid.ExceptionMessages

Namespace ExpediaRapid

    Public Class ExpediaRapidOccupancy

        ''' <remarks>
        ''' This constructor seems to exist only for testing purposes.
        ''' There is no point in testing this if it isn't used.
        ''' </remarks>

        Public Sub New(occupancy As String)

            If String.IsNullOrWhiteSpace(occupancy) Then Throw New ArgumentNullException(NameOf(occupancy))

            Dim matches As GroupCollection =
                    Regex.Match(occupancy, "^(?<adults>\d+)(-?)(?<childrenCsv>\d+(,\d+)*)?$").Groups

            Dim adultsGroup As Group = matches("adults")
            Dim childCsvGroup As Group = matches("childrenCsv")

            If Not adultsGroup.Success Then Throw New ArgumentException(OccupancyExceptions.InvalidOccupancyString)

            Dim adults As Integer = adultsGroup.Value.ToSafeInt()

            If adults = 0 Then Throw New ArgumentException(OccupancyExceptions.NoAdults)

            Me.Adults = adults

            If childCsvGroup.Success Then Me.ChildAges.AddRange(childCsvGroup.Value.Split(","c).Select(AddressOf Integer.Parse))
        End Sub

        Public Sub New(
            adults As Integer,
            childAges As List(Of Integer),
            infants As Integer)

            If adults = 0 Then Throw New ArgumentException(OccupancyExceptions.NoAdults)

            Me.Adults = adults
            If childAges IsNot Nothing Then Me.ChildAges.AddRange(childAges)
            Me.ChildAges.AddRange(Enumerable.Range(0, infants).Select(Function(i) 0))
        End Sub

        Public Property Adults As Integer
        Public Property ChildAges As New List(Of Integer)

        Public Function GetExpediaRapidOccupancy() As String
            Dim occupancyValue As New StringBuilder()
            occupancyValue.Append(Adults)

            If ChildAges.Any() Then occupancyValue.Append("-" + String.Join(",", ChildAges))

            Return occupancyValue.ToString()
        End Function

    End Class

End Namespace