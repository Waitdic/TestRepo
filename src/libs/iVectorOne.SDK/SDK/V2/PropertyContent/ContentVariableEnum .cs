namespace iVectorOne.SDK.V2.PropertyContent
{
    using System.Text.Json.Serialization;

    [JsonConverter(typeof(JsonStringEnumConverter))]
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