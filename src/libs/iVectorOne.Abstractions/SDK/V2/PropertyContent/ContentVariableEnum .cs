namespace ThirdParty.SDK.V2.PropertyContent
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ContentVariableEnum
    {
        Distance,
        Distances,
        km,
        m,
        AccommodationTypes,
        AccommodationType
    }
}