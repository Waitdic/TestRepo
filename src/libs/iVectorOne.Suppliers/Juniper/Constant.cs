namespace iVectorOne.Suppliers.Juniper
{
    using System.Collections.Generic;
    using iVectorOne.Constants;

    public static class Constant
    {
        public const string RequestorTypeCode = "1";
        public const string DateTimeFormat = "yyyy-MM-dd";
        public const int DefaultAdultAge = 40;
        public const int DefaultInfantAge = 0;
        public const string ShowCatalugueData = "1";
        public const int SearchWebRequestLimit = 10;
        public const string SearchLogFile = "Search";
        public const string PreBookLogFile = "PreBook";
        public const string BookLogFile = "Book";
        public const string CancelLogFile = "Cancel";
        public const string NotEmptyStringToken = "NotEmpty";
        public const string AvailableForSale = "AvailableForSale";
        public const string RoomViewCodePromo = "PROMO";
        public const string SpecialOfferFeatureName = "VALUE";
        public const string DiscountFeatureName = "PRICE";
        public const string FailedToken = "[Failed]";
        public const string FailedBookReference = "failed";

        public static List<string> JuniperSources => new()
        {
            ThirdParties.JUNIPERECTRAVEL,
            ThirdParties.JUNIPERELEVATE,
            ThirdParties.JUNIPERFASTPAYHOTELS,
            ThirdParties.PORTIMAR,
        };
    }
}