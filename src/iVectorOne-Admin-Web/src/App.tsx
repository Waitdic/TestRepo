import { useEffect, Fragment, memo, FC } from 'react';
import { Routes, Route } from 'react-router-dom';
import { Helmet } from 'react-helmet-async';
import { IntlProvider } from 'react-intl';
import { Amplify } from 'aws-amplify';
import { withAuthenticator } from '@aws-amplify/ui-react';
import { connect, useDispatch, useSelector } from 'react-redux';
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
//
import '@aws-amplify/ui-react/styles.css';
//
import awsExports from './aws-exports';
Amplify.configure(awsExports);

//! Temporary data for demo
import {
  dummyFetchedUser,
  dummyTenantList,
  dummyModuleList,
  dummySubscriptions,
  dummyProviders,
} from './temp';
import { Module, Provider, Subscription, Tenant } from './types';

const mapState = (state: RootState) => ({
  app: state.app,
});

type AmplifyProps = {
  signOut: () => void;
  user: { username: string };
};
type StateProps = ReturnType<typeof mapState>;
type Props = AmplifyProps & StateProps;

const App: FC<Props> = ({ signOut, user, app }) => {
  const { username } = user;
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

          {/* Not Found Route */}
          <Route path='*' element={<NotFound />} />
        </Routes>
      </IntlProvider>
    </>
  );
};

export default connect(mapState)(withAuthenticator(memo(App)));
