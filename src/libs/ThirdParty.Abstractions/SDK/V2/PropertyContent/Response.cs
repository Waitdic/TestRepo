namespace ThirdParty.SDK.V2.PropertyContent
{
    using System.Collections.Generic;

    public record Response : ResponseBase
    {
        public List<Property> Properties { get; set; } = new List<Property>();
    }
}