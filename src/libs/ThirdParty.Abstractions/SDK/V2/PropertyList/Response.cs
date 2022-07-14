namespace ThirdParty.SDK.V2.PropertyList
{
    using System.Collections.Generic;

    public record Response : ResponseBase
    {
        public List<int> Properties { get; set; } = new List<int>();
    }
}