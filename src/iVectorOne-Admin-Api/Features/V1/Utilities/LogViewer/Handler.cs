using FluentValidation;
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
        private readonly IMapper _mapper;

        public Handler(AdminContext context, IValidator<Request> validator, IMapper mapper)
        {
            _context = context;
            _validator = validator;
            _mapper = mapper;
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

            var queryText = $"SELECT T1.RequestDateTime,T3.SupplierName,T1.Type," +
                $"T1.Successful,T1.ResponseTime,T2.SupplierBookingReference,T2.LeadGuestName" +
                $" FROM SupplierAPILog T1" +
                $" INNER JOIN Supplier T3 ON T3.SupplierID = T1.SupplierID" +
                $" LEFT OUTER JOIN Booking T2 on T2.BookingID = T1.BookingID" +
                $" WHERE T1.AccountID = {request.AccountID}";

            if (request.SupplierID != 0)
            {
                queryText += $" AND T1.SupplierId={request.SupplierID}";
            }

            if (request.Type.ToLower() != "all")
            {
                queryText += $" AND T1.Type='{request.Type}'";
            }

            switch (request.Status.ToLower())
            {
                case "successful":
                    queryText += $" AND T1.Successful=1";
                    break;
                case "unsuccessful":
                    queryText += $" AND T1.Successful=0";
                    break;
            }
            //if (request.Environment.ToLower() != "all")
            //{
            //    queryText += $" AND T1.Type='{request.Type}'";
            //}

            queryText += " ORDER BY T1.RequestDateTime DESC";

            var logEntries = await _context.LogEntries
             .FromSqlRaw(queryText)
             .AsNoTracking()
             .ToListAsync(cancellationToken: cancellationToken);

            var LogEntryList = _mapper.Map<List<LogEntry>>(logEntries);

            response.Ok(new ResponseModel() { Success = true, HasMoreResults = false, LogEntries = LogEntryList });
            return response;
        }
    }
}
