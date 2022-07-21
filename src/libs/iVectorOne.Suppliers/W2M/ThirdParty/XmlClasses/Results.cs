using System.Collections.Generic;
using System.Xml.Serialization;

namespace iVectorOne.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "Results")]
    public class Results
    {
        [XmlElement(ElementName = "HotelResult")]
        public HotelResult HotelResult { get; set; }
    }

    [XmlRoot(ElementName = "Results")]
    public class HotelAvailResults
    {
        [XmlElement(ElementName = "HotelResult")]
        public List<HotelAvailResponseHotelResult> HotelResultList { get; set; }
    }
}
