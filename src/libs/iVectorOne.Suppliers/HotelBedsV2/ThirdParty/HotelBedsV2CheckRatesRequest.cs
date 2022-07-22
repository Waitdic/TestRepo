#nullable disable warnings

namespace iVectorOne.CSSuppliers.HotelBedsV2
{
    public class HotelBedsV2CheckRatesRequest
    {

        public string language { get; set; }
        public string upselling { get; set; }
        public Room[] rooms { get; set; }

        public class Room
        {
            public string rateKey { get; set; }
        }

    }
}

#nullable restore warnings