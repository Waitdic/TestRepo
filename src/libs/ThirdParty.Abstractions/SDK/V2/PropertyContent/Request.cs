namespace ThirdParty.SDK.V2.PropertyContent
{
    using MediatR;

    public record Request : RequestBase, IRequest<Response>
    {
        public string PropertyIDs { get; set; } = "";
    }
}