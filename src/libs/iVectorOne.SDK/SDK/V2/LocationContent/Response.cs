namespace iVectorOne.SDK.V2.LocationContent
{
    using System.Collections.Generic;

    public record Response: ResponseBase
    {
        /// <summary>Gets or sets locations.</summary>
        public List<Location> Locations { get; set; } = new ();
    }


}
