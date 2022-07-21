import { memo, useState, useEffect, FC, useCallback, useMemo } from 'react';
import { sortBy } from 'lodash';
import { useDispatch, useSelector } from 'react-redux';
//
import { DropdownFilterProps } from '@/types';
import { RootState } from '@/store';
import MainLayout from '@/layouts/Main';
import { Button, CardList, Notification } from '@/components';
import { getSubscriptions } from '../data-access';
import { NotificationStatus } from '@/constants';

interface SubscriptionListItem {
  name: string;
  id: number;
  isActive?: boolean;
  actions?: { name: string; href: string }[];
}

type Props = {};

const tableEmptyState = {
  title: 'No subscriptions',
  description: 'Get started by creating a new subscription.',
  href: '/subscriptions/create',
  buttonText: 'New Subscription',
};

export const SubscriptionList: FC<Props> = memo(() => {
  const dispatch = useDispatch();
  const user = useSelector((state: RootState) => state.app.user);
  const error = useSelector((state: RootState) => state.app.error);
  const subscriptions = useSelector(
    (state: RootState) => state.app.subscriptions
  );

  const [showNotification, setShowNotification] = useState(false);
  const [filteredSubscriptionList, setFilteredSubscriptionList] = useState<
    SubscriptionListItem[]
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

  const tableBodyList: any[] = filteredSubscriptionList.map(({ id, name }) => ({
    id,
    name,
    isActive: false, //! TODO: this property is not available in the response
    actions: [
      { name: 'View', href: `/subscriptions/${id}` },
      { name: 'Edit', href: `/subscriptions/${id}/edit` },
    ],
  }));

  const fetchData = useCallback(async () => {
    if (!activeTenant || activeTenant == null) return;
    await getSubscriptions(
      { id: activeTenant.tenantId, key: activeTenant.tenantKey },
      () => {
        dispatch.app.setIsLoading(true);
      },
      (fetchedSubscriptions) => {
        dispatch.app.updateSubscriptions(fetchedSubscriptions);
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
    if (!!subscriptions?.length) {
      setFilteredSubscriptionList(
        sortBy(subscriptions, 'userName').map(
          ({ userName, subscriptionId }) => ({
            name: userName,
            id: subscriptionId,
            isActive: false,
          })
        )
      );
    }
  }, [subscriptions]);

  useEffect(() => {
    if (!!activeTenant) {
      fetchData();
    }
  }, [activeTenant]);

  return (
    <>
      <MainLayout>
        <div className='flex flex-col'>
          <div className='flex align-start justify-between mb-6'>
            <h1 className='text-2xl md:text-3xl text-slate-800 font-bold'>
              Subscriptions
            </h1>
            <div className='flex gap-3'>
              <Button text='New' isLink href='/subscriptions/create' disabled />
            </div>
          </div>
          <CardList
            bodyList={tableBodyList}
            isLoading={!subscriptions.length}
            emptyState={tableEmptyState}
            statusIsPlaceholder
          />
        </div>
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
