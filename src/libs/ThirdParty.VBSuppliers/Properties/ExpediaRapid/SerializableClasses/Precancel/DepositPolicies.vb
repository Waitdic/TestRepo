Imports Newtonsoft.Json

Namespace ExpediaRapid.SerializableClasses.BookingItinerary

    Public Class DepositPolicy

        <JsonProperty("amount")>
        Public Property Amount As Decimal

        <JsonProperty("due")>
        Public Property Due As Date

    End Class

End Namespace
