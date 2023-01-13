namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class RoomRateDescription
    {
        [XmlElement("Text")]
        public List<string> Text { get; set; } = new();
    }

}
