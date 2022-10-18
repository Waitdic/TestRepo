using iVectorOne_Admin_Api.Features.Shared;

namespace iVectorOne_Admin_Api.Features.V1.Users.Create
{
    public class Handler : IRequestHandler<Request, ResponseBase>
    {
        private readonly ConfigContext _context;

        public Handler(ConfigContext context)
        {
            _context = context;
        }

        public async Task<ResponseBase> Handle(Request request, CancellationToken cancellationToken)
        {
            var response = new ResponseBase();

            var existingUser = await _context.Users
                .Where(u => u.Key == request.Subject)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            if (existingUser != null)
            {
                response.BadRequest("User already exists.");
                return response;
            }

            var user = new User
            {
                UserName = request.UserName,
                Key = request.Subject,
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);

            response.Ok(new ResponseModel { Success = true, UserId = user.UserId });

            return response;
        }
    }
}