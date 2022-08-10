using System.Xml.Serialization;

namespace iVectorOne.Suppliers.GoGlobal.Models
{
    [XmlRoot("Root")]
    public class Root<T> where T : Main, new()
    {
        public Header Header { get; set; } = new();
        public T Main { get; set; } = new();
    }
}
