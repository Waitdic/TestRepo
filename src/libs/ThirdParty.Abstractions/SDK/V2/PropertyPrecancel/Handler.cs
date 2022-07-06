namespace ThirdParty.SDK.V2.PropertyPrecancel
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using ThirdParty.Services;

    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly ICancellationService _cancelService;

        public Handler(ICancellationService cancelService)
        {
            _cancelService = cancelService;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            return await _cancelService.GetCancellationFeesAsync(request);
        }
    }
}