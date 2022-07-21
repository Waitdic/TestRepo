namespace ThirdParty.SDK.V2.PropertyContent
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>The supplier content e.g. images facilities and descriptions for a property.</summary>
    public class SupplierContent
    {
        /// <summary>Gets or sets the name of the property.</summary>
        /// <value>The name of the property.</value>
        public string PropertyName { get; set; } = string.Empty;

        /// <summary>Gets or sets the supplier of the property.</summary>
        /// <value>The supplier.</value>
        public string Supplier { get; set; } = string.Empty;

        /// <summary>Gets or sets the address.</summary>
        /// <value>The address.</value>
        public Address Address { get; set; } = new Address();

        /// <summary>Gets or sets the geography.</summary>
        /// <value>The geography.</value>
        public Geography Geography { get; set; } = new Geography();

        /// <summary>Gets or sets the rating.</summary>
        /// <value>The rating.</value>
        public string Rating { get; set; } = string.Empty;

        /// <summary>Gets or sets the description.</summary>
        /// <value>The description.</value>
        public string Description { get; set; } = string.Empty;

        /// <summary>Gets or sets the Hotel Policy.</summary>
        /// <value>The Hotel Policy.</value>
        public string HotelPolicy { get; set; } = string.Empty;

        /// <summary>Gets or sets the facilities.</summary>
        /// <value>The facilities.</value>
        public List<string> Facilities { get; set; } = new List<string>();

        /// <summary>Gets or sets the main image URL.</summary>
        /// <value>The main image URL.</value>
        public string MainImageURL { get; set; } = string.Empty;

        /// <summary>Gets or sets the images.</summary>
        /// <value>The images.</value>
        public List<string> Images { get; set; } = new List<string>();

        /// <summary>Gets or sets the content variables.</summary>
        /// <value>The content variables.</value>
        public List<ContentVariable> ContentVariables { get; set; } = new List<ContentVariable>();

        [JsonIgnore]
        public string TPKey { get; set; } = string.Empty;
    }
}