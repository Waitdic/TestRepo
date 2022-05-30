namespace ThirdParty.SDK.V2.PropertyContent
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    /// <summary>
    ///   <para>A property containing all the supplier content associated with that property</para>
    /// </summary>
    public class Property
    {
        /// <summary>Gets or sets The Central Property Identifier, returned to the user as Property Identifier as they dont need to know our Database schema</summary>
        /// <value>The central property identifier.</value>
        [JsonProperty("PropertyID")]
        public int CentralPropertyID { get; set; }

        /// <summary>Gets or sets The Travel Technology Initiative code</summary>
        /// <value>The Travel Technology Initiative code.</value>
        public string TTICode { get; set; } = string.Empty;

        /// <summary>Gets or sets the content of the supplier.</summary>
        /// <value>The content of the supplier.</value>
        public List<SupplierContent> SupplierContent { get; set; } = new List<SupplierContent>();
    }
}
