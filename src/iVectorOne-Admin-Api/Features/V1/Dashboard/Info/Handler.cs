using iVectorOne_Admin_Api.Features.Shared;

namespace iVectorOne_Admin_Api.Features.V1.Dashboard.Info
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
            Random random = new();

            //Check the account that was supplied is valid
            var account = await _context.Accounts
                .Where(x => x.TenantId == request.TenantId && x.AccountId == request.AccountId)
                .Include(x => x.AccountSuppliers)
                .ThenInclude(x => x.Supplier)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (account == null)
            {
                response.NotFound();
                return response;
            }

            var currentHour = DateTime.Now.Hour;

            var queryText = $"Portal_SearchesByHour {request.AccountId}";
            var searchesByHourData = await _context.SearchesByHour
                .FromSqlRaw(queryText)
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken);

            int currentWeekTotal = 0;
            int previousWeekTotal = 0;
            var searchesByHour = searchesByHourData.Select(x => new Node
            {
                Time = x.Hour,
                CurrentTotal = currentHour >= x.Hour ? currentWeekTotal += x.CurrentWeek : 0,
                PreviousTotal = previousWeekTotal += x.PreviousWeek,
            }).ToList();

            queryText = $"Portal_BookingsByHour {request.AccountId}";
            var bookingsByHourData = await _context.SearchesByHour
                .FromSqlRaw(queryText)
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken);

            currentWeekTotal = 0;
            previousWeekTotal = 0;
            var bookingsByHour = bookingsByHourData.Select(x => new Node
            {
                Time = x.Hour,
                CurrentTotal = currentHour >= x.Hour ? currentWeekTotal += x.CurrentWeek : 0,
                PreviousTotal = previousWeekTotal += x.PreviousWeek,
            }).ToList();

            queryText = $"Portal_DashboardSummary {request.AccountId}";
            var dashboardSummaryData = await _context.DashboardSummary
                .FromSqlRaw(queryText)
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken);

            var dashboardSummary = dashboardSummaryData.Select(x => new Summary
            {
                Name = x.Row,
                Bookings = new Bookings
                {
                    Total = string.Format("{0:#,0}", x.BookingTotal),
                    Value = $"£{string.Format("{0:#,0}", x.BookingValue)}"
                },
                Prebook = new Prebook
                {
                    Total = string.Format("{0:#,0}", x.PrebookTotal),
                    Successful = $"{x.PrebookSuccessfulPrecent:n1}%",
                },
                Searches = new Searches
                {
                    Total = string.Format("{0:#,0}", x.SearchTotal),
                    Successful = $"{x.SearchSuccessfulPrecent:n1}%",
                    Avg_Resp = $"{string.Format("{0:#,0}", x.AverageSearchTime)}ms",
                },
                S2B = x.S2B.ToString("n0")
            }).ToList();

            var supplierSummary = new List<Supplier>();

            for (int i = -6; i < 1; i++)
            {
                var queryDate = DateTime.Now.AddDays(i);

                queryText = $"Portal_SupplierSummary {request.AccountId}, '{queryDate:yyyy-MM-dd}'";
                var supplierSummaryData = await _context.SupplierSummary
                    .FromSqlRaw(queryText)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken: cancellationToken);

                supplierSummary.AddRange(supplierSummaryData.Select(x => new Supplier
                {
                    Name = x.SupplierName,
                    QueryDate = queryDate.ToString("yyyy-MM-dd"),
                    Bookings = new SupplierBookings
                    {
                        Total = string.Format("{0:#,0}", x.BookTotal),
                        Successful = $"{x.BookSuccessfulPrecent:n1}%",
                    },
                    Prebook = new SupplierPrebook
                    {
                        Total = string.Format("{0:#,0}", x.PrebookTotal),
                        Successful = $"{x.PrebookSuccessfulPrecent:n1}%",
                    },
                    Searches = new SupplierSearches
                    {
                        Total = string.Format("{0:#,0}", x.SearchTotal),
                        Successful = $"{x.SearchSuccessfulPrecent:n1}%",
                        Avg_Resp = $"{string.Format("{0:#,0}", x.AverageSearchTime)}ms",
                    },
                    S2B = x.S2B.ToString("n0")
                }).ToList());
            }

            var responseModel = new ResponseModel()
            {
                BookingsByHour = bookingsByHour,
                SearchesByHour = searchesByHour,
                Summary = dashboardSummary,
                Supplier = supplierSummary,
            };

            response.Ok(responseModel);
            return response;

        }
    }

}
