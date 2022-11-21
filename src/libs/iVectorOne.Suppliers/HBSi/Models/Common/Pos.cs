namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Xml.Serialization;

    public class Pos
    {
        public Pos() { }

        [XmlElement("Source")]
        public PosSource Source { get; set; } = new();
    }
}
