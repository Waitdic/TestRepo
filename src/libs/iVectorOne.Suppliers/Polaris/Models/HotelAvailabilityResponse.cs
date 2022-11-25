namespace iVectorOne.Suppliers.Polaris.Models
{
    using System.Collections.Generic;

    public class HotelAvailabilityResponse
    {
        public List<Hotel> Hotels { get; set; } = new();
        public Notes Notes { get; set; } = new();
    }

}
