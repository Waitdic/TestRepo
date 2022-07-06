namespace ThirdParty.CSSuppliers.AbreuV2.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class HotelResInfo
    {
        [XmlArray("HotelResIDs")]
        [XmlArrayItem("HotelResID")]
        public List<ResID> HotelResIDs { get; set; } = new();
    }
}
