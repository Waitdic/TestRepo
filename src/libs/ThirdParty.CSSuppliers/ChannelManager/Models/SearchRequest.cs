namespace ThirdParty.CSSuppliers.ChannelManager.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using ThirdParty.CSSuppliers.ChannelManager.Models.Common;

    public class SearchRequest
    {
        public SearchRequest()
        {
        }

        public SearchRequest(BookingLogin LoginDetails)
        {
            this.LoginDetails = LoginDetails;
        }

        public BookingLogin LoginDetails { get; set; } = new();

        public int BrandID { get; set; }
        public bool SearchPackageRates { get; set; }

        [XmlArrayItem("PropertyReferenceID")]
        public List<int> PropertyReferenceIDs { get; set; } = new();

        public string CheckInDate { get; set; }
        public string CheckOutDate { get; set; }

        public List<Room> Rooms { get; set; } = new();

        public class Room
        {
            public int Seq { get; set; }
            public int Adults { get; set; }
            public int Children { get; set; }
            public int Infants { get; set; }
            public string ChildAgeCSV { get; set; }
        }
    }
}