namespace ThirdParty.SDK.V2.PropertyList
{
    using System.Collections.Generic;

    public record Response
    {
        public List<int> Properties { get; set; } = new List<int>();
    }
}