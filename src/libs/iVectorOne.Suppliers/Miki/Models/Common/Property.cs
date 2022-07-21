namespace ThirdParty.CSSuppliers.Miki.Models.Common
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class Property
    {
        [XmlAttribute("ProductCode")] 
        public string ProductCode { get; set; } = string.Empty;

        public List<RoomProperty> Rooms { get; set; } = new();
    }
}
