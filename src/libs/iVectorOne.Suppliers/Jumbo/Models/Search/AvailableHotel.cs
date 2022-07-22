namespace iVectorOne.CSSuppliers.Jumbo.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class AvailableHotel
    {
        public Establishment establishment { get; set; }

        [XmlElement("roomCombinations")]
        public List<RoomCombination> roomCombinations { get; set; } = new();
    }
}