Imports ThirdParty.Abstractions.Models

Public Class YouTravelSupport

    Public Shared Function GetDistinctDestinations(ByVal oResorts As List(Of ResortSplit)) As Generic.List(Of String)

        Dim oReturn As New Generic.List(Of String)
        Dim sDestination As String

        For Each oResort As ResortSplit In oResorts

            sDestination = oResort.ResortCode.Split("_"c)(0)
            If Not oReturn.Contains(sDestination) Then
                oReturn.Add(sDestination)
            End If

        Next

        Return oReturn
    End Function

    Public Shared Function FormatDate(ByVal dDate As Date) As String
        Return dDate.ToString("dd/MM/yyyy")
    End Function

End Class
