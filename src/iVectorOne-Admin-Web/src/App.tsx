import { useEffect, Fragment, memo, FC } from 'react';
import { Routes, Route } from 'react-router-dom';
import { Helmet } from 'react-helmet-async';
import { IntlProvider } from 'react-intl';
import { Amplify } from 'aws-amplify';
import { Authenticator, useAuthenticator } from '@aws-amplify/ui-react';
import { connect, useDispatch } from 'react-redux';
//
import { RootState } from './store';
import messages from '@/i18n/messages';
import { useCoreFetching } from '@/libs/core/data-access';
import { useIvoFetching } from '@/libs/ivo/data-access';
import NotFound from '@/layouts/NotFound';
//
import { CoreView } from '@/libs/core';
import { TenantList } from '@/libs/core/tenant/list';
import { TenantEdit } from '@/libs/core/tenant/edit';
import { TenantCreate } from '@/libs/core/tenant/create';
import { ModuleList } from '@/libs/core/module/list';
import { ModuleCreate } from '@/libs/core/module/create';
import { ModuleEdit } from '@/libs/core/module/edit';
import { CustomerEdit } from '@/libs/core/customer/edit';
import { IvoView } from '@/libs/ivo';
import { SubscriptionList } from '@/libs/ivo/subscription/list';
import { SubscriptionEdit } from '@/libs/ivo/subscription/edit';
import { SubscriptionCreate } from '@/libs/ivo/subscription/create';
import { ProviderList } from '@/libs/ivo/provider/list';
import { ProviderCreate } from '@/libs/ivo/provider/create';
import { ProviderEdit } from '@/libs/ivo/provider/edit';
import Docs from './libs/core/docs';
//
import '@aws-amplify/ui-react/styles.css';
//
import awsExports from './aws-exports';
import Header from './components/Amplify/Header';
import Footer from './components/Amplify/Footer';
import SignInHeader from './components/Amplify/SignIn/Header';
import SignInFooter from './components/Amplify/SignIn/Footer';
import {
  dummyFetchedUser,
  dummyModuleList,
  dummyProviders,
  dummySubscriptions,
  dummyTenantList,
} from './temp';
import { Module, Provider, Subscription, Tenant } from './types';
Amplify.configure(awsExports);

const mapState = (state: RootState) => ({
  app: state.app,
});

type StateProps = ReturnType<typeof mapState>;
type Props = StateProps;

