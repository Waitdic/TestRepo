namespace iVectorOne.SDK.V2.PropertyContent
{
    using MediatR;
    using System.Collections.Generic;

    public record Request : RequestBase, IRequest<Response>
    {
        public List<string> PropertyIDs { get; set; } = new();
        public bool IncludeRoomTypes { get; set; } = false;
    }
}