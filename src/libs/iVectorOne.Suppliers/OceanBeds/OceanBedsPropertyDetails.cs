namespace ThirdParty.CSSuppliers.OceanBeds
{
    using System.Linq;
    using iVector.Search.Property;
    using ThirdParty.Models.Property.Booking;
    using ThirdParty.Search.Models;
    using RoomDetails = iVector.Search.Property.RoomDetails;

    public class OceanBedsPropertyDetails
    {
        public string PropertyCode { get; set; } = string.Empty;
        public string PropertyType { get; set; }
        public string StateId { get; set; }
        public string CityId { get; set; }
        public string Region { get; set; }
        public string ArrivalDate { get; set; }
        public string DepartureDate { get; set; }
        public RoomDetails RoomDetails { get; set; }

        public OceanBedsPropertyDetails(SearchDetails searchDetails, string resortCode)
        {
            string[] codes = resortCode.Split('|');
            PropertyType = codes[0];
            StateId = codes[1];
            CityId = codes[2];
            Region = codes[2];
            ArrivalDate = searchDetails.ArrivalDate.ToDateString();
            DepartureDate = searchDetails.DepartureDate.ToDateString();
            RoomDetails = searchDetails.RoomDetails;
        }

        public OceanBedsPropertyDetails(PropertyDetails propertyDetails)
        {
            string[] codes = propertyDetails.ResortCode.Split('|');
            PropertyCode = propertyDetails.Rooms[0].ThirdPartyReference.Split('|')[1];
            PropertyType = codes[0];
            StateId = codes[1];
            CityId = codes[2];
            Region = codes[2];
            ArrivalDate = propertyDetails.ArrivalDate.ToDateString();
            DepartureDate = propertyDetails.DepartureDate.ToDateString();
            RoomDetails = new RoomDetails();
            RoomDetails.AddRange(
                propertyDetails.Rooms.Select(o => new RoomDetail()
                {
                    Adults = o.Adults,
                    Children = o.Children,
                    Infants = o.Infants
                }));
        }
    }
}