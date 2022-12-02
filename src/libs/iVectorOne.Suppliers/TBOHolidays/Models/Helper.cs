namespace iVectorOne.Suppliers.TBOHolidays.Models
{
    using System;
    using System.Text;
    using Intuitive.Helpers.Extensions;
    using iVectorOne.Search.Models;

    public static class Helper
    {
        public static string[] Separators = { "~~~" };

        public static ReferenceValues GetReferenceValues(string reference)
        {
            return new ReferenceValues(reference);
        }

        public static string CleanRequest(string request)
        {
            return request
                .Replace(@"<?xml version=""1.0"" encoding=""utf-8""?>", "")
                .Replace(@"xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""", "");
        }

        public static string GetAuth(string user, string password)
        {
            return "Basic " + Convert.ToBase64String(
            ASCIIEncoding.ASCII.GetBytes($"{user}:{password}"));
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
