namespace iVectorOne.Suppliers.Miki.Models.Common
{
    using System.Collections.Generic;

    public class Result
    {
        public Envelope<HotelSearchResponse> Response { get; set; } = new();

        public Properties Properties { get; set; } = new();

        public List<RoomDescription> RoomDescriptions { get; set; } = new();
    }
}
