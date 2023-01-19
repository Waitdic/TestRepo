namespace iVectorOne.SDK.V2.ExtraContent
{
    using System.Collections.Generic;

    public record Response: ResponseBase
    {
        /// <summary>Gets or sets extras.</summary>
        public List<Extra> Extras { get; set; } = new ();
    }


}
