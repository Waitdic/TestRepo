namespace iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Create
{
    using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
    using System.Security.Cryptography;
    using System.Text;

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

            var tenant = _context.Tenants.Where(t => t.TenantId == request.TenantId).Include(t => t.Accounts).FirstOrDefault();

            if (tenant is null)
            {
                response.NotFound();
                return response;
            }

            string GeneratePassword(int length)
            {
                const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
                StringBuilder res = new StringBuilder();
                using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
                {
                    byte[] uintBuffer = new byte[sizeof(uint)];

                    while (length-- > 0)
                    {
                        rng.GetBytes(uintBuffer);
                        uint num = BitConverter.ToUInt32(uintBuffer, 0);
                        res.Append(valid[(int)(num % (uint)valid.Length)]);
                    }
                }

                return res.ToString();
              }

            Account CreateAccount(bool isLive)
            {
                var account = _mapper.Map<AccountDto, Account>(request.Account);
                var environment = isLive ? "Live" : "Test";

                account.TenantId = request.TenantId;
                account.DummyResponses = false;
                account.LogMainSearchError = true;
                account.Environment = environment.ToLower();
                account.Login = $"{account.Login.Replace(' ', '_')}_{environment}";
                account.Password = GeneratePassword(20);
                return account;
            }

            var accounts = new[]
            {
                CreateAccount(true),
                CreateAccount(false)
            };

            if (tenant.Accounts.Any(s => accounts.Any(n => n.Login == s.Login)))
            {
                response.Warnings.Add("An entry with the same name already exists for this Tenant");

                response.BadRequest();
                return response;
            }

            tenant.Accounts.AddRange(accounts);
            await _context.SaveChangesAsync(cancellationToken);

            response.Default(new ResponseModelBase { Success = true });
            return response;
        }
    }
}
