import React, { useCallback, useEffect, useMemo, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { sortBy, uniqBy } from 'lodash';
import moment from 'moment';
//
import type { Account, ChartData, MultiLevelTableData } from '@/types';
import { RootState } from '@/store';
import { NotificationStatus } from '@/constants';
import mapChartData from '@/utils/mapChartData';
import MainLayout from '@/layouts/Main';
import {
  Button,
  ChartCard,
  MultiLevelTable,
  Select,
  WelcomeBanner,
} from '@/components';
import { getDashboardChartData } from '../data-access/dashboard';
import { getAccounts } from '../data-access/account';

type Props = {
  error: string | null;
};

const Dashboard: React.FC<Props> = ({ error }) => {
  const dispatch = useDispatch();

  const user = useSelector((state: RootState) => state.app.user);
  const userKey = useSelector(
    (state: RootState) => state.app.awsAmplify.username
  );

  const [isIntermission, setIsIntermission] = useState(true);
  const [bookingsByHoursChartData, setBookingsByHoursChartData] =
    useState<ChartData | null>(null);
  const [searchesByHoursChartData, setSearchesByHoursChartData] =
    useState<ChartData | null>(null);
  const [summaryTableData, setSummaryTableData] = useState<
    MultiLevelTableData[] | null
  >(null);
  const [supplierTableData, setSupplierTableData] = useState<
    MultiLevelTableData[] | null
  >(null);
  const [accounts, setAccounts] = useState<Account[] | null>(null);
  const [supplierFilterByDate, setSupplierFilterByDate] = useState({
    id: moment(new Date()).format('DD MMM YY'),
    name: 'Today',
    isActive: true,
  });

  const activeTenant = useMemo(
    () => user?.tenants?.find((t) => t.isSelected),
    [user]
  );

  const filteredSupplierTableData = useMemo(() => {
    if (!supplierTableData) return null;

    const selectedDate = moment(
      new Date(supplierFilterByDate.id),
      'DD MM YYYY'
    ).format('YYYY-MM-DD');

    return supplierTableData.filter((d) => d.queryDate === selectedDate);
  }, [supplierTableData, supplierFilterByDate]);

  const supplierFilterByDateOptions: { id: any; name: string }[] =
    useMemo(() => {
      if (!supplierTableData) return [];
      const dateQueries = uniqBy(
        supplierTableData.map((supplier) => {
          return {
            id: supplier.queryDate,
            name: new Date(supplier.queryDate as string).toDateString(),
          };
        }),
        'id'
      );

      return sortBy?.(dateQueries, [
        function (o) {
          return o?.id;
        },
      ])
        ?.map(({ id, name }) => {
          const isToday =
            new Date(id as string).toDateString() === new Date().toDateString();
          const dateStr = moment(new Date(name)).format('DD MMM YY');
          return {
            id,
            name: isToday ? 'Today' : dateStr,
          };
        })
        .reverse();
    }, [supplierTableData]);
  const accountsOptions = useMemo(() => {
    return sortBy?.(accounts, [
      function (o) {
        return o?.userName?.toLowerCase?.();
      },
    ])?.map((a) => ({
      id: a.accountId,
      name: a.userName,
    }));
  }, [accounts]);

  const handleChangeAccount = useCallback(
    (accountId: number) => {
      if (!accounts) return;
      const newAccounts = accounts.map((a) => ({
        ...a,
        isSelected: a.accountId === Number(accountId),
      }));
      setAccounts(newAccounts);
    },
    [accounts]
  );

  const handleChangeSupplierFilterByDate = useCallback((id: string) => {
    setSupplierFilterByDate({
      id,
      name: new Date(id).toDateString(),
      isActive: true,
    });
  }, []);

  const fetchAccounts = useCallback(async () => {
    if (!activeTenant) return;
    await getAccounts(
      { id: activeTenant.tenantId, key: activeTenant.tenantKey },
      userKey as string,
      () => {
        dispatch.app.setIsLoading(true);
      },
      (fetchedAccounts) => {
        const orderedAccounts = sortBy?.(fetchedAccounts, [
          function (o) {
            return o?.userName?.toLowerCase?.();
          },
        ])?.map((a, idx) => ({
          ...a,
          isSelected: idx === 0 ? true : false,
        }));
        setAccounts(orderedAccounts);
        dispatch.app.setIsLoading(false);
        setIsIntermission(false);
      },
      (err) => {
        dispatch.app.setError(err);
        dispatch.app.setIsLoading(false);
        setIsIntermission(false);
      }
    );
  }, [activeTenant]);

  const fetchChartData = useCallback(async () => {
    if (!userKey || !accounts || !activeTenant) return;

    const selectedAccount = accounts.find((a) => a.isSelected);
    if (!selectedAccount) return;

    await getDashboardChartData({
      userKey,
      tenant: {
        id: activeTenant.tenantId,
        key: activeTenant.tenantKey,
      },
      accountId: selectedAccount.accountId,
      onInit: () => {
        dispatch.app.setIsLoading(true);
      },
      onSuccess: (data) => {
        dispatch.app.setIsLoading(false);
        const { bookingsByHour, searchesByHour, summary, supplier } = data;
        setBookingsByHoursChartData(
          mapChartData(bookingsByHour, ['red', 'blue'])
        );
        setSearchesByHoursChartData(
          mapChartData(searchesByHour, ['red', 'blue'])
        );
        setSummaryTableData(summary);
        setSupplierTableData(supplier);
      },
      onFailed: (message, instance?) => {
        dispatch.app.setIsLoading(false);
        dispatch.app.setNotification({
          status: NotificationStatus.ERROR,
          message,
          instance,
        });
      },
    });
  }, [userKey, activeTenant, accounts, isIntermission]);

  useEffect(() => {
    fetchAccounts();
    return () => {
      setAccounts(null);
    };
  }, [fetchAccounts]);

  useEffect(() => {
    const timer = setTimeout(() => fetchChartData(), 500);

    return () => {
      setBookingsByHoursChartData(null);
      setSearchesByHoursChartData(null);
      setSummaryTableData(null);
      setSupplierTableData(null);
      clearTimeout(timer);
    };
  }, [fetchChartData]);

  return (
    <MainLayout>
      {error ? (
        <div className='mb-12 flex flex-col justify-center items-center'>
          <h1 className='text-4xl font-semibold mb-2'>Welcome to iVectorOne</h1>
          <p className='text-lg text-center'>
            Our team are just getting your account setup, please check back
            later. <br /> If you are still seeing this message after a few hours
            please contact our support team.
          </p>
        </div>
      ) : (
        <WelcomeBanner />
      )}
      <div className='flex flex-col 2xl:flex-row 2xl:flex-wrap gap-12'>
        <div className='flex 2xl:grid 2xl:grid-cols-4 gap-4 basis-full items-end'>
          <div className='flex-1'>
            {!!accounts?.length && (
              <Select
                id='account'
                name='account'
                labelText='Account'
                options={accountsOptions}
                onUncontrolledChange={handleChangeAccount}
              />
            )}
          </div>
          {summaryTableData && (
            <div>
              <Button text='Refresh' onClick={fetchChartData} />
            </div>
          )}
        </div>
        <div className='basis-full'>
          <MultiLevelTable data={summaryTableData} />
        </div>
        <div className='flex-1'>
          <ChartCard
            title='Bookings by Hour (Today vs Last Week)'
            chartData={bookingsByHoursChartData as ChartData}
          />
        </div>
        <div className='flex-1'>
          <ChartCard
            title='Searches by Hour (Today vs Last Week)'
            chartData={searchesByHoursChartData as ChartData}
          />
        </div>
        <div className='basis-full'>
          {supplierFilterByDateOptions.length > 0 && supplierTableData && (
            <div className='flex justify-between mb-5'>
              <h2 className='text-2xl font-semibold'>Supplier Performance</h2>
              <div>
                <Select
                  id='suppliersFilterByDate'
                  name='suppliersFilterByDate'
                  options={supplierFilterByDateOptions}
                  onUncontrolledChange={handleChangeSupplierFilterByDate}
                />
              </div>
            </div>
          )}
          <MultiLevelTable data={filteredSupplierTableData} />
        </div>
      </div>
    </MainLayout>
  );
};

export default React.memo(Dashboard);
