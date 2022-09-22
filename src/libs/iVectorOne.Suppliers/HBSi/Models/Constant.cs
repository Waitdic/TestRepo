namespace iVectorOne.Suppliers.HBSi.Models
{
    using System;
    public static class Constant
    {
        public const string TimeStampFormat = "yyyy-MM-ddThh:mm:ss.fffzzz";
        public const string DateFormat = "yyyy-MM-dd";
        public const string AdultAgeCode = "10";
        public const string ChildAgeCode = "8";
        public const string InfantAgeCode = "7";
        public const int ChildMaxAge = 18;
        public const int InfantAge = 1;
        public static DateTime UnitTestDateTime = new(2022, 07, 18);
        public static int ProfileType_Customer = 1;
        public const string PaymentMethod_DirectBill = "DirectBill";
        public const string Failed = "failed";
    }
}
