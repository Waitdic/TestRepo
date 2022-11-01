import React, { useCallback, useEffect, useMemo, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { sortBy } from 'lodash';
//
import type { Account, ChartData, MultiLevelTableData } from '@/types';
import { RootState } from '@/store';
import { NotificationStatus } from '@/constants';
import MainLayout from '@/layouts/Main';
import {
  ChartCard,
  MultiLevelTable,
  Select,
  WelcomeBanner,
} from '@/components';
import { getDashboardChartData } from './data-access/dashboard';
import { getAccounts } from './data-access/account';
import mapChartData from '@/utils/mapChartData';

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

  const activeTenant = useMemo(
    () => user?.tenants?.find((t) => t.isSelected),
    [user]
  );

  const handleChangeAccount = useCallback(
    (accountId: number) => {
      if (!accounts) return;
      const newAccounts = accounts.map((a) => ({
        ...a,
        isSelected: a.accountId === accountId,
      }));
      setAccounts(newAccounts);
    },
    [accounts]
  );

  const fetchAccounts = useCallback(async () => {
    if (!activeTenant) return;
    await getAccounts(
      { id: activeTenant.tenantId, key: activeTenant.tenantKey },
      userKey as string,
      () => {
        dispatch.app.setIsLoading(true);
      },
      (fetchedAccounts) => {
        setAccounts(
          fetchedAccounts?.map((a, idx) => ({
            ...a,
            isSelected: idx === 0 ? true : false,
          }))
        );
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
        <div className='grid 2xl:grid-cols-4 gap-4 basis-full'>
          <div>
            {!!accounts?.length && (
              <Select
                id='account'
                name='account'
                labelText='Account'
                options={sortBy?.(accounts, [
                  function (o) {
                    return o?.userName?.toLowerCase?.();
                  },
                ])?.map((a) => ({
                  id: a.accountId,
                  name: a.userName,
                }))}
                onUncontrolledChange={handleChangeAccount}
              />
            )}
          </div>
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
          <MultiLevelTable data={supplierTableData} />
        </div>
      </div>
    </MainLayout>
  );
};

export default React.memo(Dashboard);
