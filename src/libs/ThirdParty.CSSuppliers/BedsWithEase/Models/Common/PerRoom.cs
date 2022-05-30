namespace ThirdParty.CSSuppliers.BedsWithEase.Models.Common
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class PerRoom
    {
        public int PerRoomRecordNumber { get; set; }

        [XmlArray("Adults")]
        [XmlArrayItem("Person")]
        public List<Person> Adults { get; set; } = new();

        [XmlArray("Children")]
        [XmlArrayItem("Person")]
        public Person[] Children { get; set; } = Array.Empty<Person>();

        public bool ShouldSerializeChildren()
            => Children.Any();

        [XmlArray("Infants")]
        [XmlArrayItem("Person")]
        public Person[] Infants { get; set; } = Array.Empty<Person>();

        public bool ShouldSerializeInfants()
            => Infants.Any();
    }
}
