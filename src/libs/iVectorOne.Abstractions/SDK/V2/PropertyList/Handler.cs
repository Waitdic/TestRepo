namespace iVectorOne.SDK.V2.PropertyList
{
    using System.Threading;
    using System.Threading.Tasks;
    using Intuitive;
    using MediatR;
    using iVectorOne.Services;

    // todo - move handler out of abstractions code
    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly IPropertyContentService _service;

        public Handler(IPropertyContentService service)
        {
            _service = Ensure.IsNotNull(service, nameof(service));
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            return await _service.PropertyListAsync(request.LastModified, request.Suppliers, request.Account);
        }
    }
}