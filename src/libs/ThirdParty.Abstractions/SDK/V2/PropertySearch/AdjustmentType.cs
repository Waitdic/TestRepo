namespace ThirdParty.SDK.V2.PropertySearch
{
    using System.Runtime.Serialization;
    using System.Text.Json.Serialization;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AdjustmentType
    {
        /// <summary>
        /// an Adjustment type of Supplement
        /// </summary>
        [EnumMember(Value = "Supplement")]
        Supplement,

        /// <summary>
        /// an Adjustment type of Offer
        /// </summary>
        [EnumMember(Value = "Offer")]
        Offer,

        /// <summary>
        /// an Adjustment type of Tax
        [EnumMember(Value = "Tax")]
        Tax
    }
}
