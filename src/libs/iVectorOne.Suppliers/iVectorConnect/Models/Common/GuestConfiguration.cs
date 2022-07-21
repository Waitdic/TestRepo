namespace ThirdParty.CSSuppliers.iVectorConnect.Models.Common
{
    using System;
    using System.Linq;
    using System.Xml.Serialization;

    public class GuestConfiguration
    {
        public int Adults { get; set; }

        public int Children { get; set; }

        public int Infants { get; set; }

        [XmlArray("ChildAges")]
        [XmlArrayItem("ChildAge")]
        public int[] ChildAges { get; set; } = Array.Empty<int>();

        public bool ShouldSerializeChildAges()
        {
            return ChildAges.Any();
        }
    }
}
