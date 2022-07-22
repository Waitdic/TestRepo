namespace iVectorOne.CSSuppliers.AbreuV2.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class OTA_HotelAvailRS
    {
        [XmlArray("Hotels")]
        [XmlArrayItem("Hotel")]
        public List<Hotel> Hotels { get; set; } = new();
        public BestPrice BestPrice { get; set; } = new();

    }
}
