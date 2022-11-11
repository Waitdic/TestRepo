namespace iVectorOne.SDK.V2.PropertyContent
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
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
            return await _service.PropertyContentAsync(
                request.PropertyIDs.Select(s => s.ToSafeInt()).ToList(),
                request.Account,
                request.IncludeRoomTypes);
        }
    }
}