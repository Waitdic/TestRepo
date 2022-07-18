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
    public class SupplierListHandler : IRequestHandler<SupplierListRequest, SupplierListResponse>
    {
        private readonly ConfigContext _context;
        private readonly IMapper _mapper;

        public SupplierListHandler(ConfigContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        Task<SupplierListResponse> IRequestHandler<SupplierListRequest, SupplierListResponse>.Handle(SupplierListRequest request, CancellationToken cancellationToken)
        {
            var warnings = new List<string>();
            bool success;
            var suppliers = _context.Suppliers;
            List<SupplierListItemDTO> supplierList = new List<SupplierListItemDTO>();

            if (suppliers != null)
            {
                supplierList = _mapper.Map<List<SupplierListItemDTO>>(suppliers.ToList());
                success = true;
            }
            else
            {
                success = false;
                warnings.Add(Warnings.ConfigWarnings.NoSuppliersWarning);
            }

            return Task.FromResult(new SupplierListResponse
            {
                Success = success,
                Warnings = warnings,
                Suppliers = supplierList
            });
        }
    }
}
