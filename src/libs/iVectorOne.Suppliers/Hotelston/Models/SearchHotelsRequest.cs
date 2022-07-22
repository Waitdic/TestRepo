namespace iVectorOne.CSSuppliers.Hotelston.Models
{
    using System.Xml.Serialization;
    using Common;

    public class SearchHotelsRequest : RequestBase
    {
        [XmlElement("criteria")]
        public Criteria Criteria { get; set; } = new();
    }
}
