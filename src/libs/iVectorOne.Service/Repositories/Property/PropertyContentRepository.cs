namespace iVectorOne.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Data;
    using Intuitive.Helpers.Extensions;
    using Intuitive.TPImport.Models;
    using iVectorOne.Models;
    using iVectorOne.Models.Property;
    using Content = SDK.V2.PropertyContent;

    /// <summary>Repository that retrieves property content from the database</summary>
    public class PropertyContentRepository : IPropertyContentRepository
    {
        private readonly ISql _sql;

        public PropertyContentRepository(ISql sql)
        {
            _sql = Ensure.IsNotNull(sql, nameof(sql));
        }

        /// <inheritdoc/>
        public async Task<List<int>> GetPropertyIDsAsync(DateTime lastModified, string suppliers, Account account)
        {
            return (await _sql.ReadAllAsync<int>(
                    "Property_List",
                    new CommandSettings()
                        .IsStoredProcedure()
                        .WithParameters(new
                        {
                            lastModified = lastModified,
                            suppliers = suppliers,
                            accountId = account.AccountID
                        })))
                .ToList();
        }

        /// <inheritdoc/>
        public async Task<Content.Response> GetPropertyContentAsync(List<int> propertyIDs, string suppliers, Account account, bool includeRoomTypes)
        {
            var centralPropertyIDs = new List<int>();

            var contents = await _sql.ReadAllAsync<PropertyContentItem>(
                    "Property_GetPropertyContent",
                    new CommandSettings()
                        .IsStoredProcedure()
                        .WithParameters(new
                        {
                            centralPropertyIDs = string.Join(",", propertyIDs),
                            suppliers = suppliers,
                            accountId = account.AccountID
                        }));

            var response = new Content.Response();

            foreach (var content in contents)
            {
                var details = Newtonsoft.Json.JsonConvert.DeserializeObject<PropertyDetails>(content.PropertyDetails);

                var property = response.Properties.FirstOrDefault(p => p.CentralPropertyID == content.CentralPropertyID);

                if (property == null)
                {
                    property = new Content.Property()
                    {
                        CentralPropertyID = content.CentralPropertyID,
                        TTICode = content.TTICode
                    };
                    response.Properties.Add(property);
                }

                if (details is not null)
                {
                    var supplierContent = new Content.SupplierContent()
                    {
                        PropertyName = content.Name,
                        SupplierPropertyReference = content.TPKey,
                        TPKey = details.TPKey!,
                        Address = new Content.Address()
                        {
                            AddressLine1 = details.Address1!,
                            AddressLine2 = details.Address2!,
                            TownOrCity = details.TownCity!,
                            CountyOrState = details.County!,
                            PostCodeOrZip = details.Postcode!,
                            Telephone = details.Telephone!
                        },
                        Geography = new Content.Geography()
                        {
                            Country = details.Geography!.Country!,
                            Region = details.Geography.Region!,
                            Resort = details.Geography.Resort!,
                            Latitude = details.Latitude!.ToSafeDecimal(),
                            Longitude = details.Longitude!.ToSafeDecimal(),
                            Code = details.Geography.Code!
                        },
                        Rating = details.Rating!,
                        Supplier = details.Source!,
                        Description = details.Description!,
                        HotelPolicy = details.HotelPolicy!,
                        ContentVariables = GetContentVariables(details)
                    };

                    if (details.Facilities.Any())
                    {
                        supplierContent.Facilities = details.Facilities.Select(f => f.Description).ToList()!;
                    }

                    if (details.Images.Any())
                    {
                        supplierContent.MainImageURL = details.Images.OrderBy(i => i.Sequence).Select(i => i.URL != null ? i.URL : i.SourceURL).FirstOrDefault()!;

                        if (details.Images.Count > 1)
                        {
                            supplierContent.Images = details.Images.Skip(1).Select(i => i.URL != null ? i.URL : i.SourceURL).ToList()!;
                        }

                        supplierContent.FullImages.AddRange(details.Images.Select(i => new Content.Image(i.SourceURL, i.URL, i.Title, i.Sequence, i.Height, i.Width)));
                    }

                    if (includeRoomTypes && details.RoomTypes.Any())
                    {
                        supplierContent.RoomTypes.AddRange(details.RoomTypes.Select(rt => new Content.RoomType(rt.Name, rt.Code, rt.RateName, rt.RateId, rt.Facilities.Select(f => f.Description).ToList()!)));
                    }

                    if (!property.SupplierContent.Any(sc => sc.TPKey == supplierContent.TPKey && sc.Supplier == supplierContent.Supplier))
                    {
                        property.SupplierContent.Add(supplierContent);
                    }
                }
            }

            return response;
        }

        public class PropertyContentItem
        {
            public string Name { get; set; } = string.Empty;
            public string TPKey { get; set; } = string.Empty;
            public string TTICode { get; set; } = string.Empty;
            public string Source { get; set; } = string.Empty;
            public string PropertyDetails { get; set; } = string.Empty;
            public int CentralPropertyID { get; set; }
            public string Country { get; set; } = string.Empty;
            public string Region { get; set; } = string.Empty;
            public string Resort { get; set; } = string.Empty;
        }

        /// <inheritdoc/>
        public async Task<PropertyContent> GetContentforPropertyAsync(int propertyId, Account account, string supplierBookingReference)
        {
            return await _sql.ReadSingleAsync<PropertyContent>(
                    "Property_SingleProperty",
                    new CommandSettings()
                        .IsStoredProcedure()
                        .WithParameters(new
                        {
                            propertyId = propertyId,
                            accountId = account.AccountID,
                            supplierBookingReference = supplierBookingReference
                        }));
        }

        private List<Content.ContentVariable> GetContentVariables(PropertyDetails propertyDetails)
        {
            var contentVariables = new List<Content.ContentVariable>();

            foreach (var contentVariable in propertyDetails.ContentVariables)
            {
                var cv = new Content.ContentVariable(
                    (Content.ContentVariableEnum)contentVariable.ParentGroupKey,
                    (Content.ContentVariableEnum)contentVariable.Group,
                    !string.IsNullOrEmpty(contentVariable.SubGroup) ? contentVariable.SubGroup : "",
                    contentVariable.Key!,
                    contentVariable.Value!);

                if (contentVariable.Units != 0)
                {
                    cv.Units = (Content.ContentVariableEnum)contentVariable.Units;
                }

                contentVariables.Add(cv);
            }

            return contentVariables;
        }
    }
}