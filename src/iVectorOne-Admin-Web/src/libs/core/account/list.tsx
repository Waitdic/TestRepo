import React, { useState, useEffect, useCallback, useMemo } from 'react';
import { sortBy } from 'lodash';
import { useDispatch, useSelector } from 'react-redux';
//
import { DropdownFilterProps } from '@/types';
import { RootState } from '@/store';
import MainLayout from '@/layouts/Main';
import { CardList } from '@/components';
import { getAccounts } from '../data-access/account';
import { NotificationStatus } from '@/constants';

interface AccountListItem {
  name: string;
  id: number;
  isActive?: boolean;
  actions?: { name: string; href: string }[];
}

type Props = {};

const tableEmptyState = {
  title: 'No Accounts',
  description: ['Get started by creating a new account.'],
  href: '/accounts/create',
  buttonText: 'New Account',
};

const AccountList: React.FC<Props> = () => {
  const dispatch = useDispatch();
  const user = useSelector((state: RootState) => state.app.user);
  const userKey = useSelector(
    (state: RootState) => state.app.awsAmplify.username
  );
  const accounts = useSelector((state: RootState) => state.app.accounts);
  const isLoading = useSelector((state: RootState) => state.app.isLoading);

  const [filteredAccountList, setFilteredAccountList] = useState<
    AccountListItem[]
  >([]);
  const [_filters, _setFilters] = useState<DropdownFilterProps[]>([
    {
      name: 'Active',
      value: false,
    },
  ]);

  const activeTenant = useMemo(
    () => user?.tenants.find((tenant: any) => tenant.isSelected),
    [user?.tenants]
  );

  const tableBodyList: any[] = filteredAccountList.map(
    ({ id, name, isActive }) => ({
      id,
      name,
      isActive: isActive === false, //? API not returning this field
      actions: [
        // { name: 'View', href: `/accounts/${id}` },
        { name: 'Edit', href: `/accounts/${id}/edit` },
      ],
    })
  );

  const fetchData = useCallback(async () => {
    if (!activeTenant) return;
    await getAccounts(
      { id: activeTenant.tenantId, key: activeTenant.tenantKey },
      userKey as string,
      () => {
        dispatch.app.setIsLoading(true);
      },
      (fetchedAccounts) => {
        dispatch.app.updateAccounts(fetchedAccounts);
        dispatch.app.setIsLoading(false);
      },
      (err, instance) => {
        dispatch.app.setNotification({
          status: NotificationStatus.ERROR,
          message: err,
          instance,
        });
        dispatch.app.setIsLoading(false);
      }
    );
  }, [activeTenant]);

  useEffect(() => {
    if (!!accounts?.length) {
      setFilteredAccountList(
        sortBy(accounts, 'userName').map(({ userName, accountId }) => ({
          name: userName,
          id: accountId,
          isActive: false,
        }))
      );
    }
  }, [accounts]);

  useEffect(() => {
    if (!!activeTenant) {
      fetchData();
    }
  }, [activeTenant, fetchData]);

  return (
    <>
      <MainLayout title='Accounts' addNew addNewHref='/accounts/create'>
        <CardList
          bodyList={tableBodyList}
          isLoading={isLoading}
          emptyState={tableEmptyState}
        />
      </MainLayout>
    </>
  );
};

export default React.memo(AccountList);
