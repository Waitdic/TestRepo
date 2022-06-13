namespace ThirdParty.CSSuppliers.DerbySoft
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using ThirdParty.Constants;

    public class DerbySoftSupport
    {
        private const string DateFormat = "yyyy-MM-dd";

        public static JsonSerializerSettings GetJsonSerializerSettings() =>
           new JsonSerializerSettings { DateFormatString = DateFormat };

        public static List<string> DerbysoftSources => new()
        {
            ThirdParties.DERBYSOFTCLUBMED,
            ThirdParties.DERBYSOFTSMARRIOTT,
            ThirdParties.DERBYSOFTSYNXIS,
        };
    }
}