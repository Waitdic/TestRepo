namespace iVectorOne.SDK.V2.LocationContent
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using iVectorOne.Services;

    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly ILocationContentService _locationContentService;

        public Handler(ILocationContentService locationContentService)
        {
            _locationContentService = locationContentService;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            return await _locationContentService.GetAllLocations(request);
        }
    }
}