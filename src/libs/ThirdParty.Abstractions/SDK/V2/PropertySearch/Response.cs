namespace ThirdParty.SDK.V2.PropertySearch
{
    using System.Collections.Generic;

    public record Response
    {
        public List<PropertyResult> PropertyResults { get; set; } = new List<PropertyResult>();
    }
}