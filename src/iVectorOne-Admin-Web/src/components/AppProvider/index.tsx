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
import { useCoreFetching } from '@/libs/core/data-access';
import { Dashboard } from '@/libs/core';
import { SubscriptionCreate } from '@/libs/core/subscription/create';
import { SubscriptionList } from '@/libs/core/subscription/list';
import { SubscriptionEdit } from '@/libs/core/subscription/edit';
import { SupplierList } from '@/libs/core/supplier/list';
import { SupplierCreate } from '@/libs/core/supplier/create';
import { SupplierEdit } from '@/libs/core/supplier/edit';
import MyAccount from '@/libs/core/settings/my-account';
import Feedback from '@/libs/core/settings/feedback';
import KnowledgeBase from '@/libs/core/support/knowledge-base';
import ChangeLog from '@/libs/core/support/change-log';
import RoadMap from '@/libs/core/support/road-map';
//! Temp
import { dummyModuleList } from '@/temp';
import { SubscriptionView } from '@/libs/core/subscription/view';

type Props = {
  app: { theme: string; lang: string };
  user: { username: string | undefined };
};

const AppProvider: React.FC<Props> = ({ app, user }) => {
  const dispatch = useDispatch();

  const { theme, lang } = app;
  const username = user?.username || null;

  //* Core Data Fetch
  const { error: coreError } = useCoreFetching();

  useEffect(() => {
    dispatch.app.setThemeColor(theme);
    dispatch.app.getUserByAwsJwtToken({ user: username });
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
            element={<TenantList error={coreError} />}
          />
          <Route
            path='/tenant/create'
            element={<TenantCreate error={null} />}
          />
          <Route
            path='/tenant/edit/:slug'
            element={<TenantEdit error={coreError} />}
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
            element={<SubscriptionCreate />}
          />
          <Route path='/subscriptions' element={<SubscriptionList />} />
          <Route
            path='/subscriptions/:slug/edit'
            element={<SubscriptionEdit />}
          />
          <Route path='/subscriptions/:slug' element={<SubscriptionView />} />
          {/* Supplier Routes */}
          <Route path='/suppliers' element={<SupplierList />} />
          <Route path='/suppliers/create' element={<SupplierCreate />} />
          <Route path='/suppliers/:slug/edit' element={<SupplierEdit />} />
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
