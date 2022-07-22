namespace iVectorOne.CSSuppliers.Miki.Models.Common
{
    using System.Xml.Serialization;

    [XmlRoot("dailyB2BPasswords", Namespace = SoapNamespaces.Xsi)]
    public class AccessCodesTest : AccessCodesBase
    {
        [XmlAttribute("noNamespaceSchemaLocation")]
        public string NoNamespaceSchemaLocation => "null";
    }
}
