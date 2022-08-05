namespace iVectorOne.Suppliers.GoGlobal.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class SearchRs : MainJson
    {
        [XmlElement("Hotel")]
        public List<Hotel> Hotels { get; set; }
    }

    public class MainJson
    {
        public Header Header { get; set; } = new();

    }
}