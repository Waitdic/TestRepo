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

            var subscription = _context.Subscriptions.Where(s => s.TenantId == request.TenantId && s.SubscriptionId == request.SubscriptionId)
                                            .Include(s => s.SupplierSubscriptionAttributes.Where(sa => sa.SupplierAttribute.SupplierId == request.SupplierId))
                                                .ThenInclude(sa => sa.SupplierAttribute).ThenInclude(sa => sa.Attribute).FirstOrDefault();

            var configurations = new List<ConfigurationDTO>();

            foreach (var item in subscription.SupplierSubscriptionAttributes)
            {
                var configItem = new ConfigurationDTO()
                {
                    SupplierSubscriptionAttributeID = item.SupplierSubscriptionAttributeId,
                    Name = item.SupplierAttribute.Attribute.AttributeName,
                    Value = item.Value,
                    DefaultValue = item.SupplierAttribute.Attribute.DefaultValue,
                };

                Configuration config = default;
                if (item.SupplierAttribute.Attribute.Schema != null)
                {
                    config = JsonSerializer.Deserialize<Configuration>(item.SupplierAttribute.Attribute.Schema);
                }
                if (config != null)
                {
                    configItem.Type =  config.type.ToSafeEnum<ConfigurationType>().Value;
                    configItem.Key = config.key != null ? config.key : "";
                    configItem.Order = config.order.HasValue ? config.order.Value : 999;
                    configItem.Description = config.description != null ? config.description : "";
                    configItem.Maximum = config.maximum;
                    configItem.MaxLength = config.maxLength;
                    configItem.Minimum = config.minimum;
                    configItem.MinLength = config.minLength;
                    configItem.Required = config.required;
                    if (config.dropdownOptions.Count() > 0)
                    {
                        configItem.DropDownOptions = new List<DropDownOption>(config.dropdownOptions.Select(o => new DropDownOption() { ID = o.id, Name = o.name }));
                    }
                }
                configurations.Add(configItem);
            }

            return Task.FromResult(new SupplierResponse { Configurations = configurations });
        }
    }
}
