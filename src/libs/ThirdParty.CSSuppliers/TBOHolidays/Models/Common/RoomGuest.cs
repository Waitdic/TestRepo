using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

public class RoomGuest
{
    [XmlAttribute]
    public int AdultCount { get; set; }

    [XmlAttribute]
    public int ChildCount { get; set; }

    public List<int> ChildAge { get; set; } = new();
    public bool ShouldSerializeChildAge() => ChildAge.Any();
}