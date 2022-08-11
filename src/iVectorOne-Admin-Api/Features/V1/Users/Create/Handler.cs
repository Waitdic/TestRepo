namespace iVectorOne_Admin_Api.Features.V1.Users.Create
{
    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly ConfigContext _context;
        private readonly IMapper _mapper;

        public Handler(ConfigContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var response = new Response();

            var users = await _context.Users.Where(u => u.Key == request.Subject).FirstOrDefaultAsync();

            if (users != null)
            {
                response.BadRequest();
                return response;
            }

            var user = new User
            {
                UserName = request.UserName,
                Key = request.Subject,
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            response.Default(new ResponseModel { Success = true, UserId = user.UserId });

            return response;
        }
    }
}
