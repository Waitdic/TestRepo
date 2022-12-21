namespace iVectorOne.Suppliers.PremierInn.Models.Common
{
    using System.Xml.Serialization;

    public class CancellationPolicy
    {
        [XmlAttribute] 
        public int Category { get; set; }
        
        [XmlAttribute]
        public int Days { get; set; }

        public string Text { get; set; } = string.Empty;
    }
}
