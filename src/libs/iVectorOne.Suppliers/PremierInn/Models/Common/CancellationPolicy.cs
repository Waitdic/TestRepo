using System.Xml.Serialization;

namespace iVectorOne.Suppliers.PremierInn.Models.Common
{
    public class CancellationPolicy
    {
        [XmlAttribute] 
        public string Category { get; set; } = string.Empty;

        public string Text { get; set; } = string.Empty;
    }
}
