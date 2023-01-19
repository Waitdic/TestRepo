namespace iVectorOne.SDK.V2.ExtraContent
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using iVectorOne.Services;

    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly IExtraContentService _extraContentService;

        public Handler(IExtraContentService extraContentService)
        {
            _extraContentService = extraContentService;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            return await _extraContentService.GetAllExtras(request);
        }
    }
}