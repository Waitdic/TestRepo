using AutoMapper;
using iVectorOne_Admin_Api.Config.Context;
using iVectorOne_Admin_Api.Config.Models;
using iVectorOne_Admin_Api.Config.Requests;
using iVectorOne_Admin_Api.Config.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iVectorOne_Admin_Api.Config.Handlers
{
    public class UserHandler : IRequestHandler<UserRequest, UserResponse>
    {
        private ConfigContext _context;
        private IMapper _mapper;

        public UserHandler(ConfigContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        Task<UserResponse> IRequestHandler<UserRequest, UserResponse>.Handle(UserRequest request, CancellationToken cancellationToken)
        {
            List<TenantDTO> tenantDTOs = new List<TenantDTO>();
            var warnings = new List<string>();
            var user = _context.Users.Where(t => t.Key == request.Key)
                                        .Include(u => u.UserTenants.Where(t=> t.Tenant.Status == "active"))
                                            .ThenInclude(t => t.Tenant).FirstOrDefault();
            var userName = "";
            bool success = false;

            if (user != null)
            {
                userName = user.UserName;
                tenantDTOs = _mapper.Map<List<TenantDTO>>(user.UserTenants.Select(x => x.Tenant));
                success = true;
            }
            else
            {
                warnings.Add(Warnings.ConfigWarnings.NoUserWarning);
            }

            return Task.FromResult(new UserResponse { UserName = userName, Tenants = tenantDTOs, Warnings = warnings, Success = success });
        }
    }
}
