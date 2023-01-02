namespace iVectorOne.Service
{
    using System.Security.Cryptography;
    using Intuitive;
    using Intuitive.Data;
    using Intuitive.DependencyInjection;
    using Intuitive.Helpers.Security;
    using Intuitive.Modules;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using iVectorOne;
    using iVectorOne.Suppliers;
    using iVectorOne.Suppliers.AbreuV2;
    using iVectorOne.Suppliers.Acerooms;
    using iVectorOne.Suppliers.Altura;
    using iVectorOne.Suppliers.AmadeusHotels;
    using iVectorOne.Suppliers.ATI;
    using iVectorOne.Suppliers.BedsWithEase;
    using iVectorOne.Suppliers.Bonotel;
    using iVectorOne.Suppliers.ChannelManager;
    using iVectorOne.Suppliers.DerbySoft;
    using iVectorOne.Suppliers.DOTW;
    using iVectorOne.Suppliers.ExpediaRapid;
    using iVectorOne.Suppliers.FastPayHotels;
    using iVectorOne.Suppliers.GoGlobal;
    using iVectorOne.Suppliers.HotelBedsV2;
    using iVectorOne.Suppliers.HotelsProV2;
    using iVectorOne.Suppliers.Hotelston;
    using iVectorOne.Suppliers.iVectorConnect;
    using iVectorOne.Suppliers.JonView;
    using iVectorOne.Suppliers.Jumbo;
    using iVectorOne.Suppliers.Juniper;
    using iVectorOne.Suppliers.MTS;
    using iVectorOne.Suppliers.Miki;
    using iVectorOne.Suppliers.Netstorming;
    using iVectorOne.Suppliers.OceanBeds;
    using iVectorOne.Suppliers.Restel;
    using iVectorOne.Suppliers.RMI;
    using iVectorOne.Suppliers.Serhs;
    using iVectorOne.Suppliers.Stuba;
    using iVectorOne.Suppliers.SunHotels;
    using iVectorOne.Suppliers.TBOHolidays;
    using iVectorOne.Suppliers.TeamAmerica;
    using iVectorOne.Suppliers.Travelgate;
    using iVectorOne.Suppliers.YouTravel;
    using iVectorOne.Suppliers.TourPlanTransfers;
    using iVectorOne.Suppliers.GowaySydneyTransfers;
    using iVectorOne.Lookups;
    using iVectorOne.Factories;
    using iVectorOne.Models.Tokens;
    using iVectorOne.Repositories;
    using iVectorOne.Search;
    using iVectorOne.Search.Models;
    using iVectorOne.Services;
    using iVectorOne.Utility;
    using Book = SDK.V2.PropertyBook;
    using Cancel = SDK.V2.PropertyCancel;
    using Content = SDK.V2.PropertyContent;
    using List = SDK.V2.PropertyList;
    using Prebook = SDK.V2.PropertyPrebook;
    using Precancel = SDK.V2.PropertyPrecancel;
    using Search = SDK.V2.PropertySearch;
    using TransferBook = SDK.V2.TransferBook;
    using TransferCancel = SDK.V2.TransferCancel;
    using TransferPrebook = SDK.V2.TransferPrebook;
    using TransferPrecancel = SDK.V2.TransferPrecancel;
    using TransferSearch = SDK.V2.TransferSearch;
    using ExtraBook = SDK.V2.ExtraBook;
    using ExtraPrebook = SDK.V2.ExtraPrebook;
    using ExtraSearch = SDK.V2.ExtraSearch;

    public class iVectorOneModule : ModuleBase, IServicesBuilder
    {
        public const string IVectorOne = "iVectorOne";

        public iVectorOneModule()
            : base(new ModuleId(IVectorOne), IVectorOne, dependencies: new[] { CoreInfo.CoreModuleId, DataInfo.DataModuleId })
        {
        }

        public void BuildServices(ServicesBuilderContext context, IServiceCollection services)
        {
            RegisterFactories(services);
            RegisterRepositories(context, services);
            RegisterServices(context, services);
            RegisterMediators(services);
            RegsiterThirdPartyConfigs(services);
            RegsiterThirdPartySearchServices(services);
            RegsiterThirdPartyBookServices(services);
            RegsiterThirdPartyUtilities(services);
        }

        private static void RegisterFactories(IServiceCollection services)
        {
            services.AddSingleton<ICancelPropertyResponseFactory, CancelPropertyResponseFactory>();
            services.AddSingleton<IPropertyBookResponseFactory, PropertyBookResponseFactory>();
            services.AddSingleton<IPropertyDetailsFactory, PropertyDetailsFactory>();
            services.AddSingleton<IPropertyPrebookResponseFactory, PropertyPrebookResponseFactory>();
            services.AddSingleton<IPropertySearchResponseFactory, PropertySearchResponseFactory>();
            services.AddSingleton<IResortSplitFactory, ResortSplitFactory>();
            services.AddSingleton<IRoomRequestsFactory, RoomRequestsFactory>();
            services.AddSingleton<ISearchDetailsFactory, SearchDetailsFactory>();
            services.AddSingleton<IThirdPartyFactory, ThirdPartyFactory>();
            services.AddSingleton<ICancelTransferResponseFactory, CancelTransferResponseFactory>();
            services.AddSingleton<ITransferBookResponseFactory, TransferBookResponseFactory>();
            services.AddSingleton<ITransferDetailsFactory, TransferDetailsFactory>();
            services.AddSingleton<ITransferPrebookResponseFactory, TransferPrebookResponseFactory>();
            services.AddSingleton<ITransferSearchResponseFactory, TransferSearchResponseFactory>();
            services.AddSingleton<ITransferSearchDetailsFactory, TransferSearchDetailsFactory>();
            services.AddSingleton<ITransferThirdPartyFactory, TransferThirdPartyFactory>();
            services.AddSingleton<ITransferLocationMappingFactory, TransferLocationMappingFactory>();

            services.AddSingleton<IExtraSearchResponseFactory, ExtraSearchResponseFactory>();
            services.AddSingleton<IExtraSearchDetailsFactory, ExtraSearchDetailsFactory>();
            services.AddSingleton<IExtraThirdPartyFactory, ExtraThirdPartyFactory>();
            services.AddSingleton<IExtraLocationMappingFactory, ExtraLocationMappingFactory>();
            services.AddSingleton<IExtraDetailsFactory, ExtraDetailsFactory>();
            services.AddSingleton<IExtraPrebookResponseFactory, ExtraPrebookResponseFactory>();
            services.AddSingleton<IExtraBookResponseFactory, ExtraBookResponseFactory>();
        }

        private static void RegisterRepositories(ServicesBuilderContext context, IServiceCollection services)
        {
            services.AddSingleton<IAPILogRepository, APILogRepository>();
            services.AddSingleton<ICurrencyLookupRepository, CurrencyLookupRepository>();
            services.AddSingleton<IMealBasisLookupRepository, MealBasisLookupRepository>();
            services.AddSingleton<IPropertyContentRepository, PropertyContentRepository>();
            services.AddSingleton<ISearchRepository, SearchRepository>();
            services.AddSingleton<ISearchStoreRepository>(_ => new SearchStoreRepository(context.Configuration.GetConnectionString("Telemetry")));
            services.AddSingleton<ITransferSearchStoreRepository>(_ => new TransferSearchStoreRepository(context.Configuration.GetConnectionString("Telemetry")));
            services.AddSingleton<ISupplierLogRepository, SupplierLogRepository>();
            services.AddSingleton<IBookingRepository, BookingRepository>();
            services.AddSingleton<ITransferAPILogRepository, TransferAPILogRepository>();
            services.AddSingleton<ITransferSearchRepository, TransferSearchRepository>();
            services.AddSingleton<ITransferSupplierLogRepository, TransferSupplierLogRepository>();
            services.AddSingleton<ITransferBookingRepository, TransferBookingRepository>();
            services.AddSingleton<IExtraSearchStoreRepository>(_ => new ExtraSearchStoreRepository(context.Configuration.GetConnectionString("Telemetry")));
            services.AddSingleton<IExtraSearchRepository, ExtraSearchRepository>();
            services.AddSingleton<IExtraAPILogRepository, ExtraAPILogRepository>();
            services.AddSingleton<IExtraSupplierLogRepository, ExtraSupplierLogRepository>();
            services.AddSingleton<IExtraBookingRepository, ExtraBookingRepository>();

        }

        private static void RegisterServices(ServicesBuilderContext context, IServiceCollection services)
        {
            services.AddSingleton<IBookService, BookService>();
            services.AddSingleton<ICancellationService, CancellationService>();
            services.AddSingleton<ITokenService, EncodedTokenService>();
            services.AddSingleton<IPrebookService, PrebookService>();
            services.AddSingleton<IPropertyContentService, PropertyContentService>();
            services.AddSingleton<ISearchService, SearchService>();
            services.AddSingleton<ISuppierReferenceValidator, SuppierReferenceValidator>();
            services.AddSingleton<ISearchResultsProcessor, SearchResultsProcessor>();
            services.AddSingleton<IGroupResultsProcessor, GroupResultsProcessor>();
            services.AddSingleton<IFilter, Filter>();
            services.AddSingleton<IResultDeduper, ResultsDeduper>();
            services.AddSingleton<IBaseConverter, Base92Converter>();
            services.AddSingleton<ITokenValues, TokenValues>();
            services.AddSingleton<ITPSupport, TPSupportWrapper>();
            services.AddSingleton<IRequestTracker, ThirdPartyRequestTracker>();
            services.AddSingleton<IThirdPartyPropertySearchRunner, ThirdPartyPropertySearchRunner>();
            services.AddSingleton<Services.Transfer.ISearchService, Services.Transfer.SearchService>();
            services.AddSingleton<Services.Transfer.ILocationManagerService, Services.Transfer.LocationManagerService>();
            services.AddSingleton<Services.Transfer.IPrebookService, Services.Transfer.PrebookService>();
            services.AddSingleton<Services.Transfer.IBookService, Services.Transfer.BookService>();
            services.AddSingleton<Services.Transfer.ICancellationService, Services.Transfer.CancellationService>();
            services.AddSingleton<ITransferSearchResultsProcessor, TransferSearchResultsProcessor>();
            services.AddSingleton<IThirdPartyTransferSearchRunner, ThirdPartyTransferSearchRunner>();

            services.AddSingleton<Services.Extra.ISearchService, Services.Extra.SearchService>();
            services.AddSingleton<Services.Extra.IPrebookService, Services.Extra.PrebookService>();
            services.AddSingleton<Services.Extra.IBookService, Services.Extra.BookService>();
            services.AddSingleton<IExtraSearchResultsProcessor, ExtraSearchResultsProcessor>();
            services.AddSingleton<IThirdPartyExtraSearchRunner, ThirdPartyExtraSearchRunner>();

            services.AddSingleton((s)
                => s.GetService<ISecretKeeperFactory>()!
                    .CreateSecretKeeper("FireyNebulaIsGod", EncryptionType.Aes, CipherMode.ECB));

            services.AddSingleton<ISearchStoreService>(s =>
                new SearchStoreService(
                    s.GetRequiredService<ILogger<SearchStoreService>>(),
                    s.GetRequiredService<ISearchStoreRepository>(),
                    context.Configuration.GetValue<int>("SearchStoreBulkInsertSize")));

            services.AddSingleton<Services.Transfer.ISearchStoreService>(s =>
                new Services.Transfer.SearchStoreService(
                    s.GetRequiredService<ILogger<Services.Transfer.SearchStoreService>>(),
                    s.GetRequiredService<ITransferSearchStoreRepository>(),
                    context.Configuration.GetValue<int>("SearchStoreBulkInsertSize")));

            services.AddSingleton<Services.Extra.ISearchStoreService>(s =>
                new Services.Extra.SearchStoreService(
                    s.GetRequiredService<ILogger<Services.Extra.SearchStoreService>>(),
                    s.GetRequiredService<IExtraSearchStoreRepository>(),
                    context.Configuration.GetValue<int>("SearchStoreBulkInsertSize")));
        }

        private static void RegisterMediators(IServiceCollection services)
        {
            services.AddHandler<List.Request, List.Response, List.Handler>();
            services.AddHandlerAndValidator<Content.Request, Content.Response, Content.Handler, Content.Validator>();
            services.AddHandlerAndValidator<Search.Request, Search.Response, Search.Handler, Search.Validator>();
            services.AddHandlerAndValidator<Prebook.Request, Prebook.Response, Prebook.Handler, Prebook.Validator>();
            services.AddHandlerAndValidator<Book.Request, Book.Response, Book.Handler, Book.Validator>();
            services.AddHandlerAndValidator<Precancel.Request, Precancel.Response, Precancel.Handler, Precancel.Validator>();
            services.AddHandlerAndValidator<Cancel.Request, Cancel.Response, Cancel.Handler, Cancel.Validator>();
            services.AddHandlerAndValidator<TransferSearch.Request, TransferSearch.Response, TransferSearch.Handler, TransferSearch.Validator>();
            services.AddHandlerAndValidator<TransferPrebook.Request, TransferPrebook.Response, TransferPrebook.Handler, TransferPrebook.Validator>();
            services.AddHandlerAndValidator<TransferBook.Request, TransferBook.Response, TransferBook.Handler, TransferBook.Validator>();
            services.AddHandlerAndValidator<TransferPrecancel.Request, TransferPrecancel.Response, TransferPrecancel.Handler, TransferPrecancel.Validator>();
            services.AddHandlerAndValidator<TransferCancel.Request, TransferCancel.Response, TransferCancel.Handler, TransferCancel.Validator>();
            services.AddHandlerAndValidator<ExtraSearch.Request, ExtraSearch.Response, ExtraSearch.Handler, ExtraSearch.Validator>();
            services.AddHandlerAndValidator<ExtraPrebook.Request, ExtraPrebook.Response, ExtraPrebook.Handler, ExtraPrebook.Validator>();
            services.AddHandlerAndValidator<ExtraBook.Request, ExtraBook.Response, ExtraBook.Handler, ExtraBook.Validator>();
        }

        private void RegsiterThirdPartyConfigs(IServiceCollection services)
        {
            services.AddSingleton<IAbreuV2Settings, InjectedAbreuV2Settings>();
            services.AddSingleton<IAceroomsSettings, InjectedAceroomsSettings>();
            services.AddSingleton<IAlturaSettings, InjectedAlturaSettings>();
            services.AddSingleton<IATISettings, InjectedATISettings>();
            services.AddSingleton<IAmadeusHotelsSettings, InjectedAmadeusHotelsSettings>();
            services.AddSingleton<IBedsWithEaseSettings, InjectedBedsWithEaseSettings>();
            services.AddSingleton<IBonotelSettings, InjectedBonotelSettings>();
            services.AddSingleton<IChannelManagerSettings, InjectedChannelManagerSettings>();
            services.AddSingleton<IDerbySoftSettings, InjectedDerbySoftSettings>();
            services.AddSingleton<IDOTWSettings, InjectedDOTWSettings>();
            services.AddSingleton<IExpediaRapidSettings, InjectedExpediaRapidSettings>();
            services.AddSingleton<IFastPayHotelsSettings, InjectedFastPayHotelsSettings>();
            services.AddSingleton<IGoGlobalSettings, InjectedGoGlobalSettings>();
            services.AddSingleton<IHotelBedsV2Settings, InjectedHotelBedsV2Settings>();
            services.AddSingleton<IHotelsProV2Settings, InjectedHotelsProV2Settings>();
            services.AddSingleton<IHotelstonSettings, InjectedHotelstonSettings>();
            services.AddSingleton<IiVectorConnectSettings, InjectediVectorConnectSettings>();
            services.AddSingleton<IJonViewSettings, InjectedJonViewSettings>();
            services.AddSingleton<IJumboSettings, InjectedJumboSettings>();
            services.AddSingleton<IJuniperSettings, InjectedJuniperSettings>();
            services.AddSingleton<INetstormingSettings, InjectedNetstormingSettings>();
            services.AddSingleton<INullTestSupplierSettings, InjectedNullTestSupplierSettings>();
            services.AddSingleton<IMikiSettings, InjectedMikiSettings>();
            services.AddSingleton<IMTSSettings, InjectedMTSSettings>();
            services.AddSingleton<IOceanBedsSettings, InjectedOceanBedsSettings>();
            services.AddSingleton<IRestelSettings, InjectedRestelSettings>();
            services.AddSingleton<IRMISettings, InjectedRMISettings>();
            services.AddSingleton<ISerhsSettings, InjectedSerhsSettings>();
            services.AddSingleton<IStubaSettings, InjectedStubaSettings>();
            services.AddSingleton<ISunHotelsSettings, InjectedSunHotelsSettings>();
            services.AddSingleton<ITBOHolidaysSettings, InjectedTBOHolidaysSettings>();
            services.AddSingleton<ITeamAmericaSettings, InjectedTeamAmericaSettings>();
            services.AddSingleton<ITravelgateSettings, InjectedTravelgateSettings>();
            services.AddSingleton<IW2MSettings, InjectedW2MSettings>();
            services.AddSingleton<IWelcomeBedsSettings, InjectedWelcomeBedsSettings>();
            services.AddSingleton<IYalagoSettings, InjectedYalagoSettings>();
            services.AddSingleton<IYouTravelSettings, InjectedYouTravelSettings>();

            services.AddSingleton<ITestTransferSupplierSettings, InjectedTestTransferSupplierSettings>();
            services.AddSingleton<ITestExtraSupplierSettings, InjectedExtraTestSupplierSettings>();
        }

        private void RegsiterThirdPartySearchServices(IServiceCollection services)
        {
            services.AddSingleton<IThirdPartySearch, AbreuV2Search>();
            services.AddSingleton<IThirdPartySearch, AceroomsSearch>();
            services.AddSingleton<IThirdPartySearch, AlturaSearch>();
            services.AddSingleton<IThirdPartySearch, AmadeusHotelsSearch>();
            services.AddSingleton<IThirdPartySearch, ATISearch>();
            services.AddSingleton<IThirdPartySearch, BedsWithEaseSearch>();
            services.AddSingleton<IThirdPartySearch, BonotelSearch>();
            services.AddSingleton<IThirdPartySearch, ChannelManagerSearch>();
            services.AddSingleton<IThirdPartySearch, DerbySoftSearch>();
            services.AddSingleton<IThirdPartySearch, DOTWSearch>();
            services.AddSingleton<IThirdPartySearch, ExpediaRapidSearch>();
            services.AddSingleton<IThirdPartySearch, FastPayHotelsSearch>();
            services.AddSingleton<IThirdPartySearch, GoGlobalSearch>();
            services.AddSingleton<IThirdPartySearch, HotelBedsV2Search>();
            services.AddSingleton<IThirdPartySearch, HotelsProV2Search>();
            services.AddSingleton<IThirdPartySearch, HotelstonSearch>();
            services.AddSingleton<IThirdPartySearch, iVectorConnectSearch>();
            services.AddSingleton<IThirdPartySearch, JonViewSearch>();
            services.AddSingleton<IThirdPartySearch, JumboSearch>();
            services.AddSingleton<IThirdPartySearch, JuniperSearch>();
            services.AddSingleton<IThirdPartySearch, NullTestSupplierSearch>();
            services.AddSingleton<IThirdPartySearch, NetstormingSearch>();
            services.AddSingleton<IThirdPartySearch, MikiSearch>();
            services.AddSingleton<IThirdPartySearch, MTSSearch>();
            services.AddSingleton<IThirdPartySearch, OceanBedsSearch>();
            services.AddSingleton<IThirdPartySearch, RestelSearch>();
            services.AddSingleton<IThirdPartySearch, RMISearch>();
            services.AddSingleton<IThirdPartySearch, SerhsSearch>();
            services.AddSingleton<IThirdPartySearch, StubaSearch>();
            services.AddSingleton<IThirdPartySearch, SunHotelsSearch>();
            services.AddSingleton<IThirdPartySearch, TBOHolidaysSearch>();
            services.AddSingleton<IThirdPartySearch, TeamAmericaSearch>();
            services.AddSingleton<IThirdPartySearch, TravelgateSearch>();
            services.AddSingleton<IThirdPartySearch, W2MSearch>();
            services.AddSingleton<IThirdPartySearch, WelcomeBedsSearch>();
            services.AddSingleton<IThirdPartySearch, YalagoSearch>();
            services.AddSingleton<IThirdPartySearch, YouTravelSearch>();

            services.AddSingleton<Transfer.IThirdPartySearch, GowaySydneyTransfersSearch>();
            services.AddSingleton<Transfer.IThirdPartySearch, TestTransferSupplierSearch>();
            services.AddSingleton<Extra.IThirdPartySearch, TestExtraSupplierSearch>();
            services.AddSingleton<Extra.IThirdParty, TestExtraSupplier>();
        }

        private void RegsiterThirdPartyBookServices(IServiceCollection services)
        {
            services.AddSingleton<IThirdParty, AbreuV2>();
            services.AddSingleton<IThirdParty, Acerooms>();
            services.AddSingleton<IThirdParty, Altura>();
            services.AddSingleton<IThirdParty, AmadeusHotels>();
            services.AddSingleton<IThirdParty, ATI>();
            services.AddSingleton<IThirdParty, BedsWithEase>();
            services.AddSingleton<IThirdParty, Bonotel>();
            services.AddSingleton<IThirdParty, ChannelManager>();
            services.AddSingleton<IThirdParty, DerbySoft>();
            services.AddSingleton<IThirdParty, DOTW>();
            services.AddSingleton<IThirdParty, ExpediaRapid>();
            services.AddSingleton<IThirdParty, FastPayHotels>();
            services.AddSingleton<IThirdParty, GoGlobal>();
            services.AddSingleton<IThirdParty, HotelBedsV2>();
            services.AddSingleton<IThirdParty, HotelsProV2>();
            services.AddSingleton<IThirdParty, Hotelston>();
            services.AddSingleton<IThirdParty, iVectorConnect>();
            services.AddSingleton<IThirdParty, JonView>();
            services.AddSingleton<IThirdParty, Jumbo>();
            services.AddSingleton<IThirdParty, Juniper>();
            services.AddSingleton<IThirdParty, Miki>();
            services.AddSingleton<IThirdParty, MTS>();
            services.AddSingleton<IThirdParty, Netstorming>();
            services.AddSingleton<IThirdParty, OceanBeds>();
            services.AddSingleton<IThirdParty, Restel>();
            services.AddSingleton<IThirdParty, RMI>();
            services.AddSingleton<IThirdParty, Serhs>();
            services.AddSingleton<IThirdParty, Stuba>();
            services.AddSingleton<IThirdParty, SunHotels>();
            services.AddSingleton<IThirdParty, TBOHolidays>();
            services.AddSingleton<IThirdParty, TeamAmerica>();
            services.AddSingleton<IThirdParty, Travelgate>();
            services.AddSingleton<IThirdParty, W2M>();
            services.AddSingleton<IThirdParty, WelcomeBeds>();
            services.AddSingleton<IThirdParty, YouTravel>();

            services.AddSingleton<Transfer.IThirdParty, GowaySydneyTransfers>();
            services.AddSingleton<Transfer.IThirdParty, TestTransferSupplier>();
        }

        public void RegsiterThirdPartyUtilities(IServiceCollection services)
        {
            services.AddSingleton<IDOTWSupport, DOTWSupport>();
            services.AddSingleton<IExpediaRapidAPI, ExpediaRapidAPI>();
        }
    }
}