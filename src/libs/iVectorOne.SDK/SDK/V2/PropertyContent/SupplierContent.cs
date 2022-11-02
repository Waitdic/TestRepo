namespace iVectorOne.SDK.V2.PropertyContent
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    /// <summary>The supplier content e.g. images facilities and descriptions for a property.</summary>
    public class SupplierContent
    {
        /// <summary>Gets or sets the name of the property.</summary>
        public string PropertyName { get; set; } = string.Empty;

        /// <summary>Gets or sets the supplier of the property.</summary>
        public string Supplier { get; set; } = string.Empty;

        /// <summary>Gets or sets the supplier of the property property reference, TPKey in the database.</summary>
        public string SupplierPropertyReference { get; set; } = string.Empty;

        /// <summary>Gets or sets the address.</summary>
        public Address Address { get; set; } = new Address();

        /// <summary>Gets or sets the geography.</summary>
        public Geography Geography { get; set; } = new Geography();

        /// <summary>Gets or sets the rating.</summary>
        public string Rating { get; set; } = string.Empty;

        /// <summary>Gets or sets the description.</summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>Gets or sets the Hotel Policy.</summary>
        public string HotelPolicy { get; set; } = string.Empty;

        /// <summary>Gets or sets the facilities.</summary>
        public List<string> Facilities { get; set; } = new List<string>();

        /// <summary>Gets or sets the main image URL.</summary>
        public string MainImageURL { get; set; } = string.Empty;

        /// <summary>Gets or sets the images.</summary>
        public List<string> Images { get; set; } = new List<string>();

        /// <summary>Prior to V3, the full image details are returned in this node to avoid a breaking change.</summary>
        public List<Image> FullImages { get; set; } = new List<Image>();

        /// <summary>Gets or sets the content variables.</summary>
        public List<ContentVariable> ContentVariables { get; set; } = new List<ContentVariable>();

        /// <summary>Gets or sets the room types.</summary>
        public List<RoomType> RoomTypes { get; set; } = new List<RoomType>();

        [JsonIgnore]
        public string TPKey { get; set; } = string.Empty;
    }
}