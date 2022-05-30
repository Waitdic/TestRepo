namespace ThirdParty.SDK.V2.PropertySearch
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using ThirdParty.Search.Models;
    using ThirdParty.Services;

    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly ISearchService _searchService;
        private readonly IRequestTracker _requestTracker;

        public Handler(
            ISearchService searchService,
            IRequestTracker requestTracker)
        {
            _searchService = searchService;
            _requestTracker = requestTracker;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            return await _searchService.SearchAsync(request, request.User, false, _requestTracker);
        }
    }
}