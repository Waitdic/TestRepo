namespace iVectorOne.CSSuppliers.Jumbo.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class RoomCombination
    {
        public Room rooms { get; set; } = new();

        [XmlElement("prices")]
        public List<Price> prices { get; set; } = new();
    }
}