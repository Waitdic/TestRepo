namespace iVectorOne.SDK.V2.ExtraSearch
{
    using iVectorOne.Search.Models;
    using iVectorOne.Services.Extra;
    using MediatR;
    using System.Threading;
    using System.Threading.Tasks;

    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly ISearchService _searchService;
        private readonly IRequestTracker _requestTracker;

        public Handler(ISearchService searchService, IRequestTracker requestTracker)
        {
            _searchService = searchService;
            _requestTracker = requestTracker;
        }
        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            return await _searchService.SearchAsync(request, false, _requestTracker);
        }
    }
}
