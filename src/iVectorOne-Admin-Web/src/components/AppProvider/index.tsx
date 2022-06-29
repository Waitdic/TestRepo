import React, { Fragment, useEffect } from 'react';
import { Helmet } from 'react-helmet-async';
import { IntlProvider } from 'react-intl';
import { Route, Routes } from 'react-router-dom';
import { useDispatch } from 'react-redux';
//
import messages from '@/i18n/messages';
import NotFound from '@/layouts/NotFound';
import Docs from '@/libs/core/docs';
import { ModuleCreate } from '@/libs/core/module/create';
import { ModuleEdit } from '@/libs/core/module/edit';
import { ModuleList } from '@/libs/core/module/list';
import { TenantCreate } from '@/libs/core/tenant/create';
import { TenantEdit } from '@/libs/core/tenant/edit';
import { TenantList } from '@/libs/core/tenant/list';
import { Module, Provider } from '@/types';
import { useCoreFetching } from '@/libs/core/data-access';
import { useIvoFetching } from '@/libs/ivo/data-access';
import { IvoView } from '@/libs/ivo';
import { SubscriptionCreate } from '@/libs/ivo/subscription/create';
import { SubscriptionList } from '@/libs/ivo/subscription/list';
import { SubscriptionEdit } from '@/libs/ivo/subscription/edit';
import { ProviderList } from '@/libs/ivo/provider/list';
import { ProviderCreate } from '@/libs/ivo/provider/create';
import { ProviderEdit } from '@/libs/ivo/provider/edit';
//! Temp
import { dummyModuleList, dummyProviders } from '@/temp';
import MyAccount from '@/libs/core/settings/my-account';
import Feedback from '@/libs/core/settings/feedback';
import KnowledgeBase from '@/libs/core/support/knowledge-base';

type Props = {
  app: { theme: string; lang: string };
  user: { username?: string | undefined };
  signOut: () => void;
};

const AppProvider: React.FC<Props> = ({ app, user, signOut }) => {
  const dispatch = useDispatch();

  const { theme, lang } = app;
  const username = user?.username || null;

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

  useEffect(() => {
    dispatch.app.setThemeColor(theme);
    dispatch.app.getUserByAwsJwtToken({ user: username });
    dispatch.app.setSignOutCallback(signOut);
  }, []);

  useEffect(() => {
    if (fetchedUser) {
      dispatch.app.updateUser(fetchedUser);
    }
    // if (moduleList.length) {
    //   dispatch.app.updateModuleList(moduleList);
    // }
    if (tenantList.length) {
      dispatch.app.updateTenantList(tenantList);
    }
  }, [fetchedUser, moduleList, tenantList]);

  useEffect(() => {
    // if (providers.length) {
    //   dispatch.app.updateProviders(providers);
    // }
    if (subscriptions.length) {
      dispatch.app.updateSubscriptions(subscriptions);
    }
  }, [providers, subscriptions]);

  //Temporary used waiting for the API to be ready
  useEffect(() => {
    if (dummyModuleList.length) {
      dispatch.app.updateModuleList(dummyModuleList);
    }
  }, [dummyModuleList]);

  useEffect(() => {
    if (dummyProviders.length) {
      dispatch.app.updateProviders(dummyProviders);
    }
  }, [dummyProviders]);

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
          {/* Dashboard */}
          <Route path='/' element={<IvoView error={coreError} />} />
          {/* Tenant Routes */}
          <Route
            path='/tenant/list'
            element={
              <TenantList
                fetchedTenantList={{
                  tenantList,
                  isLoading: coreIsLoading,
                  error: coreError,
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
                  tenantList,
                  isLoading: coreIsLoading,
                  error: coreError,
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
          {/* Subscription Routes */}
          <Route
            path='/subscriptions/create'
            element={<SubscriptionCreate error={null} />}
          />
          <Route
            path='/subscriptions'
            element={
              <SubscriptionList
                fetchedSubscriptionList={{
                  subscriptions,
                  isLoading: ivoIsLoading,
                  error: ivoError,
                }}
              />
            }
          />
          <Route
            path='/subscriptions/edit/:slug'
            element={
              <SubscriptionEdit
                fetchedSubscriptionList={{
                  subscriptions,
                  isLoading: ivoIsLoading,
                  error: ivoError,
                }}
              />
            }
          />
          {/* Provider Routes */}
          <Route
            path='/providers'
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
            path='/providers/create'
            element={<ProviderCreate error={null} />}
          />
          <Route
            path='/providers/edit/:slug'
            element={<ProviderEdit error={null} />}
          />
          {/* Settings */}
          <Route path='/settings/my-account' element={<MyAccount />} />
          <Route path='/settings/feedback' element={<Feedback />} />
          {/* Support */}
          <Route path='/support/knowledge-base' element={<KnowledgeBase />} />
          {/* Docs */}
          <Route path='/docs/:id' element={<Docs />} />
          {/* Not Found Route */}
          <Route path='*' element={<NotFound />} />
        </Routes>
      </IntlProvider>
    </>
  );
};

export default AppProvider;
