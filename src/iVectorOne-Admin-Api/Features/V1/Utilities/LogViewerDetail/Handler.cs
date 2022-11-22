using FluentValidation;
using FluentValidation.Results;
using iVectorOne_Admin_Api.Adaptors;
using iVectorOne_Admin_Api.Data.Models.Dashboard;
using iVectorOne_Admin_Api.Features.Shared;
using iVectorOne_Admin_Api.Features.V1.Dashboard.Info;
using Microsoft.Identity.Client;

namespace iVectorOne_Admin_Api.Features.V1.Utilities.LogViewerDetail
{
    public class Handler : IRequestHandler<Request, ResponseBase>
    {
        private readonly AdminContext _context;
        private readonly IMapper _mapper;

        public Handler(AdminContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ResponseBase> Handle(Request request, CancellationToken cancellationToken)
        {
            var response = new ResponseBase();

            var account = _context.Accounts.Where(a => a.AccountId == request.AccountID)
                .AsNoTracking()
                .FirstOrDefault();

            var queryText =
                $"SELECT RequestLog, ResponseLog" +
                $" FROM SupplierAPILog" +
                $" WHERE SupplierAPILogID = {request.Key}";

            var logDetail = await _context.LogDetails
             .FromSqlRaw(queryText)
             .AsNoTracking()
             .ToListAsync(cancellationToken: cancellationToken);

            var LogDetails = logDetail.Select(x => new LogDetail
            {
                RequestLog = x.RequestLog,
                ResponseLog = x.ResponseLog
            }).ToList();

            response.Ok(new ResponseModel() { Success = true, LogDetails = LogDetails});
            return response;
        }
    }
}
