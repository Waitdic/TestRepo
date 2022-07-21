namespace ThirdParty.CSSuppliers.TBOHolidays.Models.Common
{
    using System.Xml.Serialization;

    public class TextPolicy : BasePolicy
    {
        [XmlText]
        public string Value { get; set; } = string.Empty;
    }
}
