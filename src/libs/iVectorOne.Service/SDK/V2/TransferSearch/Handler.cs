namespace iVectorOne.SDK.V2.TransferSearch
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using iVectorOne.Search.Models;
    using iVectorOne.Services;

    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly ITransferSearchService _searchService;
        private readonly IRequestTracker _requestTracker;

        public Handler(
            ITransferSearchService searchService,
            IRequestTracker requestTracker)
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