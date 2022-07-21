namespace iVectorOne.CSSuppliers.AmadeusHotels.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class DataElementsMaster
    {
        [XmlElement("dataElementsIndiv")]
        public DataElementsIndiv[] DataElementsIndiv { get; set; } = Array.Empty<DataElementsIndiv>();
    }
}
