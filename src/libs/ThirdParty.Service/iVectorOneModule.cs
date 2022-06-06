namespace ThirdParty.Service
{
    using System.Security.Cryptography;
    using Intuitive;
    using Intuitive.Data;
    using Intuitive.DependencyInjection;
    using Intuitive.Helpers;
    using Intuitive.Helpers.Security;
    using Intuitive.Modules;
    using Microsoft.Extensions.DependencyInjection;
    using ThirdParty;
    using ThirdParty.CSSuppliers;
    using ThirdParty.CSSuppliers.Acerooms;
    using ThirdParty.CSSuppliers.Altura;
    using ThirdParty.CSSuppliers.BedsWithEase;
    using ThirdParty.CSSuppliers.Bonotel;
    using ThirdParty.CSSuppliers.DerbySoft;
    using ThirdParty.CSSuppliers.DOTW;
    using ThirdParty.CSSuppliers.ExpediaRapid;
    using ThirdParty.CSSuppliers.HotelBedsV2;
    using ThirdParty.CSSuppliers.iVectorConnect;
    using ThirdParty.CSSuppliers.JonView;
    using ThirdParty.CSSuppliers.Juniper;
    using ThirdParty.CSSuppliers.NetStorming;
    using ThirdParty.CSSuppliers.NetStorming.WHL;
    using ThirdParty.CSSuppliers.OceanBeds;
    using ThirdParty.CSSuppliers.Restel;
    using ThirdParty.CSSuppliers.Serhs;
    using ThirdParty.CSSuppliers.Stuba;
    using ThirdParty.CSSuppliers.SunHotels;
    using ThirdParty.CSSuppliers.TeamAmerica;
    using ThirdParty.CSSuppliers.YouTravel;
    using ThirdParty.Lookups;
    using ThirdParty.Factories;
    using ThirdParty.Repositories;
    using ThirdParty.Results;
    using ThirdParty.Models.Tokens;
    using ThirdParty.Search.Models;
    using ThirdParty.Services;
    using ThirdParty.Utility;
    using List = SDK.V2.PropertyList;
    using Content = SDK.V2.PropertyContent;
    using Search = SDK.V2.PropertySearch;
    using Prebook = SDK.V2.PropertyPrebook;
    using Book = SDK.V2.PropertyBook;
    using Precancel = SDK.V2.PropertyPrecancel;
    using Cancel = SDK.V2.PropertyCancel;
    using ThirdParty.CSSuppliers.Jumbo;
    using ThirdParty.CSSuppliers.MTS;
    using ThirdParty.CSSuppliers.Travelgate;

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
                => s.GetService<ISecretKeeperFactory>()
                    .CreateSecretKeeper("bobisben", EncryptionType.Des, CipherMode.CBC));

            //services.AddHttpClient<Request>().ConfigurePrimaryHttpMessageHandler();
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
            services.AddSingleton<IAceroomsSettings, InjectedAceroomsSettings>();
            services.AddSingleton<IAlturaSettings, InjectedAlturaSettings>();
            services.AddSingleton<IBedsWithEaseSettings, InjectedBedsWithEaseSettings>();
            services.AddSingleton<IBonotelSettings, InjectedBonotelSettings>();
            services.AddSingleton<IBookabedSettings, InjectedBookabedSettings>();
            services.AddSingleton<IDerbySoftClubMedSettings, InjectedDerbySoftClubMedSettings>();
            services.AddSingleton<IDerbySoftMarriottSettings, InjectedDerbySoftMarriottSettings>();
            services.AddSingleton<IDerbySoftSynxisSettings, InjectedDerbySoftSynxisSettings>();
            services.AddSingleton<IDOTWSettings, InjectedDOTWSettings>();
            services.AddSingleton<IExpediaRapidSettings, InjectedExpediaRapidSettings>();
            services.AddSingleton<IHotelBedsV2Settings, InjectedHotelBedsV2Settings>();
            services.AddSingleton<IImperatoreSettings, InjectedImperatoreSettings>();
            services.AddSingleton<IJonViewSettings, InjectedJonViewSettings>();
            services.AddSingleton<IJumboSettings, InjectedJumboSettings>();
            services.AddSingleton<IJuniperECTravelSettings, InjectedJuniperECTravelSettings>();
            services.AddSingleton<IJuniperElevateSettings, InjectedJuniperElevateSettings>();
            services.AddSingleton<IJuniperFastPayHotelsSettings, InjectedJuniperFastPayHotelsSettings>();
            services.AddSingleton<INetstormingSettings, InjectedWHLSettings>();
            services.AddSingleton<INullTestSupplierSettings, InjectedNullTestSupplierSettings>();
            services.AddSingleton<IMTSSettings, InjectedMTSSettings>();
            services.AddSingleton<IOceanBedsSettings, InjectedOceanBedsSettings>();
            services.AddSingleton<IRestelSettings, InjectedRestelSettings>();
            services.AddSingleton<ISerhsSettings, InjectedSerhsSettings>();
            services.AddSingleton<IStubaSettings, InjectedStubaSettings>();
            services.AddSingleton<ISunHotelsSettings, InjectedSunHotelsSettings>();
            services.AddSingleton<ITeamAmericaSettings, InjectedTeamAmericaSettings>();
            services.AddSingleton<ITravelgateArabianASettings, InjectedTravelgateArabianASettings>();
            services.AddSingleton<ITravelgateBookohotelSettings, InjectedTravelgateBookohotelSettings>();
            services.AddSingleton<ITravelgateDarinaSettings, InjectedTravelgateDarinaSettings>();
            services.AddSingleton<ITravelgateDerbysoftBestWesternSettings, InjectedTravelgateDerbysoftBestWesternSettings>();
            services.AddSingleton<ITravelgateDerbysoftIHGSettings, InjectedTravelgateDerbysoftIHGSettings>();
            services.AddSingleton<ITravelgateDerbysoftNAVHSettings, InjectedTravelgateDerbysoftNAVHSettings>();
            services.AddSingleton<ITravelgateDerbysoftUORSettings, InjectedTravelgateDerbysoftUORSettings>();
            services.AddSingleton<ITravelgateDingusSettings, InjectedTravelgateDingusSettings>();
            services.AddSingleton<ITravelgateDingusBlueSeaSettings, InjectedTravelgateDingusBlueSeaSettings>();
            services.AddSingleton<ITravelgateDingusSpringHotelsSettings, InjectedTravelgateDingusSpringHotelsSettings>();
            services.AddSingleton<ITravelgateDingusTHBSettings, InjectedTravelgateDingusTHBSettings>();
            services.AddSingleton<ITravelgateDOTWv3Settings, InjectedTravelgateDOTWv3Settings>();
            services.AddSingleton<ITravelgateEETGlobalSettings, InjectedTravelgateEETGlobalSettings>();
            services.AddSingleton<ITravelgateEuroPlayasSettings, InjectedTravelgateEuroPlayasSettings>();
            services.AddSingleton<ITravelgateGekkoSettings, InjectedTravelgateGekkoSettings>();
            services.AddSingleton<ITravelgateHotelTraderSettings, InjectedTravelgateHotelTraderSettings>();
            services.AddSingleton<ITravelgateIxpiraSettings, InjectedTravelgateIxpiraSettings>();
            services.AddSingleton<ITravelgateMethabookSettings, InjectedTravelgateMethabookSettings>();
            services.AddSingleton<ITravelgateOswaldArrigoSettings, InjectedTravelgateOswaldArrigoSettings>();
            services.AddSingleton<ITravelgatePerlatoursSettings, InjectedTravelgatePerlatoursSettings>();
            services.AddSingleton<ITravelgateTBOSettings, InjectedTravelgateTBOSettings>();
            services.AddSingleton<ITravelgateTravellandaSettings, InjectedTravelgateTravellandaSettings>();
            services.AddSingleton<ITravelgateTraveltinoSettings, InjectedTravelgateTraveltinoSettings>();
            services.AddSingleton<ITravelgateViajesOlympiaSettings, InjectedTravelgateViajesOlympiaSettings>();
            services.AddSingleton<ITravelgateWHLSettings, InjectedTravelgateWHLSettings>();
            services.AddSingleton<ITravelgateYalagoSettings, InjectedTravelgateYalagoSettings>();
            services.AddSingleton<IW2MSettings, InjectedW2MSettings>();
            services.AddSingleton<IWelcomeBedsSettings, InjectedWelcomeBedsSettings>();
            services.AddSingleton<IYalagoSettings, InjectedYalagoSettings>();
            services.AddSingleton<IYouTravelSettings, InjectedYouTravelSettings>();
        }

        private void RegsiterThirdPartySearchServices(IServiceCollection services)
        {
            services.AddSingleton<IThirdPartySearch, AceroomsSearch>();
            services.AddSingleton<IThirdPartySearch, AlturaSearch>();
            services.AddSingleton<IThirdPartySearch, BedsWithEaseSearch>();
            services.AddSingleton<IThirdPartySearch, BookaBedSearch>();
            services.AddSingleton<IThirdPartySearch, BonotelSearch>();
            services.AddSingleton<IThirdPartySearch, DerbySoftClubMedSearch>();
            services.AddSingleton<IThirdPartySearch, DerbySoftSynxisSearch>();
            services.AddSingleton<IThirdPartySearch, DerbySoftMarriottSearch>();
            services.AddSingleton<IThirdPartySearch, DOTWSearch>();
            services.AddSingleton<IThirdPartySearch, ExpediaRapidSearch>();
            services.AddSingleton<IThirdPartySearch, HotelBedsV2Search>();
            services.AddSingleton<IThirdPartySearch, ImperatoreSearch>();
            services.AddSingleton<IThirdPartySearch, JonViewSearch>();
            services.AddSingleton<IThirdPartySearch, JumboSearch>();
            services.AddSingleton<IThirdPartySearch, JuniperElevateSearch>();
            services.AddSingleton<IThirdPartySearch, JuniperECTravelSearch>();
            services.AddSingleton<IThirdPartySearch, JuniperFastPayHotelsSearch>();
            services.AddSingleton<IThirdPartySearch, NullTestSupplier>();
            services.AddSingleton<IThirdPartySearch, MTSSearch>();
            services.AddSingleton<IThirdPartySearch, OceanBedsSearch>();
            services.AddSingleton<IThirdPartySearch, RestelSearch>();
            services.AddSingleton<IThirdPartySearch, SerhsSearch>();
            services.AddSingleton<IThirdPartySearch, StubaSearch>();
            services.AddSingleton<IThirdPartySearch, SunHotelsSearch>();
            services.AddSingleton<IThirdPartySearch, TeamAmericaSearch>();
            services.AddSingleton<IThirdPartySearch, TravelgateArabianASearch>();
            services.AddSingleton<IThirdPartySearch, TravelgateBookohotelSearch>();
            services.AddSingleton<IThirdPartySearch, TravelgateDarinaSearch>();
            services.AddSingleton<IThirdPartySearch, TravelgateDerbysoftBestWesternSearch>();
            services.AddSingleton<IThirdPartySearch, TravelgateDerbysoftIHGSearch>();
            services.AddSingleton<IThirdPartySearch, TravelgateDerbysoftNAVHSearch>();
            services.AddSingleton<IThirdPartySearch, TravelgateDerbysoftUORSearch>();
            services.AddSingleton<IThirdPartySearch, TravelgateDingusSearch>();
            services.AddSingleton<IThirdPartySearch, TravelgateDingusBlueSeaSearch>();
            services.AddSingleton<IThirdPartySearch, TravelgateDingusSpringHotelsSearch>();
            services.AddSingleton<IThirdPartySearch, TravelgateDingusTHBSearch>();
            services.AddSingleton<IThirdPartySearch, TravelgateDOTWv3Search>();
            services.AddSingleton<IThirdPartySearch, TravelgateEETGlobalSearch>();
            services.AddSingleton<IThirdPartySearch, TravelgateEuroPlayasSearch>();
            services.AddSingleton<IThirdPartySearch, TravelgateGekkoSearch>();
            services.AddSingleton<IThirdPartySearch, TravelgateHotelTraderSearch>();
            services.AddSingleton<IThirdPartySearch, TravelgateIxpiraSearch>();
            services.AddSingleton<IThirdPartySearch, TravelgateMethabookSearch>();
            services.AddSingleton<IThirdPartySearch, TravelgateOswaldArrigoSearch>();
            services.AddSingleton<IThirdPartySearch, TravelgatePerlatoursSearch>();
            services.AddSingleton<IThirdPartySearch, TravelgateTBOSearch>();
            services.AddSingleton<IThirdPartySearch, TravelgateTravellandaSearch>();
            services.AddSingleton<IThirdPartySearch, TravelgateTraveltinoSearch>();
            services.AddSingleton<IThirdPartySearch, TravelgateViajesOlympiaSearch>();
            services.AddSingleton<IThirdPartySearch, TravelgateWHLSearch>();
            services.AddSingleton<IThirdPartySearch, TravelgateYalagoSearch>();
            services.AddSingleton<IThirdPartySearch, W2MSearch>();
            services.AddSingleton<IThirdPartySearch, WelcomeBedsSearch>();
            services.AddSingleton<IThirdPartySearch, WHLSearch>();
            services.AddSingleton<IThirdPartySearch, YalagoSearch>();
            services.AddSingleton<IThirdPartySearch, YouTravelSearch>();
        }

        private void RegsiterThirdPartyBookServices(IServiceCollection services)
        {
            services.AddSingleton<IThirdParty, Acerooms>();
            services.AddSingleton<IThirdParty, Altura>();
            services.AddSingleton<IThirdParty, BedsWithEase>();
            services.AddSingleton<IThirdParty, Bonotel>();
            services.AddSingleton<IThirdParty, BookaBed>();
            services.AddSingleton<IThirdParty, DerbySoftClubMed>();
            services.AddSingleton<IThirdParty, DerbySoftSynxis>();
            services.AddSingleton<IThirdParty, DerbySoftMarriott>();
            services.AddSingleton<IThirdParty, DOTW>();
            services.AddSingleton<IThirdParty, ExpediaRapid>();
            services.AddSingleton<IThirdParty, HotelBedsV2>();
            services.AddSingleton<IThirdParty, Imperatore>();
            services.AddSingleton<IThirdParty, JonView>();
            services.AddSingleton<IThirdParty, Jumbo>();
            services.AddSingleton<IThirdParty, JuniperElevate>();
            services.AddSingleton<IThirdParty, JuniperECTravel>();
            services.AddSingleton<IThirdParty, JuniperFastPayHotels>();
            services.AddSingleton<IThirdParty, MTS>();
            services.AddSingleton<IThirdParty, OceanBeds>();
            services.AddSingleton<IThirdParty, Restel>();
            services.AddSingleton<IThirdParty, Serhs>();
            services.AddSingleton<IThirdParty, Stuba>();
            services.AddSingleton<IThirdParty, SunHotels>();
            services.AddSingleton<IThirdParty, TeamAmerica>();
            services.AddSingleton<IThirdParty, TravelgateArabianA>();
            services.AddSingleton<IThirdParty, TravelgateBookohotel>();
            services.AddSingleton<IThirdParty, TravelgateDarina>();
            services.AddSingleton<IThirdParty, TravelgateDerbysoftBestWestern>();
            services.AddSingleton<IThirdParty, TravelgateDerbysoftIHG>();
            services.AddSingleton<IThirdParty, TravelgateDerbysoftNAVH>();
            services.AddSingleton<IThirdParty, TravelgateDerbysoftUOR>();
            services.AddSingleton<IThirdParty, TravelgateDingus>();
            services.AddSingleton<IThirdParty, TravelgateDingusBlueSea>();
            services.AddSingleton<IThirdParty, TravelgateDingusSpringHotels>();
            services.AddSingleton<IThirdParty, TravelgateDingusTHB>();
            services.AddSingleton<IThirdParty, TravelgateDOTWv3>();
            services.AddSingleton<IThirdParty, TravelgateEETGlobal>();
            services.AddSingleton<IThirdParty, TravelgateEuroPlayas>();
            services.AddSingleton<IThirdParty, TravelgateGekko>();
            services.AddSingleton<IThirdParty, TravelgateHotelTrader>();
            services.AddSingleton<IThirdParty, TravelgateIxpira>();
            services.AddSingleton<IThirdParty, TravelgateMethabook>();
            services.AddSingleton<IThirdParty, TravelgateOswaldArrigo>();
            services.AddSingleton<IThirdParty, TravelgatePerlatours>();
            services.AddSingleton<IThirdParty, TravelgateTBO>();
            services.AddSingleton<IThirdParty, TravelgateTravellanda>();
            services.AddSingleton<IThirdParty, TravelgateTraveltino>();
            services.AddSingleton<IThirdParty, TravelgateViajesOlympia>();
            services.AddSingleton<IThirdParty, TravelgateWHL>();
            services.AddSingleton<IThirdParty, TravelgateYalago>();
            services.AddSingleton<IThirdParty, W2M>();
            services.AddSingleton<IThirdParty, WelcomeBeds>();
            services.AddSingleton<IThirdParty, WHL>();
            services.AddSingleton<IThirdParty, YouTravel>();
        }

        public void RegsiterThirdPartyUtilities(IServiceCollection services)
        {
            services.AddSingleton<IDOTWSupport, DOTWSupport>();
            services.AddSingleton<IExpediaRapidAPI, ExpediaRapidAPI>();
        }
    }
}