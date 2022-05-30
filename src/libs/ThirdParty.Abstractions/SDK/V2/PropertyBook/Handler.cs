namespace ThirdParty.SDK.V2.PropertyBook
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using ThirdParty.Services;

    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly IBookService _bookService;

        public Handler(IBookService bookService)
        {
            _bookService = bookService;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            return await _bookService.BookAsync(request, request.User);
        }
    }
}