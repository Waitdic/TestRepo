namespace iVectorOne.Suppliers.GoGlobal
{
    public static class Constant
    {
        public const string DataFormat = "yyyy-MM-dd";
        public const string Version_2_3 = "2.3";

        public const string FormatJSON = "JSON";

        public static int MaxChildAge = 18;
        public static string ErrataId = "Errata";

        public static string OperationTypeRequest = "Request";

        public static string RequestHotelSearch = "HOTEL_SEARCH_REQUEST";
        public static int RequestCodeHotelSearch = 11;

        public static int PreBookRequestNumber = 9;
        public static string PreBookRequestCode = "BOOKING_VALUATION_REQUEST";

        public static int BookRequestNumber = 2;
        public static string BookRequestCode = "BOOKING_INSERT_REQUEST";

        public static int StatusConfirmationRequestNumber = 5;
        public static string StatusConfirmationRequestCode = "BOOKING_STATUS_REQUEST";

        public static int CancelRequestNumber = 3;
        public static string CancelRequestCode = "BOOKING_CANCEL_REQUEST";

        public static int BookingSearchRequestNumber = 4;
        public static string BookingSearchRequestCode = "BOOKING_SEARCH_REQUEST";
    }
}
