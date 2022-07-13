using AutoMapper;
using iVectorOne_Admin_Api.Config.Context;
using iVectorOne_Admin_Api.Config.Models;
using iVectorOne_Admin_Api.Config.Requests;
using iVectorOne_Admin_Api.Config.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iVectorOne_Admin_Api.Config.Handlers
{
    public class SubscriptionHandler : IRequestHandler<SubscriptionRequest, SubscriptionResponse>
    {
        private readonly ConfigContext _context;
        private readonly IMapper _mapper;

        public SubscriptionHandler(ConfigContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        Task<SubscriptionResponse> IRequestHandler<SubscriptionRequest, SubscriptionResponse>.Handle(SubscriptionRequest request, CancellationToken cancellationToken)
        {
            var tenant = _context.Tenants.Where(t => t.TenantId == request.TenantId).Include(t => t.Subscriptions).FirstOrDefault();
            Subscription? subscription = null;
            SubscriptionDTO? subDTO= new SubscriptionDTO();
            var warnings = new List<string>();
            bool success = false;

            if (tenant != null)
            {
                subscription = tenant.Subscriptions.FirstOrDefault(s => s.SubscriptionId == request.SubscriptionId);
                if (subscription != null)
                {
                    subDTO = _mapper.Map<SubscriptionDTO>(subscription);
                    success = true;
                }
                else
                {
                    warnings.Add(Warnings.ConfigWarnings.NoSubscriptionWarning);
                }
            }
            else
            {
                warnings.Add(Warnings.ConfigWarnings.NoTenantWarning);
            }

            return Task.FromResult(new SubscriptionResponse() { Subscription = subDTO, Warnings = warnings, Success = success });
        }
    }
}
