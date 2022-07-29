using System;

namespace iVectorOne.Suppliers.Models.Altura
{
    public static class Constant
    {
        public const string RequestTypeSearch = "Availability";
        public const string RequestTypePrebook = "CheckResult";
        public const string RequestTypeBook = "Confirmation";
        public const string LogfilePrebook = "PrebookCheck";
        public const string LogFileBook = "BookingConfirmation";
        public const string LogFilePreCancel = "PreCancel";
        public const string LogFileCancel = "Cancel";
        public const string RequestTypeCancellation = "CancellationConfirmation";
        public const string RequestTypeCancelCost = "CancellationRequest";
        public const string DestinationTypeHotel = "HOTEL";
        public const string ApiVersion = "v2.8.3";
        public const string ApiDateFormat = "yyyyMMdd";
        public const string StatusAvailable = "Available";
        public const string StatusConfirmed = "CONFIRMED";
        public const string StatusCancelled = "Cancelled";
        public static DateTime DateMax = new(2099, 1, 1);
        public const string FailedReference = "[Failed]";
    }
}
