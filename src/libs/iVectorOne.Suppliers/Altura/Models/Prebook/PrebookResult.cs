namespace iVectorOne.Suppliers.Models.Altura
{
    using System.Xml.Serialization;

    public class PrebookResult
    {
        public PrebookResult() { }

        [XmlElement("State")]
        public string State { get; set; }
    }
}
