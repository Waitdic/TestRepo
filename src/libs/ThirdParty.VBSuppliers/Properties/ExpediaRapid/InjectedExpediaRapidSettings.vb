Imports Intuitive.Helpers.Extensions
Imports ThirdParty.Abstractions
Imports ThirdParty.Abstractions.Search.Settings
Imports ThirdParty.Abstractions.Support.SettingsSupport

Namespace ExpediaRapid

    Public Class InjectedExpediaRapidSettings
        Implements IExpediaRapidSettings

        Private ReadOnly configuration As ThirdPartyConfiguration

        Public Sub New(ByVal configuration As ThirdPartyConfiguration)
            Me.configuration = configuration
        End Sub

        Public ReadOnly Property ApiKey(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IExpediaRapidSettings.ApiKey
            Get
                Return Get_Value("ApiKey", configuration)
            End Get
        End Property

        Public ReadOnly Property Secret(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IExpediaRapidSettings.Secret
            Get
                Return Get_Value("Secret", configuration)
            End Get
        End Property

        Public ReadOnly Property Scheme(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IExpediaRapidSettings.Scheme
            Get
                Return Get_Value("Scheme", configuration)
            End Get
        End Property

        Public ReadOnly Property Host(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IExpediaRapidSettings.Host
            Get
                Return Get_Value("Host", configuration)
            End Get
        End Property

        Public ReadOnly Property SearchPath(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IExpediaRapidSettings.SearchPath
            Get
                Return Get_Value("SearchPath", configuration)
            End Get
        End Property

        Public ReadOnly Property PaymentTerms(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IExpediaRapidSettings.PaymentTerms
            Get
                Return Get_Value("PaymentTerms", configuration)
            End Get
        End Property

        Public ReadOnly Property PartnerPointOfSale(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IExpediaRapidSettings.PartnerPointOfSale
            Get
                Return Get_Value("PartnerPointOfSale", configuration)
            End Get
        End Property

        Public ReadOnly Property LanguageCode(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IExpediaRapidSettings.LanguageCode
            Get
                Return Get_Value("LanguageCode", configuration)
            End Get
        End Property

        Public ReadOnly Property SearchRequestBatchSize(tpAttributeSearch As IThirdPartyAttributeSearch) As Integer Implements IExpediaRapidSettings.SearchRequestBatchSize
            Get
                Return Get_Value("SearchRequestBatchSize", configuration).ToSafeInt()
            End Get
        End Property

        Public ReadOnly Property CountryCode(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IExpediaRapidSettings.CountryCode
            Get
                Return Get_Value("CountryCode", configuration)
            End Get
        End Property

        Public ReadOnly Property SalesChannel(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IExpediaRapidSettings.SalesChannel
            Get
                Return Get_Value("SalesChannel", configuration)
            End Get
        End Property

        Public ReadOnly Property SalesEnvironment(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IExpediaRapidSettings.SalesEnvironment
            Get
                Return Get_Value("SalesEnvironment", configuration)
            End Get
        End Property

        Public ReadOnly Property SortType(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IExpediaRapidSettings.SortType
            Get
                Return Get_Value("SortType", configuration)
            End Get
        End Property

        Public ReadOnly Property RatePlanCount(tpAttributeSearch As IThirdPartyAttributeSearch) As Integer Implements IExpediaRapidSettings.RatePlanCount
            Get
                Return Get_Value("RatePlanCount", configuration).ToSafeInt()
            End Get
        End Property

        Public ReadOnly Property UseGZIP(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean Implements IExpediaRapidSettings.UseGZIP
            Get
                Return Get_Value("UseGZIP", configuration).ToSafeBoolean()
            End Get
        End Property

        Public ReadOnly Property AllowCancellations(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean Implements IExpediaRapidSettings.AllowCancellations
            Get
                Return Get_Value("AllowCancellations", configuration).ToSafeBoolean()
            End Get
        End Property

        Public ReadOnly Property OffsetCancellationDays(tpAttributeSearch As IThirdPartyAttributeSearch) As Integer Implements IExpediaRapidSettings.OffsetCancellationDays
            Get
                Return Get_Value("OffsetCancellationDays", configuration).ToSafeInt()
            End Get
        End Property

        Public ReadOnly Property UserAgent(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IExpediaRapidSettings.UserAgent
            Get
                Return Get_Value("UserAgent", configuration)
            End Get
        End Property
        Public ReadOnly Property BillingTerms(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IExpediaRapidSettings.BillingTerms
            Get
                Return Get_Value("Password", configuration)
            End Get
        End Property

        Public ReadOnly Property PlatformName(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IExpediaRapidSettings.PlatformName
            Get
                Return Get_Value("PlatformName", configuration)
            End Get
        End Property

        Public ReadOnly Property RateOption(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IExpediaRapidSettings.RateOption
            Get
                Return Get_Value("RateOption", configuration)
            End Get
        End Property

        Public ReadOnly Property ValidateAffiliateID(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean Implements IExpediaRapidSettings.ValidateAffiliateID
            Get
                Return Get_Value("ValidateAffiliateID", configuration).ToSafeBoolean()
            End Get
        End Property

    End Class

End Namespace