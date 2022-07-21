namespace ThirdParty.CSSuppliers.FastPayHotels.Models
{
    using System.Collections.Generic;

    public class FastPayHotelsAvailabilityRequest
    {
        public string messageID { get; set; } = string.Empty;
        public string currency { get; set; } = string.Empty;
        public string checkIn { get; set; } = string.Empty;
        public string checkOut { get; set; } = string.Empty;
        public List<SharedModels.Occupancy> occupancies { get; set; } = new List<SharedModels.Occupancy>();
        public List<string> hotelCodes { get; set; } = new List<string>();
        public Parameters parameters { get; set; }

        public class Parameters
        {
            public string countryOfResidence { get; set; } = string.Empty;
            public string nationality { get; set; } = string.Empty;
        }
    }
}