namespace iVectorOne.SDK.V2.TransferSearch
{
    using System.Collections.Generic;

    public record Response : ResponseBase
    {
        public List<TransferResult> TransferResults { get; set; } = new List<TransferResult>();
    }
}
