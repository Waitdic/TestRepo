﻿using AutoMapper;
using iVectorOne_Admin_Api.Config.Context;
using iVectorOne_Admin_Api.Config.Models;
using iVectorOne_Admin_Api.Config.Requests;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iVectorOne_Admin_Api.Config.Handlers
{
    public class SupplierSubscriptionHandler : IRequestHandler<SupplierSubscriptionRequest, SupplierSubscriptionResponse>
    {
        private ConfigContext _context;
        private IMapper _mapper;

        public SupplierSubscriptionHandler(ConfigContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        Task<SupplierSubscriptionResponse> IRequestHandler<SupplierSubscriptionRequest, SupplierSubscriptionResponse>.Handle(SupplierSubscriptionRequest request, CancellationToken cancellationToken)
        {
            var tenant = _context.Tenants.Where(t => t.TenantId == request.TenantId).Include(t => t.Subscriptions)
                                                                                        .ThenInclude(s => s.SupplierSubscriptions)
                                                                                            .ThenInclude(s => s.Supplier).FirstOrDefault();
            Subscription? subscription = null;
            SupplierSubscriptionDTO? suppliers = new SupplierSubscriptionDTO();
            var warnings = new List<string>();
            bool success = false;
            if (tenant != null)
            {
                subscription = tenant.Subscriptions.Where(s => s.SubscriptionId == request.SubscriptionId).FirstOrDefault();
                if (subscription != null)
                {
                    suppliers = _mapper.Map<SupplierSubscriptionDTO>(subscription);
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

            return Task.FromResult(new SupplierSubscriptionResponse() { SupplierSubscription = suppliers, Warnings = warnings, Success = success });
        }
    }
}
