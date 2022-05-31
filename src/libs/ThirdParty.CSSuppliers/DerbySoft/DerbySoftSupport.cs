namespace ThirdParty.CSSuppliers.DerbySoft
{
    using Newtonsoft.Json;

    public class DerbySoftSupport
    {

        private const string DateFormat = "yyyy-MM-dd";
        public static JsonSerializerSettings GetJsonSerializerSettings() =>
           new JsonSerializerSettings { DateFormatString = DateFormat };
    }
}