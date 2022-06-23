using AutoMapper;
using iVectorOne_Admin_Api.Config.Context;
using iVectorOne_Admin_Api.Config.Models;
using iVectorOne_Admin_Api.Config.Requests;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iVectorOne_Admin_Api.Config.Handlers
{
    public class SubscriptionHandler : IRequestHandler<SubscriptionRequest, SubscriptionResponse>
    {
        private ConfigContext _context;
        private IMapper _mapper;

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
                subscription = tenant.Subscriptions.Where(s => s.SubscriptionId == request.SubscriptionId).FirstOrDefault();
                if (subscription != null)
                {
                    subDTO = _mapper.Map<SubscriptionDTO>(subscription);
                    success = true;
                }
                else
                {
                    warnings.Add("Could not find a subscription with a matching id for the specified tenant");
                }
            }
            else
            {
                warnings.Add("Could not find a tenant with a matching key");
            }

            return Task.FromResult(new SubscriptionResponse() { Subscription = subDTO, Warnings = warnings, Success = success });
        }
    }
}
