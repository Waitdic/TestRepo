import React, { Fragment, useEffect } from 'react';
import { IntlProvider } from 'react-intl';
import { Route, Routes } from 'react-router-dom';
import { useDispatch } from 'react-redux';
//
import messages from '@/i18n/messages';
import { useCoreFetching } from '@/libs/core/data-access';
import Dashboard from '@/libs/core/dashboard';
import NotFound from '@/layouts/NotFound';
import TenantCreate from '@/libs/core/tenant/create';
import TenantEdit from '@/libs/core/tenant/edit';
import TenantList from '@/libs/core/tenant/list';
import AccountCreate from '@/libs/core/account/create';
import AccountList from '@/libs/core/account/list';
import AccountEdit from '@/libs/core/account/edit';
import AccountView from '@/libs/core/account/view';
import SupplierList from '@/libs/core/supplier/list';
import SupplierCreate from '@/libs/core/supplier/create';
import SupplierEdit from '@/libs/core/supplier/edit';
import Contact from '@/libs/core/settings/contact';
import Feedback from '@/libs/core/settings/feedback';
import KnowledgeBase from '@/libs/core/support/knowledge-base';
import ChangeLog from '@/libs/core/support/change-log';
import RoadMap from '@/libs/core/support/road-map';
import UserCreate from '@/libs/core/user/create';
import UserList from '@/libs/core/user/list';
import UserView from '@/libs/core/user/view';
import Search from '@/libs/search';

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
      <IntlProvider
        locale={lang}
        textComponent={Fragment}
        messages={messages[lang]}
        defaultLocale='en-US'
      >
        <Routes>
          {/* Dashboard */}
          <Route path='/' element={<Dashboard error={coreError} />} />
          {/* Search */}
          <Route path='/search' element={<Search />} />
          {/* Tenant Routes */}
          <Route path='/tenants'>
            <Route index element={<TenantList />} />
            <Route path='create' element={<TenantCreate />} />
            <Route path=':slug'>
              <Route path='edit' element={<TenantEdit />} />
            </Route>
          </Route>
          {/* Account Routes */}
          <Route path='/accounts'>
            <Route index element={<AccountList />} />
            <Route path='create' element={<AccountCreate />} />
            <Route path=':slug'>
              <Route index element={<AccountView />} />
              <Route path='edit' element={<AccountEdit />} />
            </Route>
          </Route>
          {/* Supplier Routes */}
          <Route path='/suppliers'>
            <Route index element={<SupplierList />} />
            <Route path='create' element={<SupplierCreate />} />
            <Route path=':slug/edit' element={<SupplierEdit />} />
          </Route>
          {/* User Routes */}
          <Route path='/users'>
            <Route index element={<UserList />} />
            <Route path='create' element={<UserCreate />} />
            <Route path=':slug' element={<UserView />} />
          </Route>
          {/* Settings */}
          <Route path='/settings'>
            <Route path='contact' element={<Contact />} />
            <Route path='feedback' element={<Feedback />} />
          </Route>
          {/* Support */}
          <Route path='/support'>
            <Route path='knowledge-base' element={<KnowledgeBase />} />
            <Route path='change-log' element={<ChangeLog />} />
            <Route path='road-map' element={<RoadMap />} />
          </Route>
          {/* Not Found Route */}
          <Route path='*' element={<NotFound />} />
        </Routes>
      </IntlProvider>
    </>
  );
};

export default AppProvider;
