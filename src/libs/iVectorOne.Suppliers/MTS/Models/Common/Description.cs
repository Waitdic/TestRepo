namespace iVectorOne.Suppliers.MTS.Models.Common
{
    using System.Xml.Serialization;

    public class Description
    {
        [XmlElement()]
        public string Text = string.Empty;

        [XmlAttribute()]
        public string Name = string.Empty;
    }
}
