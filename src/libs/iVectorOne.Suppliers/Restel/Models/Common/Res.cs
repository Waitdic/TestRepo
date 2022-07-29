using System.Xml;
using System.Xml.Schema;

namespace iVectorOne.Suppliers.Restel.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class Res : IXmlSerializable
    {
        public string[] Lin { get; set; } = Array.Empty<string>();

        public void WriteXml(XmlWriter writer)
        {
            foreach (string s in Lin)
            {
                writer.WriteStartElement("lin");
                writer.WriteCData(s);
                writer.WriteEndElement();
            }
        }

        public XmlSchema GetSchema()
        {
            throw new NotSupportedException();
        }

        public void ReadXml(XmlReader reader)
        {
            throw new NotSupportedException();
        }
    }
}