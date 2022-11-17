using Intuitive.Helpers.Extensions;
using iVectorOne_Admin_Api.Features.Shared;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;

namespace iVectorOne_Admin_Api.Features.V1.Suppliers.Info
{
    public class Handler : IRequestHandler<Request, ResponseBase>
    {
        private readonly AdminContext _context;

        public Handler(AdminContext context)
        {
            _context = context;
        }

        public async Task<ResponseBase> Handle(Request request, CancellationToken cancellationToken)
        {
            var response = new ResponseBase();
            var configurations = new List<ConfigurationDTO>();

            var supplier = await _context.Suppliers
                .AsNoTracking()
                .Include(s => s.SupplierAttributes)
                .ThenInclude(sa => sa.Attribute)
                .FirstOrDefaultAsync(s => s.SupplierId == request.SupplierID, cancellationToken: cancellationToken);

            if (supplier == null)
            {
                response.NotFound("User not found.");
                return response;
            }

            if (supplier.SupplierAttributes != null && supplier.SupplierAttributes.Any())
            {
                //Legacy = true == field schema is defined in JSON
                //Legacy = false == field schema is defined in explicit fields
                configurations = MapLegacyConfigurations(supplier.SupplierAttributes.Where(x => x.Attribute.IsLegacyFormat == true).ToList());
                //configurations = MapConfigurations(supplier.SupplierAttributes.Where(x => x.Attribute.IsLegacyFormat == true).ToList());
            }

            response.Ok(new ResponseModel
            {
                Success = true,
                SupplierName = supplier.SupplierName,
                Configurations = configurations
            });

            return response;
        }

        #region Helpers
        internal static List<ConfigurationDTO> MapConfigurations(List<SupplierAttribute> supplierAttributes)
        {
            var configurations = new List<ConfigurationDTO>();

            return configurations;
        }

        internal static List<ConfigurationDTO> MapLegacyConfigurations(List<SupplierAttribute> supplierAttributes)
        {
            var configurations = new List<ConfigurationDTO>();

            foreach (var item in supplierAttributes)
            {
                var configuration = new ConfigurationDTO
                {
                    SupplierAttributeID = item.SupplierAttributeId,
                    Value = String.IsNullOrEmpty(item.Attribute.DefaultValue) ? "" : item.Attribute.DefaultValue,
                    DefaultValue = item.Attribute.DefaultValue,
                };

                if (item.Attribute.Schema != null)
                {
                    AttributeSchema config = JsonSerializer
                        .Deserialize<AttributeSchema>(item.Attribute.Schema,
                            new JsonSerializerOptions()
                            {
                                PropertyNameCaseInsensitive = true,
                            })!;

                    if (config != null)
                    {
                        configuration.Name = config.Name ?? item.Attribute.AttributeName;
                        configuration.Type = config.Type.ToSafeEnum<ConfigurationType>().Value;
                        configuration.Key = config.Key ?? "";
                        configuration.Order = config.Order.HasValue ? config.Order.Value : 999;
                        configuration.Description = config.Description ?? "";
                        configuration.Maximum = config.Maximum;
                        configuration.MaxLength = config.MaxLength;
                        configuration.Minimum = config.Minimum;
                        configuration.MinLength = config.MinLength;
                        configuration.Required = config.Required;
                        if (config.DropDownOptions.Count > 0)
                        {
                            configuration.DropDownOptions = config.DropDownOptions;
                        }
                    }
                }

                configurations.Add(configuration);
            }

            return configurations;
        }

        #endregion
    }

    #region Internals

    internal record AttributeSchema
    {
        public string? Name { get; set; }

        public string? Key { get; set; }

        public int? Order { get; set; }

        public bool? Required { get; set; }

        public string? Type { get; set; }

        public string? Description { get; set; }

        public int? Minimum { get; set; }

        public int? Maximum { get; set; }

        public int? MinLength { get; set; }

        public int? MaxLength { get; set; }

        public List<DropDownOptionDTO> DropDownOptions { get; set; } = new List<DropDownOptionDTO>();
    }

    #endregion
}