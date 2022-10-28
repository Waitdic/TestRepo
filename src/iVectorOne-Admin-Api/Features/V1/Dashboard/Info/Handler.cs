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
                    Successful = x.PrebookTotal == 0 ? "0 %" : $"{(x.PrebookSuccess / x.PrebookTotal) * 100} %",
                },
                Searches = new Searches
                {
                    Total = x.SearchTotal.ToString(),
                    Successful = x.SearchTotal == 0 ? "0 %" : $"{(x.SearchSuccessful / x.SearchTotal) * 100} %",
                    AvgResp = $"{x.AverageSearchTime} ms",
                },
                S2B = x.BookingTotal == 0 ? "0 " : (x.SearchTotal / x.BookingTotal).ToString(),
            }).ToList();

            #region Create Dummy Data

            List<Supplier> supplier = new List<Supplier>();
            for (int i = 0; i < 4; i++) { supplier.Add(new Supplier()); }

            supplier[0].Name = "HotelBeds";
            supplier[0].SearchTotal = random.Next(1900, 10000);
            supplier[0].SearchSuccess = random.Next(75, 95);
            supplier[0].AvgResponse = random.Next(2000, 3000);
            supplier[0].PrebookTotal = random.Next(50, 400);
            supplier[0].PrebookSuccess = random.Next(85, 99);
            supplier[0].BookTotal = random.Next(20);
            supplier[0].BookSuccess = random.Next(97, 100);
            supplier[0].S2B = 0;

            supplier[1].Name = "SunHotels";
            supplier[1].SearchTotal = random.Next(1900, 10000);
            supplier[1].SearchSuccess = random.Next(75, 95);
            supplier[1].AvgResponse = random.Next(2000, 3000);
            supplier[1].PrebookTotal = random.Next(50, 400);
            supplier[1].PrebookSuccess = random.Next(85, 99);
            supplier[1].BookTotal = random.Next(20);
            supplier[1].BookSuccess = random.Next(97, 100);
            supplier[1].S2B = 0;

            supplier[2].Name = "Miki";
            supplier[2].SearchTotal = random.Next(1900, 10000);
            supplier[2].SearchSuccess = random.Next(75, 95);
            supplier[2].AvgResponse = random.Next(2000, 3000);
            supplier[2].PrebookTotal = random.Next(50, 400);
            supplier[2].PrebookSuccess = random.Next(85, 99);
            supplier[2].BookTotal = random.Next(20);
            supplier[2].BookSuccess = random.Next(97, 100);
            supplier[2].S2B = 0;

            supplier[3].Name = "Stuba";
            supplier[3].SearchTotal = random.Next(1900, 10000);
            supplier[3].SearchSuccess = random.Next(75, 95);
            supplier[3].AvgResponse = random.Next(2000, 3000);
            supplier[3].PrebookTotal = random.Next(50, 400);
            supplier[3].PrebookSuccess = random.Next(85, 99);
            supplier[3].BookTotal = random.Next(20);
            supplier[3].BookSuccess = random.Next(97, 100);
            supplier[3].S2B = 0;


            #endregion

            var responseModel = new ResponseModel()
            {
                BookingsByHour = bookingsByHour,
                SearchesByHour = searchesByHour,
                Summary = dashboardSummary,
                Supplier = supplier,
            };

            response.Ok(responseModel);
            return response;

        }
    }

}