const App: FC<Props> = ({ app }) => {
  const { user, signOut } = useAuthenticator();

  const username = user?.username || '';
  const { lang, theme } = app;

  const dispatch = useDispatch();

  //* Core Data Fetch
  const {
    user: fetchedUser,
    tenantList,
    moduleList,
    isLoading: coreIsLoading,
    error: coreError,
  } = useCoreFetching();

  //* IVO Data Fetch
  const {
    subscriptions,
    providers,
    isLoading: ivoIsLoading,
    error: ivoError,
  } = useIvoFetching();

  console.log(fetchedUser, tenantList, moduleList, subscriptions, providers);

  useEffect(() => {
    dispatch.app.setThemeColor(theme);
    dispatch.app.getUserByAwsJwtToken(username);
    dispatch.app.setSignOutCallback(signOut);
  }, []);

  // Temporary not used waiting for the API to be ready
  // useEffect(() => {
  //   if (fetchedUser) {
  //     dispatch.app.updateUser(fetchedUser);
  //   }
  //   if (moduleList.length) {
  //     dispatch.app.updateModuleList(moduleList);
  //   }
  //   if (tenantList.length) {
  //     dispatch.app.updateTenantList(tenantList);
  //   }
  // }, [fetchedUser, moduleList, tenantList]);

  // useEffect(() => {
  //   if (providers.length) {
  //     dispatch.app.updateProviders(providers);
  //   }
  //   if (subscriptions.length) {
  //     dispatch.app.updateSubscriptions(subscriptions);
  //   }
  // }, [providers, subscriptions]);
  //Temporary used waiting for the API to be ready
  useEffect(() => {
    if (dummyFetchedUser) {
      dispatch.app.updateUser(dummyFetchedUser);
    }
    if (dummyModuleList.length) {
      dispatch.app.updateModuleList(dummyModuleList);
    }
    if (dummyTenantList.length) {
      dispatch.app.updateTenantList(dummyTenantList);
    }
  }, [dummyFetchedUser, dummyModuleList, dummyTenantList]);

  useEffect(() => {
    if (dummyProviders.length) {
      dispatch.app.updateProviders(dummyProviders);
    }
    if (dummySubscriptions.length) {
      dispatch.app.updateSubscriptions(dummySubscriptions);
    }
  }, [dummyProviders, dummySubscriptions]);

  return (
    <Authenticator
      loginMechanisms={['email']}
      components={{
        Header,
        SignIn: {
          Header: SignInHeader,
          Footer: SignInFooter,
        },
        Footer,
      }}
    >
      {() => (
        <>
          <Helmet htmlAttributes={{ lang }} />

          <IntlProvider
            locale={lang}
            textComponent={Fragment}
            messages={messages[lang]}
            defaultLocale='en-US'
          >
            <Routes>
              {/* Root Route */}
              <Route path='/' element={<CoreView />} />

              {/* Tenant Routes */}
              <Route
                path='/tenant/list'
                element={
                  <TenantList
                    fetchedTenantList={{
                      // tenantList,
                      // isLoading: coreIsLoading,
                      // error: coreError,
                      tenantList: dummyTenantList as Tenant[],
                      isLoading: false,
                      error: null,
                    }}
                  />
                }
              />
              <Route
                path='/tenant/create'
                element={<TenantCreate error={null} />}
              />
              <Route
                path='/tenant/edit/:slug'
                element={
                  <TenantEdit
                    fetchedTenantList={{
                      // tenantList,
                      // isLoading: coreIsLoading,
                      // error: coreError,
                      tenantList: dummyTenantList as Tenant[],
                      isLoading: false,
                      error: null,
                    }}
                  />
                }
              />

              {/* Module Routes */}
              <Route
                path='/module/list'
                element={
                  <ModuleList
                    fetchedModuleList={{
                      // moduleList,
                      // isLoading: coreIsLoading,
                      // error: coreError,
                      moduleList: dummyModuleList as Module[],
                      isLoading: false,
                      error: null,
                    }}
                  />
                }
              />
              <Route
                path='/module/create'
                element={<ModuleCreate error={null} isLoading={false} />}
              />
              <Route
                path='/module/edit/:slug'
                element={<ModuleEdit error={null} isLoading={false} />}
              />

              {/* IVO Landing */}
              <Route path='/ivo' element={<IvoView />} />

              {/* Subscription Routes */}
              <Route
                path='/ivo/subscription/create'
                element={<SubscriptionCreate error={null} />}
              />
              <Route
                path='/ivo/subscription/list'
                element={
                  <SubscriptionList
                    fetchedSubscriptionList={{
                      // subscriptions,
                      // isLoading: ivoIsLoading,
                      // error: ivoError,
                      subscriptions: dummySubscriptions as Subscription[],
                      isLoading: false,
                      error: null,
                    }}
                  />
                }
              />
              <Route
                path='/ivo/subscription/edit/:slug'
                element={
                  <SubscriptionEdit
                    fetchedSubscriptionList={{
                      // subscriptions,
                      // isLoading: ivoIsLoading,
                      // error: ivoError,
                      subscriptions: dummySubscriptions as Subscription[],
                      isLoading: false,
                      error: null,
                    }}
                  />
                }
              />

              {/* Provider Routes */}
              <Route
                path='/ivo/provider/list'
                element={
                  <ProviderList
                    fetchedProviderList={{
                      // providers,
                      // isLoading: ivoIsLoading,
                      // error: ivoError,
                      providers: dummyProviders as Provider[],
                      isLoading: false,
                      error: null,
                    }}
                  />
                }
              />
              <Route
                path='/ivo/provider/create'
                element={<ProviderCreate error={null} />}
              />
              <Route
                path='/ivo/provider/edit/:slug'
                element={<ProviderEdit error={null} />}
              />

              {/* Customer Edit (Dummy) */}
              <Route path='/customer/edit/:id' element={<CustomerEdit />} />

              {/* Docs */}
              <Route path='/docs/:id' element={<Docs />} />

              {/* Not Found Route */}
              <Route path='*' element={<NotFound />} />
            </Routes>
          </IntlProvider>
        </>
      )}
    </Authenticator>
  );
};

export default connect(mapState)(memo(App));
