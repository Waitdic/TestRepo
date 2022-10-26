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
            Random random = new Random();

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

            #region Create Dummy Data

            var stopFlag = random.Next(1, 22);
            List<Node> bookingsByHour = new List<Node>();
            for (int i = 0; i < 24; i++)
            {
                bookingsByHour.Add(new Node());
                bookingsByHour[i].Time = i;
                bookingsByHour[i].PreviousTotal = random.Next(5);
                if (i < stopFlag)
                {
                    bookingsByHour[i].CurrentTotal = random.Next(5);
                }
                if (i > 0)
                {
                    bookingsByHour[i].CurrentTotal = bookingsByHour[i].CurrentTotal + bookingsByHour[i - 1].CurrentTotal;
                    bookingsByHour[i].PreviousTotal = bookingsByHour[i].PreviousTotal + bookingsByHour[i - 1].PreviousTotal;
                }

            }
            List<Node> searchesByHour = new List<Node>();
            for (int i = 0; i < 24; i++)
            {
                searchesByHour.Add(new Node());
                searchesByHour[i].Time = i;
                searchesByHour[i].PreviousTotal = random.Next(5000);
                if (i < stopFlag)
                {
                    searchesByHour[i].CurrentTotal = random.Next(5000);
                }
                if (i > 0)
                {
                    searchesByHour[i].CurrentTotal = searchesByHour[i].CurrentTotal + searchesByHour[i - 1].CurrentTotal;
                    searchesByHour[i].PreviousTotal = searchesByHour[i].PreviousTotal + searchesByHour[i - 1].PreviousTotal;
                }
            }
            List<Summary> summary = new List<Summary>();
            for (int i = 0; i < 3; i++) { summary.Add(new Summary()); }

            summary[0].Name = "Today";
            summary[0].BookTotal = random.Next(20);
            summary[0].BookValue = random.Next(500);
            summary[0].PrebookTotal = random.Next(400);
            summary[0].PrebookSuccess = random.Next(65, 85);
            summary[0].SearchTotal = random.Next(10000);
            summary[0].SearchSuccess = random.Next(65, 85);
            summary[0].AvgResponse = random.Next(2000, 3000);
            summary[0].S2B = summary[0].SearchTotal / summary[0].BookTotal;

            summary[1].Name = "WTD";
            summary[1].BookTotal = random.Next(20, 100);
            summary[1].BookValue = random.Next(2000, 5000);
            summary[1].PrebookTotal = random.Next(1000, 4000);
            summary[1].PrebookSuccess = random.Next(65, 85);
            summary[1].SearchTotal = random.Next(10000, 70000);
            summary[1].SearchSuccess = random.Next(65, 85);
            summary[1].AvgResponse = random.Next(2000, 3000);
            summary[1].S2B = summary[1].SearchTotal / summary[1].BookTotal;

            summary[2].Name = "MTD";
            summary[2].BookTotal = random.Next(1000);
            summary[2].BookValue = random.Next(100000, 500000);
            summary[2].PrebookTotal = random.Next(10000, 40000);
            summary[2].PrebookSuccess = random.Next(65, 90);
            summary[2].SearchTotal = random.Next(100000, 700000);
            summary[2].SearchSuccess = random.Next(65, 90);
            summary[2].AvgResponse = random.Next(2000, 3000);
            summary[2].S2B = summary[2].SearchTotal / summary[2].BookTotal;

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
            supplier[0].S2B = supplier[0].SearchTotal / supplier[0].BookTotal;

            supplier[1].Name = "SunHotels";
            supplier[1].SearchTotal = random.Next(1900, 10000);
            supplier[1].SearchSuccess = random.Next(75, 95);
            supplier[1].AvgResponse = random.Next(2000, 3000);
            supplier[1].PrebookTotal = random.Next(50, 400);
            supplier[1].PrebookSuccess = random.Next(85, 99);
            supplier[1].BookTotal = random.Next(20);
            supplier[1].BookSuccess = random.Next(97, 100);
            supplier[1].S2B = supplier[1].SearchTotal / supplier[1].BookTotal;

            supplier[2].Name = "Miki";
            supplier[2].SearchTotal = random.Next(1900, 10000);
            supplier[2].SearchSuccess = random.Next(75, 95);
            supplier[2].AvgResponse = random.Next(2000, 3000);
            supplier[2].PrebookTotal = random.Next(50, 400);
            supplier[2].PrebookSuccess = random.Next(85, 99);
            supplier[2].BookTotal = random.Next(20);
            supplier[2].BookSuccess = random.Next(97, 100);
            supplier[2].S2B = supplier[2].SearchTotal / supplier[2].BookTotal;

            supplier[3].Name = "Stuba";
            supplier[3].SearchTotal = random.Next(1900, 10000);
            supplier[3].SearchSuccess = random.Next(75, 95);
            supplier[3].AvgResponse = random.Next(2000, 3000);
            supplier[3].PrebookTotal = random.Next(50, 400);
            supplier[3].PrebookSuccess = random.Next(85, 99);
            supplier[3].BookTotal = random.Next(20);
            supplier[3].BookSuccess = random.Next(97, 100);
            supplier[3].S2B = supplier[3].SearchTotal / supplier[3].BookTotal;


            #endregion

            var responseModel = new ResponseModel()
            {
                BookingsByHour = bookingsByHour,
                SearchesByHour = searchesByHour,
                Summary = summary,
                Supplier = supplier,
            };

            response.Ok(responseModel);
            return response;

        }
    }

}
