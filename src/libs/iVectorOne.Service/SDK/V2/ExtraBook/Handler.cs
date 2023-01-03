namespace iVectorOne.SDK.V2.ExtraBook
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using iVectorOne.Services.Extra;

    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly IBookService _bookService;

        public Handler(IBookService bookService)
        {
            _bookService = bookService;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            return await _bookService.BookAsync(request);
        }
    }
}