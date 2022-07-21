namespace iVectorOne.CSSuppliers.Models.Altura
{
    using System.Xml.Serialization;

    [XmlRoot("AlturaDS_responses")]
    public class AlturaPrebookResponse
    {
        public AlturaPrebookResponse() { }

        [XmlElement("Response")]
        public PrebookResponse Response { get; set; } = new();
    }
}
