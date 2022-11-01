using iVectorOne_Admin_Api.Features.Shared;

namespace iVectorOne_Admin_Api.Features.V1.Users.List
{
    public class Handler : IRequestHandler<Request, ResponseBase>
    {
        private readonly AdminContext _context;
        private readonly IMapper _mapper;

        public Handler(AdminContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ResponseBase> Handle(Request request, CancellationToken cancellationToken)
        {
            var response = new ResponseBase();

            var users = await _mapper.ProjectTo<UserDto>(_context.Users)
                .OrderBy(t => t.UserName)
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken);

            response.Ok(new ResponseModel { Success = true, Users = users });

            return response;
        }
    }
}