import { memo, useState, useEffect, FC, useCallback, useMemo } from 'react';
import { sortBy } from 'lodash';
import { useDispatch, useSelector } from 'react-redux';
//
import { DropdownFilterProps } from '@/types';
import { RootState } from '@/store';
import MainLayout from '@/layouts/Main';
import { CardList, Notification } from '@/components';
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

export const AccountList: FC<Props> = memo(() => {
  const dispatch = useDispatch();
  const user = useSelector((state: RootState) => state.app.user);
  const error = useSelector((state: RootState) => state.app.error);
  const accounts = useSelector((state: RootState) => state.app.accounts);

  const [showNotification, setShowNotification] = useState(false);
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

  const tableBodyList: any[] = filteredAccountList.map(({ id, name }) => ({
    id,
    name,
    isActive: false, //! TODO: this property is not available in the response
    actions: [
      { name: 'View', href: `/accounts/${id}` },
      { name: 'Edit', href: `/accounts/${id}/edit` },
    ],
  }));

  const fetchData = useCallback(async () => {
    if (!activeTenant || activeTenant == null) return;
    await getAccounts(
      { id: activeTenant.tenantId, key: activeTenant.tenantKey },
      () => {
        dispatch.app.setIsLoading(true);
      },
      (fetchedAccounts) => {
        dispatch.app.updateAccounts(fetchedAccounts);
        dispatch.app.setIsLoading(false);
      },
      (err) => {
        dispatch.app.setError(err);
        dispatch.app.setIsLoading(false);
      }
    );
  }, [activeTenant]);

  useEffect(() => {
    if (!!error) {
      setShowNotification(true);
    }
  }, [error]);

  useEffect(() => {
    if (!!accounts?.length) {
      setFilteredAccountList(
        sortBy(accounts, 'userName').map(({ userName, subscriptionId }) => ({
          name: userName,
          id: subscriptionId,
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
          isLoading={!accounts.length}
          emptyState={tableEmptyState}
          statusIsPlaceholder
        />
      </MainLayout>

      {showNotification && (
        <Notification
          title='Data fetching error'
          description={error as string}
          show={showNotification}
          setShow={setShowNotification}
          status={NotificationStatus.ERROR}
        />
      )}
    </>
  );
});
