import React, { Fragment, useEffect } from 'react';
import { Helmet } from 'react-helmet-async';
import { IntlProvider } from 'react-intl';
import { Route, Routes } from 'react-router-dom';
import { useDispatch } from 'react-redux';
//
import messages from '@/i18n/messages';
import NotFound from '@/layouts/NotFound';
import { ModuleCreate } from '@/libs/core/module/create';
import { ModuleEdit } from '@/libs/core/module/edit';
import { ModuleList } from '@/libs/core/module/list';
import { TenantCreate } from '@/libs/core/tenant/create';
import { TenantEdit } from '@/libs/core/tenant/edit';
import { TenantList } from '@/libs/core/tenant/list';
import { Module } from '@/types';
import { useCoreFetching, useIvoFetching } from '@/libs/core/data-access';
import { Dashboard } from '@/libs/core';
import { SubscriptionCreate } from '@/libs/core/subscription/create';
import { SubscriptionList } from '@/libs/core/subscription/list';
import { SubscriptionEdit } from '@/libs/core/subscription/edit';
import { ProviderList } from '@/libs/core/provider/list';
import { ProviderCreate } from '@/libs/core/provider/create';
import { ProviderEdit } from '@/libs/core/provider/edit';
import MyAccount from '@/libs/core/settings/my-account';
import Feedback from '@/libs/core/settings/feedback';
import KnowledgeBase from '@/libs/core/support/knowledge-base';
import ChangeLog from '@/libs/core/support/change-log';
import RoadMap from '@/libs/core/support/road-map';
//! Temp
import { dummyModuleList } from '@/temp';

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
  const { isLoading: coreIsLoading, error: coreError } = useCoreFetching();

  //* IVO Data Fetch
  const { isLoading: ivoIsLoading, error: ivoError } = useIvoFetching();

  useEffect(() => {
    dispatch.app.setThemeColor(theme);
    dispatch.app.getUserByAwsJwtToken({ user: username });
    dispatch.app.setSignOutCallback(signOut);
  }, []);

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
          <Route path='/' element={<Dashboard error={coreError} />} />
          {/* Tenant Routes */}
          <Route
            path='/tenant/list'
            element={<TenantList isLoading={coreIsLoading} error={coreError} />}
          />
          <Route
            path='/tenant/create'
            element={<TenantCreate error={null} />}
          />
          <Route
            path='/tenant/edit/:slug'
            element={<TenantEdit isLoading={coreIsLoading} error={coreError} />}
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
          <Route path='/subscriptions' element={<SubscriptionList />} />
          <Route
            path='/subscriptions/:slug/edit'
            element={<SubscriptionEdit />}
          />
          {/* Provider Routes */}
          <Route path='/providers' element={<ProviderList />} />
          <Route
            path='/providers/create'
            element={<ProviderCreate error={null} />}
          />
          <Route
            path='/providers/:slug/edit'
            element={<ProviderEdit error={null} />}
          />
          {/* Settings */}
          <Route path='/settings/my-account' element={<MyAccount />} />
          <Route path='/settings/feedback' element={<Feedback />} />
          {/* Support */}
          <Route path='/support/knowledge-base' element={<KnowledgeBase />} />
          <Route path='/support/change-log' element={<ChangeLog />} />
          <Route path='/support/road-map' element={<RoadMap />} />
          {/* Not Found Route */}
          <Route path='*' element={<NotFound />} />
        </Routes>
      </IntlProvider>
    </>
  );
};

export default AppProvider;
