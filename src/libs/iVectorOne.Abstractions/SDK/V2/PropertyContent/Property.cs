namespace iVectorOne.SDK.V2.PropertyContent
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    /// <summary>
    ///   <para>A property containing all the supplier content associated with that property</para>
    /// </summary>
    public class Property
    {
        /// <summary>Gets or sets The Central Property Identifier, returned to the user as Property Identifier as they dont need to know our Database schema</summary>
        [JsonPropertyName("PropertyID")]
        public int CentralPropertyID { get; set; }

        /// <summary>Gets or sets The Travel Technology Initiative code</summary>
        public string TTICode { get; set; } = string.Empty;

        /// <summary>Gets or sets the content of the supplier.</summary>
        public List<SupplierContent> SupplierContent { get; set; } = new List<SupplierContent>();
    }
}