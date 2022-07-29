namespace iVectorOne.Suppliers.DerbySoft
{
    using Newtonsoft.Json;
    using iVectorOne.Suppliers.DerbySoft.Models;
    using iVectorOne.Models.Property.Booking;

    public class PreBookHelper
    {
        public string SearchToken = string.Empty;
        public string PreBookToken = string.Empty;
        public string PreBookBookingToken = string.Empty;
        public Cancellations? Cancellations;
        public RoomRate? RoomRate;

        public PreBookHelper() { }
        public PreBookHelper(string searchToken, RoomRate roomRate, Cancellations cancellations)
        {
            SearchToken = searchToken;
            RoomRate = roomRate;
            Cancellations = cancellations;
        }
        public static string SerializePreBookHelper(PreBookHelper preBookHelper)
        {
            //escape curly brackets 
            return JsonConvert.SerializeObject(preBookHelper).Replace("{", "{{").Replace("}", "}}");
        }
        public static PreBookHelper DeserializePreBookHelper(string preBookHelper)
        {
            //unescape curly brackets 
            if (preBookHelper.StartsWith("{{"))
            {
                preBookHelper = preBookHelper.Replace("{{", "{").Replace("}}", "}");
            }

            //html encoding (for connect only)
            if (preBookHelper.Contains("&quot;"))
            {
                preBookHelper = System.Net.WebUtility.HtmlDecode(preBookHelper);
            }

            return JsonConvert.DeserializeObject<PreBookHelper>(preBookHelper);
        }
    }
}