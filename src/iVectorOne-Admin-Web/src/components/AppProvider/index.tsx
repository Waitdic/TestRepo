import React, { Fragment, useEffect } from 'react';
import { Helmet } from 'react-helmet-async';
import { IntlProvider } from 'react-intl';
import { Route, Routes } from 'react-router-dom';
import { useDispatch } from 'react-redux';
//
import messages from '@/i18n/messages';
import NotFound from '@/layouts/NotFound';
import { CoreView } from '@/libs/core';
import { CustomerEdit } from '@/libs/core/customer/edit';
import Docs from '@/libs/core/docs';
import { ModuleCreate } from '@/libs/core/module/create';
import { ModuleEdit } from '@/libs/core/module/edit';
import { ModuleList } from '@/libs/core/module/list';
import { TenantCreate } from '@/libs/core/tenant/create';
import { TenantEdit } from '@/libs/core/tenant/edit';
import { TenantList } from '@/libs/core/tenant/list';
import { IvoView } from '@/libs/ivo';
import { ProviderCreate } from '@/libs/ivo/provider/create';
import { ProviderEdit } from '@/libs/ivo/provider/edit';
import { ProviderList } from '@/libs/ivo/provider/list';
import { SubscriptionCreate } from '@/libs/ivo/subscription/create';
import { SubscriptionEdit } from '@/libs/ivo/subscription/edit';
import { SubscriptionList } from '@/libs/ivo/subscription/list';
import { Module, Provider, Subscription, Tenant } from '@/types';
import { useCoreFetching } from '@/libs/core/data-access';
import { useIvoFetching } from '@/libs/ivo/data-access';
//! Temp
import { dummyModuleList, dummyProviders } from '@/temp';

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
    setError: setCoreError,
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
          {/* Core Route */}
          <Route path='/core' element={<CoreView />} />
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
          {/* IVO Landing */}
          <Route
            path='/'
            element={<IvoView error={coreError} setError={setCoreError} />}
          />
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
                  subscriptions,
                  isLoading: ivoIsLoading,
                  error: ivoError,
                }}
              />
            }
          />
          <Route
            path='/ivo/subscription/edit/:slug'
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
          {/*//? Customer Edit (Dummy) */}
          <Route path='/customer/edit/:id' element={<CustomerEdit />} />
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
