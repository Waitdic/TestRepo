namespace iVectorOne.Suppliers.PremierInn.Models.Soap
{
    using System.Xml;
    using System.Xml.Serialization;

    public class ProcessMessage : SoapContent
    {
        [XmlIgnore]
        public string Content { get; set; }

        [XmlElement("XMLIn")]
        public XmlNode[] XMLIn
        {
            get
            {
                var dummy = new XmlDocument();
                return new XmlNode[] { dummy.CreateCDataSection(Content) };
            }
            set => Content = value[0].Value;
        }
    }
}
