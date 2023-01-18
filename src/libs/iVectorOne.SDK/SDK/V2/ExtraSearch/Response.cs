namespace iVectorOne.SDK.V2.ExtraSearch
{
    using System.Collections.Generic;

    public record Response : ResponseBase
    {
        public List<ExtraResult> ExtraResults { get; set; } = new List<ExtraResult>();
    }
}
