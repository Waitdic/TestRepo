using iVectorOne_Admin_Api.Config.Models;

namespace iVectorOne_Admin_Api.Features.V1.Users.List
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

            var userRecords = await _context.Users
                .OrderBy(t => t.UserName)
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken);

            var users = _mapper.Map<List<UserDto>>(userRecords);

            response.Default(new ResponseModel { Success = true, Users = users });

            return response;
        }
    }
}
