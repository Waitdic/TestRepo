export const countries = [
  { id: 'en', name: 'England' },
  { id: 'us', name: 'United States' },
  { id: 'hu', name: 'Hungary' },
];

export const userNavigation = [
  { name: 'Your Profile', href: `/customer/edit/1` },
  { name: 'Settings', href: '#' },
];

export const dummyFetchedUser = {
  fullName: 'William Never',
  tenants: [
    {
      isActive: true,
      name: 'Alihoco',
      tenantId: '89379',
    },
    {
      isActive: false,
      name: 'Goway',
      tenantId: '50566',
    },
    {
      isActive: false,
      name: 'Thomas Cook',
      tenantId: '22897',
    },
  ],
};

export const dummyTenantList = [
  { tenantId: '95897', name: 'Alihoco' },
  { tenantId: '43484', name: 'Goway' },
  { tenantId: '52536', name: 'Thomas Cook' },
];

export const dummyModuleList = [
  {
    consoles: [
      {
        name: 'Tenant',
        uri: '/tenant/list',
        roles: [{ name: 'tenant-administrator' }],
      },
      {
        name: 'Module',
        uri: '/module/list',
        roles: [{ name: 'module-administrator' }],
      },
    ],
    isActive: false,
    moduleId: '18472',
    name: 'Core',
    uri: '/core',
  },
  {
    consoles: [
      {
        name: 'Subscription',
        uri: '/ivo/subscription/list',
        roles: [{ name: 'subscription-administrator' }],
      },
      {
        name: 'Provider',
        uri: '/ivo/provider/list',
        roles: [{ name: 'provider-administrator' }],
      },
    ],
    isActive: true,
    moduleId: '18473',
    name: 'iVectorOne',
    uri: '/',
  },
];

export const dummySubscriptions = [
  {
    CurrencyCode: 'usd',
    LogMainSearchError: true,
    PropertyTPRequestLimit: 500,
    SearchTimeoutSeconds: 60,
    isActive: true,
    key: 'C65C1118C11F4C718C3A29104DD3E038',
    name: 'Alihoco Production',
    password: '',
    userName: '',
  },
  {
    CurrencyCode: 'usd',
    LogMainSearchError: true,
    PropertyTPRequestLimit: 500,
    SearchTimeoutSeconds: 60,
    isActive: true,
    key: 'C23F31958D4F49AE99F1018DD27D0965',
    name: 'Alihoco Test',
    password: '',
    userName: '',
  },
];

export const dummyProviders = [
  {
    id: 'test_id',
    name: 'Alihoco Production',
    providers: [
      { name: 'YouTravel', isActive: true },
      { name: 'Stuba', isActive: false },
      { name: 'Acerooms', isActive: true },
    ],
  },
  {
    id: 'test_prod_id',
    name: 'Alihoco Test',
    providers: [
      { name: 'YouTravel Prod', isActive: true },
      { name: 'Stuba', isActive: true },
      { name: 'Acerooms', isActive: true },
    ],
  },
];
