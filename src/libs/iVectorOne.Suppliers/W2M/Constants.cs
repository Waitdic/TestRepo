namespace iVectorOne.CSSuppliers
{
    internal class Constants
    {
        internal class CancellationResponse
        {
            internal const string CancellationCostRetrieved = "warnCancellationCostRetrieved";
            internal const string CancelledAndCancellationCostRetrieved = "warnCancelledAndCancellationCostRetrieved";
            internal const string ReservationAlreadyCancelledCode = "RESERVATION_ALREADY_CANCELLED";
        }

        internal class HotelAvailCheck
        {
            internal const string PriceChangedWarning = "warnPriceChanged";
        }

        internal class SoapActions
        {
            internal const string AvailabilityCheck = "HotelAvail";
            internal const string HotelAvailabilityCheck = "HotelCheckAvail";
            internal const string CancelBooking = "CancelBooking";
            internal const string HotelBooking = "HotelBooking";
            internal const string HotelBookingRules = "HotelBookingRules";
        }

        internal class RequestNames
        {
            internal const string Search = "Search";
            internal const string AvailabilityCheck = "Availability Check";
            internal const string CancelBooking = "Cancel Booking";
            internal const string HotelBooking = "Hotel Booking";
            internal const string HotelBookingRules = "Hotel Booking Rules";
        }

        internal class Comments
        {
            internal const string GeneralBookingCommentType = "RES";
        }

        internal const char BookingCodesSeparator = '|';
        internal const string ErrataTitle = "Important information";
        internal const string DateTimeFormat = "yyyy-MM-dd";
        internal const string SoapVersion = "1.1";
        internal const string ErrorsNode = "<Errors>";
        internal const string FailedReference = "failed";
    }
}
