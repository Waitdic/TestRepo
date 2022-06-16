namespace ThirdParty.Service
{
    using System.Security.Cryptography;
    using Intuitive;
    using Intuitive.Data;
    using Intuitive.DependencyInjection;
    using Intuitive.Helpers.Security;
    using Intuitive.Modules;
    using Microsoft.Extensions.DependencyInjection;
    using ThirdParty;
    using ThirdParty.CSSuppliers;
    using ThirdParty.CSSuppliers.AbreuV2;
    using ThirdParty.CSSuppliers.Acerooms;
    using ThirdParty.CSSuppliers.Altura;
    using ThirdParty.CSSuppliers.AmadeusHotels;
    using ThirdParty.CSSuppliers.ATI;
    using ThirdParty.CSSuppliers.BedsWithEase;
    using ThirdParty.CSSuppliers.Bonotel;
    using ThirdParty.CSSuppliers.DerbySoft;
    using ThirdParty.CSSuppliers.DOTW;
    using ThirdParty.CSSuppliers.ExpediaRapid;
    using ThirdParty.CSSuppliers.FastPayHotels;
    using ThirdParty.CSSuppliers.HotelBedsV2;
    using ThirdParty.CSSuppliers.HotelsProV2;
    using ThirdParty.CSSuppliers.Hotelston;
    using ThirdParty.CSSuppliers.iVectorChannelManager;
    using ThirdParty.CSSuppliers.iVectorConnect;
    using ThirdParty.CSSuppliers.JonView;
    using ThirdParty.CSSuppliers.Jumbo;
    using ThirdParty.CSSuppliers.Juniper;
    using ThirdParty.CSSuppliers.MTS;
    using ThirdParty.CSSuppliers.Miki;
    using ThirdParty.CSSuppliers.Netstorming;
    using ThirdParty.CSSuppliers.OceanBeds;
    using ThirdParty.CSSuppliers.Restel;
    using ThirdParty.CSSuppliers.RMI;
    using ThirdParty.CSSuppliers.Serhs;
    using ThirdParty.CSSuppliers.Stuba;
    using ThirdParty.CSSuppliers.SunHotels;
    using ThirdParty.CSSuppliers.TBOHolidays;
    using ThirdParty.CSSuppliers.TeamAmerica;
    using ThirdParty.CSSuppliers.Travelgate;
    using ThirdParty.CSSuppliers.YouTravel;
    using ThirdParty.Lookups;
    using ThirdParty.Factories;
    using ThirdParty.Models.Tokens;
    using ThirdParty.Repositories;
    using ThirdParty.Search;
    using ThirdParty.Search.Models;
    using ThirdParty.Services;
    using ThirdParty.Utility;
    using Book = SDK.V2.PropertyBook;
    using Cancel = SDK.V2.PropertyCancel;
    using Content = SDK.V2.PropertyContent;
    using List = SDK.V2.PropertyList;
    using Prebook = SDK.V2.PropertyPrebook;
    using Precancel = SDK.V2.PropertyPrecancel;
    using Search = SDK.V2.PropertySearch;

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
            RegisterRepositories(services);
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
        }

        private static void RegisterRepositories(IServiceCollection services)
        {
            services.AddSingleton<IBookingLogRepository, BookingLogRepository>();
            services.AddSingleton<ICurrencyLookupRepository, CurrencyLookupRepository>();
            services.AddSingleton<IMealBasisLookupRepository, MealBasisLookupRepository>();
            services.AddSingleton<IPropertyContentRepository, PropertyContentRepository>();
            services.AddSingleton<ISearchRepository, SearchRepository>();
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

            services.AddSingleton((s)
                => s.GetService<ISecretKeeperFactory>()!
                    .CreateSecretKeeper("bobisben", EncryptionType.Des, CipherMode.CBC));
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
        }

        public void RegsiterThirdPartyUtilities(IServiceCollection services)
        {
            services.AddSingleton<IDOTWSupport, DOTWSupport>();
            services.AddSingleton<IExpediaRapidAPI, ExpediaRapidAPI>();
        }
    }
}