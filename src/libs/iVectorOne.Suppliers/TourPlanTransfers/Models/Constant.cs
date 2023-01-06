using iVectorOne.Models;

namespace iVectorOne.Suppliers.TourPlanTransfers.Models
{
    public static class Constant
    {
        public const string DateTimeFormat = "yyyy-MM-dd";
        public const string Info = "GSIT";
        public const string TransferOptText = "TR????????????";
        public const string TransferOptExoText = "TF????????????";
        public const string OneWay = "OneWay";
        public const string Outbound = "Outbound";
        public const string UnexpectedError = "Unexpected error executing search request.";
        public const string FreesaleCode = "OK";
        public const string OnRequestCode = "RQ";
        public static readonly Warning BookException = new Warning("BookException", "Failed to confirm booking");
        public static readonly Warning CancelException = new Warning("CancelException", "Failed to cancel bookng");
        public static readonly Warning PrebookException = new Warning("PrebookException", "Failed to prebook");
        public const string InvalidSupplierReference = "Invalid Supplier Reference";

    }
}
