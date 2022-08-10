using AutoMapper;
using Intuitive.Helpers.Extensions;
using iVectorOne_Admin_Api.Config.Models;
using iVectorOne_Admin_Api.Config.Requests;
using iVectorOne_Admin_Api.Config.Responses;
using iVectorOne_Admin_Api.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace iVectorOne_Admin_Api.Config.Handlers
{
    public class SupplierAttributeHandler : IRequestHandler<SupplierAttributeRequest, SupplierAttributeResponse>
    {
        private readonly ConfigContext _context;

        public SupplierAttributeHandler(ConfigContext context)
        {
            _context = context;
        }

        Task<SupplierAttributeResponse> IRequestHandler<SupplierAttributeRequest, SupplierAttributeResponse>.Handle(SupplierAttributeRequest request, CancellationToken cancellationToken)
        {
            var configurations = new List<ConfigurationDTO>();
            var warnings = new List<string>();
            var supplierName = string.Empty;
            var success = false;
            var supplier = _context.Suppliers.Include(s => s.SupplierAttributes).ThenInclude(sa=> sa.Attribute).FirstOrDefault(s => s.SupplierId == request.SupplierID);
            if (supplier != null)
            {
                supplierName = supplier?.SupplierName;
                if (supplier.SupplierAttributes != null && supplier.SupplierAttributes.Any())
                {
                    success = true;
                    foreach (var item in supplier.SupplierAttributes)
                    {
                        var configItem = new ConfigurationDTO()
                        {
                            SupplierAttributeID = item.SupplierAttributeId,
                            Value = String.IsNullOrEmpty(item.Attribute.DefaultValue) ? "" : item.Attribute.DefaultValue,
                            DefaultValue = item.Attribute.DefaultValue,
                        };

                        AttributeSchema config = default!;
                        if (item.Attribute.Schema != null)
                        {
                            config = JsonSerializer.Deserialize<AttributeSchema>(
                                item.Attribute.Schema,
                                new JsonSerializerOptions()
                                {
                                    PropertyNameCaseInsensitive = true,
                                })!;
                        }
                        if (config != null)
                        {
                            configItem.Name = config.Name != null ? config.Name : item.Attribute.AttributeName;
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
                    warnings.Add(Warnings.ConfigWarnings.MultiNoSupplierAttributesWarning);
                }
            }
            else
            {
                success = false;
                warnings.Add(Warnings.ConfigWarnings.NoSupplierWarning);
            }

            return Task.FromResult(new SupplierAttributeResponse
            {
                Success = success,
                Configurations = configurations,
                Warnings = warnings,
                SupplierID = request.SupplierID,
                SupplierName = supplierName
            });
        }
    }
}
