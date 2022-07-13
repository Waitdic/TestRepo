using AutoMapper;
using iVectorOne_Admin_Api.Config.Context;
using iVectorOne_Admin_Api.Config.Models;
using iVectorOne_Admin_Api.Config.Requests;
using iVectorOne_Admin_Api.Config.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iVectorOne_Admin_Api.Config.Handlers
{
    public class TenantHandler : IRequestHandler<TenantRequest, TenantResponse>
    {
        private readonly ConfigContext _context;
        private readonly IMapper _mapper;

        public TenantHandler(ConfigContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        Task<TenantResponse> IRequestHandler<TenantRequest, TenantResponse>.Handle(TenantRequest request, CancellationToken cancellationToken)
        {
            List<SubscriptionDTO> subscriptionDTOs = new List<SubscriptionDTO>();
            var warnings = new List<string>();
            var tenant = _context.Tenants.Where(t => t.TenantId == request.TenantId).Include(t => t.Subscriptions).FirstOrDefault();
            bool success = false;
            int tenantId = request.TenantId;
            string tenantName = string.Empty;

            if (tenant != null)
            {
                subscriptionDTOs = _mapper.Map<List<SubscriptionDTO>>(tenant.Subscriptions);
                success = true;
                tenantId = tenant.TenantId;
                tenantName = tenant.CompanyName;
            }
            else
            {
                warnings.Add(Warnings.ConfigWarnings.NoTenantWarning);
            }

            return Task.FromResult(new TenantResponse
            {
                TenantId = tenantId,
                TenantName = tenantName,
                Subscriptions = subscriptionDTOs,
                Warnings = warnings,
                Success = success
            });
        }
    }
}
