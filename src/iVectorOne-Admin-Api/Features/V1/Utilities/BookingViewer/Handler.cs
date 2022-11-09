using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using iVectorOne_Admin_Api.Adaptors;
using iVectorOne_Admin_Api.Data.Models;
using iVectorOne_Admin_Api.Features.Shared;

namespace iVectorOne_Admin_Api.Features.V1.Utilities.BookingViewer
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

            var queryText = $"Portal_BookingSearch '{(request.Query!.EndsWith('%') ? request.Query : $"{request.Query}%")}', {request.AccountID}";
            var bookings = await _context.BookingLogs
             .FromSqlRaw(queryText)
             .AsNoTracking()
             .ToListAsync(cancellationToken: cancellationToken);

            var LogEntryList = bookings.Select(x => new LogEntry
            {
                SupplierApiLogId = x.SupplierApiLogId,
                SupplierName = x.SupplierName,
                LeadGuestName = x.LeadGuestName ?? "",
                Succesful = x.Success,
                ResponseTime = $"{string.Format("{0:#,0}", x.ResponseTime)}ms",
                ResponseTimeValue = x.ResponseTime,
                Type = x.Type,
                SupplierBookingReference = x.SupplierBookingReference ?? "",
                Timestamp = x.BookingDateTime.ToString("s", System.Globalization.CultureInfo.InvariantCulture)
            }).ToList();

            response.Ok(new ResponseModel() { Success = true, LogEntries = LogEntryList });
            return response;
        }
    }
}
