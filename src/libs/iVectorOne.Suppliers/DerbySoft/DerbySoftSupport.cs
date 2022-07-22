namespace iVectorOne.CSSuppliers.DerbySoft
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using iVectorOne.Constants;

    public class DerbySoftSupport
    {
        private const string DateFormat = "yyyy-MM-dd";

        public static JsonSerializerSettings GetJsonSerializerSettings() =>
           new JsonSerializerSettings { DateFormatString = DateFormat };

        public static List<string> DerbysoftSources => new()
        {
            ThirdParties.DERBYSOFTCLUBMED,
            ThirdParties.DERBYSOFTMARRIOTT,
            ThirdParties.DERBYSOFTSYNXIS,
        };
    }
}