namespace ThirdParty.CSSuppliers.Models.Altura
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class RequestRoom
    {
        public RequestRoom() { }
        [XmlAttribute("code")]
        public int Code { get; set; }

        [XmlElement("Adults")]
        public int Adults { get; set; }

        [XmlElement("Children")]
        public int Children { get; set; }

        [XmlArray("ChildrenAges")]
        [XmlArrayItem("ChildAge")]
        public List<int> ChildrenAges { get; set; } = new();
    }
}
