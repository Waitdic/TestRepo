namespace iVectorOne.Suppliers.Polaris.Models
{
    internal static class Constant
    {
        public const string DateFormat = "yyyy-MM-dd";
        public const int DefaultAdultAge = 30;
        public const int DefaultInfantAge = 0;

        internal static class LocationType
        {
            public const string Empty = "EMPTY";
            public const string Commercial = "COMMERCIAL";
            public const string Geographic = "GEOGRAPHIC";
            public const string Zone = "ZONE";
        }

        internal static class Status 
        {
            public const string Ok = "OK";
            public const string Error = "ERROR";
        }
    }
}
