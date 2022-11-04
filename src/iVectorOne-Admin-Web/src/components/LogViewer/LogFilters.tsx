import React, { useCallback, useEffect, useMemo, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { sortBy } from 'lodash';
//
import type { Account, LogEntries, LogViewerFilters } from '@/types';
import { getAccounts } from '@/libs/core/data-access/account';
import { RootState } from '@/store';
import { NotificationStatus } from '@/constants';
import {
  Button,
  Datepicker,
  Select,
  Tabs,
  UncontrolledTextField,
} from '@/components';
import {
  getBookingsLogEntries,
  getFilteredLogEntries,
} from '@/libs/log-viewer/data-access';

type Props = {
  setResults: React.Dispatch<React.SetStateAction<LogEntries[]>>;
};

const LogFilters: React.FC<Props> = ({ setResults }) => {
  const dispatch = useDispatch();

  const user = useSelector((state: RootState) => state.app.user);
  const userKey = useSelector(
    (state: RootState) => state.app.awsAmplify.username
  );
  const isLoading = useSelector((state: RootState) => state.app.isLoading);

  const [searchQuery, setSearchQuery] = useState('');
  const [filters, setFilters] = useState<LogViewerFilters>({
    accountId: -1,
    logDateRange: [
      new Date(),
      new Date(new Date().setDate(new Date().getDate() + 1)),
    ],
    supplier: 'all',
    system: 'all',
    type: 'all',
    responseSuccess: 'all',
  });
  const [accounts, setAccounts] = useState<Account[]>([]);
  const [tabs, setTabs] = useState([
    {
      name: 'Filter',
      href: 'filter',
      current: true,
    },
    {
      name: 'Booking Search',
      href: 'search',
      current: false,
    },
  ]);

  const activeTenant = useMemo(
    () => user?.tenants?.find((t) => t.isSelected),
    [user]
  );
  const isActiveTab = (name: string) => {
    return tabs.find((t) => t.current)?.href === name;
  };

  const handleOnTabChange = (href: string) => {
    setTabs((prev) =>
      prev.map((t) => ({
        ...t,
        current: t.href === href,
      }))
    );
  };

  const handleChangeLogDateRange = useCallback(
    (date: Date[] | Date) => {
      setFilters((prevFilters) => ({
        ...prevFilters,
        logDateRange: date,
      }));
    },
    [setFilters]
  );

  const handleOnFilterChange = (name: string, optionId: number) => {
    setFilters((prevFilters: any) => ({
      ...prevFilters,
      [name]: optionId,
    }));
  };

  const handleOnSearchQueryChange = (
    e: React.ChangeEvent<HTMLInputElement>
  ) => {
    const { value } = e.target;
    setSearchQuery(value);
  };

  const handleOnSearch = useCallback(async () => {
    if (isLoading || !activeTenant || !userKey) return;
    if (searchQuery.length < 3) {
      dispatch.app.setNotification({
        status: NotificationStatus.ERROR,
        message: 'Search query must be at least 3 characters long',
      });
      return;
    }

    await getBookingsLogEntries({
      tenant: { id: activeTenant.tenantId, key: activeTenant.tenantKey },
      userKey,
      accountId: filters.accountId,
      searchQuery,
      onInit: () => {
        dispatch.app.setIsLoading(true);
      },
      onSuccess: (logEntries) => {
        dispatch.app.setIsLoading(false);
        setResults(logEntries);
      },
      onFailed: (message) => {
        dispatch.app.setIsLoading(false);
        dispatch.app.setNotification({
          status: NotificationStatus.ERROR,
          message,
        });
      },
    });
  }, [isLoading, activeTenant, userKey, filters, searchQuery]);

  const handleOnLogRefresh = useCallback(async () => {
    if (!activeTenant || !userKey || filters.accountId === -1) return;

    await getFilteredLogEntries({
      tenant: { id: activeTenant.tenantId, key: activeTenant.tenantKey },
      userKey,
      accountId: filters.accountId,
      filters,
      onInit: () => {
        dispatch.app.setIsLoading(true);
      },
      onSuccess: (logEntries) => {
        dispatch.app.setIsLoading(false);
        setResults(logEntries);
      },
      onFailed: (message) => {
        dispatch.app.setIsLoading(false);
        dispatch.app.setNotification({
          status: NotificationStatus.ERROR,
          message,
        });
      },
    });
  }, [activeTenant, userKey, filters]);

  const fetchAccounts = useCallback(async () => {
    if (!activeTenant || !userKey) return;
    await getAccounts(
      {
        id: activeTenant?.tenantId,
        key: activeTenant?.tenantKey,
      },
      userKey,
      () => {
        dispatch.app.setIsLoading(true);
      },
      (accounts) => {
        dispatch.app.setIsLoading(false);
        setAccounts(accounts);
        setFilters((prev) => ({
          ...prev,
          accountId: accounts[0].accountId,
        }));
      },
      (error, instance) => {
        dispatch.app.setIsLoading(false);
        console.error(error, instance);
      }
    );
  }, [userKey, activeTenant]);

  useEffect(() => {
    fetchAccounts();

    return () => {
      setAccounts([]);
    };
  }, [fetchAccounts]);

  useEffect(() => {
    handleOnLogRefresh();
  }, [handleOnLogRefresh]);

  return (
    <div className='no-scrollbar relative px-3 pb-6 pt-0 border-b md:border-b-0 md:border-r border-slate-200 min-w-[380px] md:space-y-3'>
      <Tabs tabs={tabs} onTabChange={handleOnTabChange} />

      {isActiveTab('filter') && (
        <div className='grid md:grid-cols-2 lg:grid-cols-4 gap-3 pb-5'>
          <div>
            <Select
              id='accountId'
              name='accountId'
              labelText='Account'
              options={sortBy?.(accounts, [
                function (o) {
                  return o?.userName?.toLowerCase?.();
                },
              ])?.map((a) => ({
                id: a.accountId,
                name: a.userName,
              }))}
              onUncontrolledChange={(optionId) =>
                handleOnFilterChange('accountId', optionId)
              }
            />
          </div>
          <div>
            <Datepicker
              mode='range'
              label='Log Date Range'
              onChange={handleChangeLogDateRange}
            />
          </div>
          <div>
            <Select
              id='supplier'
              name='supplier'
              labelText='Supplier'
              options={[
                {
                  id: 'all',
                  name: 'All',
                },
              ]}
              defaultValue={{
                id: filters.supplier,
                name: filters.supplier as string,
              }}
              onUncontrolledChange={(optionId) =>
                handleOnFilterChange('supplier', optionId)
              }
            />
          </div>
          <div>
            <Select
              id='system'
              name='system'
              labelText='System'
              options={[
                {
                  id: 'all',
                  name: 'All',
                },
                {
                  id: 'Live Only',
                  name: 'Live Only',
                },
                {
                  id: 'Test Only',
                  name: 'Test Only',
                },
              ]}
              defaultValue={{
                id: filters.system,
                name: filters.system,
              }}
              onUncontrolledChange={(optionId) =>
                handleOnFilterChange('system', optionId)
              }
            />
          </div>
          <div>
            <Select
              id='type'
              name='type'
              labelText='Type'
              options={[
                {
                  id: 'all',
                  name: 'All',
                },
                {
                  id: 'Prebook Only',
                  name: 'Prebook Only',
                },
                {
                  id: 'Book Only',
                  name: 'Book Only',
                },
              ]}
              defaultValue={{
                id: filters.type,
                name: filters.type,
              }}
              onUncontrolledChange={(optionId) =>
                handleOnFilterChange('type', optionId)
              }
            />
          </div>
          <div>
            <Select
              id='responseSuccess'
              name='responseSuccess'
              labelText='Response Success'
              options={[
                {
                  id: 'all',
                  name: 'All',
                },
                {
                  id: 'Successful Only',
                  name: 'Successful Only',
                },
                {
                  id: 'Unsuccessful Only',
                  name: 'Unsuccessful Only',
                },
              ]}
              defaultValue={{
                id: filters.responseSuccess,
                name: filters.responseSuccess,
              }}
              onUncontrolledChange={(optionId) =>
                handleOnFilterChange('responseSuccess', optionId)
              }
            />
          </div>
          <div className='col-span-2 flex justify-end items-end'>
            <Button text='Refresh' onClick={handleOnLogRefresh} />
          </div>
        </div>
      )}
      {isActiveTab('search') && (
        <div className='grid lg:grid-cols-4'>
          <div className='col-span-full text-sm mb-2'>
            <p>
              Please input a booking reference, supplier booking reference or
              lead quest name
            </p>
          </div>
          <div className='col-span-2'>
            <UncontrolledTextField
              name='searchQuery'
              onChange={handleOnSearchQueryChange}
              value={searchQuery}
            />
          </div>
          <div className='col-span-full'>
            <Button text='Search' onClick={handleOnSearch} className='mt-5' />
          </div>
        </div>
      )}
    </div>
  );
};

export default React.memo(LogFilters);
