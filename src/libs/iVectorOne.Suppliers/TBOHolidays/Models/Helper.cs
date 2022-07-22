namespace iVectorOne.CSSuppliers.TBOHolidays.Models
{
    using System;
    using Intuitive.Helpers.Extensions;

    public static class Helper
    {
        public static string[] Separators = { "~~~" };

        public static ReferenceValues GetReferenceValues(string reference)
        {
            return new ReferenceValues(reference);
        }
    }

    public struct ReferenceValues
    {
        public ReferenceValues(string reference)
        {
            string[] items = reference.Split(Helper.Separators, StringSplitOptions.None);

            SessionId = items[0];
            ResultIndex = items[1].ToSafeInt();
            RoomIndex = items[2].ToSafeInt();
            RoomTypeCode = items[3];
            RatePlanCode = items[4];
            RoomRateInfo = items[5];
            SupplementInformation = items[6];
        }

        public string SessionId { get; }

        public int ResultIndex { get; }

        public int RoomIndex { get; }

        public string RoomTypeCode { get; }

        public string RatePlanCode { get; }

        public string RoomRateInfo { get; set; }

        public string SupplementInformation { get; set; }
    }
}
