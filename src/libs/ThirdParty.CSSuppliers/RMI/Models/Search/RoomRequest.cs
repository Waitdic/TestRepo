namespace ThirdParty.CSSuppliers.RMI.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class RoomRequest
    {
        public int Adults { get; set; }
        public int Children { get; set; }
        public int Infants { get; set; }

        [XmlArray("ChildAges")]
        [XmlArrayItem("ChildAge")]
        public List<ChildAge> ChildAges { get; set; }
    }
}
