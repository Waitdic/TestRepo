namespace ThirdParty.SDK.V2.PropertyList
{
    using System;
    using MediatR;

    public record Request : RequestBase, IRequest<Response>
    {
        public DateTime? LastModified { get; set; }

        public string Suppliers { get; set; }
    }
}