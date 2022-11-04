﻿using FluentValidation;
using FluentValidation.Results;
using iVectorOne_Admin_Api.Adaptors;
using iVectorOne_Admin_Api.Features.Shared;
using iVectorOne_Admin_Api.Features.V1.Dashboard.Info;

namespace iVectorOne_Admin_Api.Features.V1.Utilities.LogViewer
{
    public class Handler : IRequestHandler<Request, ResponseBase>
    {
        private readonly AdminContext _context;
        private readonly IValidator<Request> _validator;

        public Handler(AdminContext context, IValidator<Request> validator)
        {
            _context = context;
            _validator = validator;
        }

        public async Task<ResponseBase> Handle(Request request, CancellationToken cancellationToken)
        {
            var response = new ResponseBase();

            var account = _context.Accounts.Where(a => a.AccountId == request.AccountID)
                .Include(a => a.AccountSuppliers)
                .ThenInclude(a => a.Supplier)
                .AsNoTracking()
                .FirstOrDefault();

            if (account == null)
            {
                response.NotFound("Account not found.");
                return response;
            }

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                response.BadRequest("One or more parameters contained invalid values.", validationResult.ToDictionary());
                return response;
            }

            var logEntries = new List<LogEntry>();
            for (int i = 0; i < 30; i++)
            {
                logEntries.Add(new LogEntry
                {
                    Environment = $"Environment-{i}",
                    Timestamp = $"Timestamp-{i}",
                    SupplierName = $"SupplierName-{i}",
                    Type = $"Type-{i}",
                    ResponseTime = $"ResponseTime-{i}",
                    SupplierReference = $"SupplierReference-{i}",
                    LeadGuest = $"LeadGuest-{i}"
                });
            }

            response.Ok(new ResponseModel() { Success = true, HasMoreResults = false, LogEntries = logEntries });
            return response;
        }
    }
}
