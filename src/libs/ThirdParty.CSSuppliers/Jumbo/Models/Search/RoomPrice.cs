namespace ThirdParty.CSSuppliers.Jumbo.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class RoomPrice
    {
        [XmlElement("comments")]
        public List<Comment> comments { get; set; } = new();

        public int paxes { get; set; }

        public decimal price { get; set; }

        public string typeCode { get; set; }
    }
}