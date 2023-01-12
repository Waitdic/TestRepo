﻿namespace iVectorOne.SDK.V2.TransferPrebook
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using iVectorOne.Services.Transfer;

    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly IPrebookService _prebookService;

        public Handler(IPrebookService prebookService)
        {
            _prebookService = prebookService;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            return await _prebookService.PrebookAsync(request);
        }
    }
}