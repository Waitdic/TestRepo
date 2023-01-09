namespace iVectorOne.SDK.V2.TransferPrecancel
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using iVectorOne.Services.Transfer;

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