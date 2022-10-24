namespace iVectorOne.Suppliers.ExpediaRapid.SerializableClasses.Book
{
    using System;
    using Newtonsoft.Json;

    public class Phone
    {

        [JsonProperty("country_code")]
        public string CountryCode { get; set; }

        [JsonProperty("area_code")]
        public string AreaCode { get; set; }

        [JsonProperty("number")]
        public string Number { get; set; }

        public Phone()
        {
        }

        public Phone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException("Phone number not supplied");

            phone = phone.TrimStart(new[] { '+', '0' });

            CountryCode = phone.Remove(2);
            phone = phone[2..];

            AreaCode = phone.Remove(2);
            Number = phone[2..];
        }
    }
}