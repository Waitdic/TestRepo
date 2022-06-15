namespace ThirdParty.CSSuppliers.iVectorChannelManager.Models
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public partial class SearchRequest
    {
        public SearchRequest() { }

        public SearchRequest(BookingLogin LoginDetails)
        {
            this.LoginDetails = LoginDetails;
        }

        public BookingLogin LoginDetails { get; set; }

        public int BrandID { get; set; }
        public bool SearchPackageRates { get; set; } = false;

        [XmlArrayItem("PropertyReferenceID")]
        public List<int> PropertyReferenceIDs { get; set; }

        public string CheckInDate { get; set; }
        public string CheckOutDate { get; set; }

        public List<Room> Rooms { get; set; }

        public partial class Room
        {
            public int Seq { get; set; }
            public int Adults { get; set; }
            public int Children { get; set; }
            public int Infants { get; set; }
            public string ChildAgeCSV { get; set; }
        }

    }
}