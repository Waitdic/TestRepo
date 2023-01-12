namespace iVectorOne.SDK.V2.LocationContent
{
    using MediatR;
    public record Request: RequestBase, IRequest<Response>
    {
        /// <summary>Gets or sets the supplier.</summary>
        public string Supplier { get; set; } = string.Empty;
    }
}
