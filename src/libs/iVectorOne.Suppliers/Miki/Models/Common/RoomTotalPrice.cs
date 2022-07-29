namespace iVectorOne.Suppliers.Miki.Models.Common
{
    using System.Xml.Serialization;

    public class RoomTotalPrice
    {
        [XmlElement("price")]
        public decimal Price { get; set; }
    }
}
