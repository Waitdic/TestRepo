namespace iVectorOne.Suppliers.TBOHolidays.Models.Common
{
    using System.Xml.Serialization;

    public class DefaultPolicy : BasePolicy
    {
        [XmlText]
        public string Value { get; set; } = string.Empty;
    }
}
