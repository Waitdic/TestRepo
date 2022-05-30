namespace ThirdParty.Service
{
    using Intuitive;
    using Intuitive.Data;
    using Intuitive.DependencyInjection;
    using Intuitive.Helpers;
    using Intuitive.Modules;
    using Microsoft.Extensions.DependencyInjection;
    using ThirdParty;
    using ThirdParty.CSSuppliers;
    using ThirdParty.CSSuppliers.HotelBedsV2;
    using ThirdParty.CSSuppliers.Serhs;
    using ThirdParty.CSSuppliers.Restel;
    using ThirdParty.CSSuppliers.NetStorming.WHL;
    using ThirdParty.CSSuppliers.iVectorConnect;
    using ThirdParty.CSSuppliers.Altura;
    using ThirdParty.CSSuppliers.BedsWithEase;
    using ThirdParty.CSSuppliers.TeamAmerica;
    using ThirdParty.CSSuppliers.OceanBeds;
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
    using ThirdParty.CSSuppliers.NetStorming;
    using ThirdParty.JuniperBase;
    using Intuitive.Helpers.Security;
    using System.Security.Cryptography;
    using Intuitive.Net.WebRequests;
    using System.Net.Http;

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

            services.AddHelperServices(context.Configuration);

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
            services.AddSingleton<IBookabedSettings, InjectedBookabedSettings>();
            services.AddSingleton<IDerbySoftClubMedSettings, InjectedDerbySoftClubMedSettings>();
            services.AddSingleton<IDerbySoftMarriottSettings, InjectedDerbySoftMarriottSettings>();
            services.AddSingleton<IDerbySoftSynxisSettings, InjectedDerbySoftSynxisSettings>();
            services.AddSingleton<IHotelBedsV2Settings, InjectedHotelBedsV2Settings>();
            services.AddSingleton<IImperatoreSettings, InjectedImperatoreSettings>();
            services.AddSingleton<IJuniperECTravelSettings, InjectedJuniperECTravelSettings>();
            services.AddSingleton<IJuniperElevateSettings, InjectedJuniperElevateSettings>();
            services.AddSingleton<IJuniperFastPayHotelsSettings, InjectedJuniperFastPayHotelsSettings>();
            services.AddSingleton<INetstormingSettings, InjectedWHLSettings>();
            services.AddSingleton<INullTestSupplierSettings, InjectedNullTestSupplierSettings>();
            services.AddSingleton<IOceanBedsSettings, InjectedOceanBedsSettings>();
            services.AddSingleton<IRestelSettings, InjectedRestelSettings>();
            services.AddSingleton<ISerhsSettings, InjectedSerhsSettings>();
            services.AddSingleton<ITeamAmericaSettings, InjectedTeamAmericaSettings>();
            services.AddSingleton<IW2MSettings, InjectedW2MSettings>();
            services.AddSingleton<IWelcomeBedsSettings, InjectedWelcomeBedsSettings>();
            services.AddSingleton<IYalagoSettings, InjectedYalagoSettings>();
        }

        private void RegsiterThirdPartySearchServices(IServiceCollection services)
        {
            services.AddSingleton<ThirdPartyPropertySearchBase, AceroomsSearch>();
            services.AddSingleton<ThirdPartyPropertySearchBase, AlturaSearch>();
            services.AddSingleton<ThirdPartyPropertySearchBase, BedsWithEaseSearch>();
            services.AddSingleton<ThirdPartyPropertySearchBase, BookaBedSearch>();
            services.AddSingleton<ThirdPartyPropertySearchBase, DerbySoftClubMedSearch>();
            services.AddSingleton<ThirdPartyPropertySearchBase, DerbySoftSynxisSearch>();
            services.AddSingleton<ThirdPartyPropertySearchBase, DerbySoftMarriottSearch>();
            services.AddSingleton<ThirdPartyPropertySearchBase, HotelBedsV2Search>();
            services.AddSingleton<ThirdPartyPropertySearchBase, ImperatoreSearch>();
            services.AddSingleton<ThirdPartyPropertySearchBase, JuniperElevateSearch>();
            services.AddSingleton<ThirdPartyPropertySearchBase, JuniperECTravelSearch>();
            services.AddSingleton<ThirdPartyPropertySearchBase, JuniperFastPayHotelsSearch>();
            services.AddSingleton<ThirdPartyPropertySearchBase, NullTestSupplier>();
            services.AddSingleton<ThirdPartyPropertySearchBase, OceanBedsSearch>();
            services.AddSingleton<ThirdPartyPropertySearchBase, RestelSearch>();
            services.AddSingleton<ThirdPartyPropertySearchBase, SerhsSearch>();
            services.AddSingleton<ThirdPartyPropertySearchBase, TeamAmericaSearch>();
            services.AddSingleton<ThirdPartyPropertySearchBase, W2MSearch>();
            services.AddSingleton<ThirdPartyPropertySearchBase, WelcomeBedsSearch>();
            services.AddSingleton<ThirdPartyPropertySearchBase, WHLSearch>();
            services.AddSingleton<ThirdPartyPropertySearchBase, YalagoSearch>();
        }

        private void RegsiterThirdPartyBookServices(IServiceCollection services)
        {
            services.AddSingleton<IThirdParty, Acerooms>();
            services.AddSingleton<IThirdParty, Altura>();
            services.AddSingleton<IThirdParty, BedsWithEase>();
            services.AddSingleton<IThirdParty, BookaBed>();
            services.AddSingleton<IThirdParty, DerbySoftClubMed>();
            services.AddSingleton<IThirdParty, DerbySoftSynxis>();
            services.AddSingleton<IThirdParty, DerbySoftMarriott>();
            services.AddSingleton<IThirdParty, HotelBedsV2>();
            services.AddSingleton<IThirdParty, Imperatore>();
            services.AddSingleton<IThirdParty, JuniperElevate>();
            services.AddSingleton<IThirdParty, JuniperECTravel>();
            services.AddSingleton<IThirdParty, JuniperFastPayHotels>();
            services.AddSingleton<IThirdParty, OceanBeds>();
            services.AddSingleton<IThirdParty, Restel>();
            services.AddSingleton<IThirdParty, Serhs>();
            services.AddSingleton<IThirdParty, TeamAmerica>();
            services.AddSingleton<IThirdParty, W2M>();
            services.AddSingleton<IThirdParty, WelcomeBeds>();
            services.AddSingleton<IThirdParty, WHL>();
            services.AddSingleton<IThirdParty, Yalago>();
        }
    }
}