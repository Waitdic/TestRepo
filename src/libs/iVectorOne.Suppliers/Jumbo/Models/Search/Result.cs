namespace iVectorOne.CSSuppliers.Jumbo.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class Result
    {
        [XmlElement("availableHotels")]
        public List<AvailableHotel> availableHotels { get; set; } = new();
    }
}