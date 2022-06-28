using AutoMapper;
using Intuitive.Helpers.Extensions;
using iVectorOne_Admin_Api.Config.Context;
using iVectorOne_Admin_Api.Config.Models;
using iVectorOne_Admin_Api.Config.Requests;
using iVectorOne_Admin_Api.Config.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace iVectorOne_Admin_Api.Config.Handlers
{
    public class SupplierHandler : IRequestHandler<SupplierRequest, SupplierResponse>
    {
        private ConfigContext _context;
        private IMapper _mapper;

        public SupplierHandler(ConfigContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        Task<SupplierResponse> IRequestHandler<SupplierRequest, SupplierResponse>.Handle(SupplierRequest request, CancellationToken cancellationToken)
        {
            var configurations = new List<ConfigurationDTO>();
            var warnings = new List<string>();
            var supplierName = string.Empty;
            var success = false;
            var subscription = _context.Subscriptions.Where(s => s.TenantId == request.TenantId && s.SubscriptionId == request.SubscriptionId)
                                                        .Include(s => s.SupplierSubscriptionAttributes
                                                            .Where(ssa => ssa.SupplierAttribute.SupplierId == request.SupplierId))
                                                        .ThenInclude(ssa => ssa.SupplierAttribute)
                                                        .ThenInclude(sa => sa.Attribute).FirstOrDefault();
            if (subscription != null)
            {
                var supplier = _context.Suppliers.FirstOrDefault(s => s.SupplierId == request.SupplierId);
                supplierName = supplier?.SupplierName;
                if (subscription.SupplierSubscriptionAttributes != null && subscription.SupplierSubscriptionAttributes.Count() > 0)
                {
                    success = true;
                    foreach (var item in subscription.SupplierSubscriptionAttributes)
                    {
                        var configItem = new ConfigurationDTO()
                        {
                            SupplierSubscriptionAttributeID = item.SupplierSubscriptionAttributeId,
                            Name = item.SupplierAttribute.Attribute.AttributeName,
                            Value = item.Value,
                            DefaultValue = item.SupplierAttribute.Attribute.DefaultValue,
                        };                        
                        
                        AttributeSchema config = default!;
                        if (item.SupplierAttribute.Attribute.Schema != null)
                        {
                            config = JsonSerializer.Deserialize<AttributeSchema>(
                                item.SupplierAttribute.Attribute.Schema,
                                new JsonSerializerOptions()
                                {
                                    PropertyNameCaseInsensitive = true,
                                })!;
                        }
                        if (config != null)
                        {
                            configItem.Type = config.Type.ToSafeEnum<ConfigurationType>().Value;
                            configItem.Key = config.Key != null ? config.Key : "";
                            configItem.Order = config.Order.HasValue ? config.Order.Value : 999;
                            configItem.Description = config.Description != null ? config.Description : "";
                            configItem.Maximum = config.Maximum;
                            configItem.MaxLength = config.MaxLength;
                            configItem.Minimum = config.Minimum;
                            configItem.MinLength = config.MinLength;
                            configItem.Required = config.Required;
                            if (config.DropDownOptions.Count() > 0)
                            {
                                configItem.DropDownOptions = config.DropDownOptions;
                            }
                        }
                        configurations.Add(configItem);
                    }
                }
                else
                {
                    success = false;
                    warnings.Add("No Configurations found for subscription supplier combo");
                }
            }

            return Task.FromResult(new SupplierResponse
            {
                Success = success,
                Configurations = configurations,
                Warnings = warnings,
                SupplierID = request.SupplierId,
                SupplierName = supplierName
            });
        }
    }
}
