﻿using iVectorOne_Admin_Api.Features.Shared;

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
                CurrentTotal = currentWeekTotal += x.CurrentWeek,
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
                CurrentTotal = currentWeekTotal += x.CurrentWeek,
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
                    Total = x.BookingTotal.ToString(),
                    Value = $"£ {x.BookingValue}",
                },
                Prebook = new Prebook
                {
                    Total = x.PrebookTotal.ToString(),
                    Successful = $"{x.PrebookSuccessfulPrecent:n1} %",
                },
                Searches = new Searches
                {
                    Total = x.SearchTotal.ToString(),
                    Successful = $"{x.SearchSuccessfulPrecent:n1} %",
                    Avg_Resp = $"{x.AverageSearchTime} ms",
                },
                S2B = x.S2B.ToString("n1")
            }).ToList();

            var supplierSummary = new List<Supplier>();

            for (int i = -4; i < 1; i++)
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
                        Total = x.BookTotal.ToString(),
                        Successful = $"{x.BookSuccessfulPrecent:n1} %",
                    },
                    Prebook = new SupplierPrebook
                    {
                        Total = x.PrebookTotal.ToString(),
                        Successful = $"{x.PrebookSuccessfulPrecent:n1} %",
                    },
                    Searches = new SupplierSearches
                    {
                        Total = x.SearchTotal.ToString(),
                        Successful = $"{x.SearchSuccessfulPrecent:n1} %",
                        Avg_Resp = $"{x.AverageSearchTime} ms",
                    },
                    S2B = x.S2B.ToString("n1")
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
