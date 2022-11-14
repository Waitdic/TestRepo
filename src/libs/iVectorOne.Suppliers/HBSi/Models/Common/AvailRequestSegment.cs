namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class AvailRequestSegment
    {
        [XmlArray(ElementName = "HotelSearchCriteria")]
        [XmlArrayItem(ElementName = "Criterion")]
        public List<Criterion> HotelSearchCriteria { get; set; } = new List<Criterion>();
    }
}
