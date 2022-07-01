import { memo, useState, useEffect, FC, useMemo } from 'react';
//
import { DropdownFilterProps, Subscription } from '@/types';
import { ButtonColors, NotificationStatus } from '@/constants';
import MainLayout from '@/layouts/Main';
import {
  ErrorBoundary,
  Button,
  Notification,
  CardList,
  DropdownFilter,
  ModalSearch,
} from '@/components';
import { useSelector } from 'react-redux';
import { RootState } from '@/store';

interface SubscriptionListItem {
  name: string;
  id: number;
  isActive?: boolean;
  actions?: { name: string; href: string }[];
}

type Props = {};

export const SubscriptionList: FC<Props> = memo(({}) => {
  const subscriptions = useSelector(
    (state: RootState) => state.app.subscriptions
  );

  const [filteredSubscriptionList, setFilteredSubscriptionList] = useState<
    SubscriptionListItem[]
  >([]);
  const [filters, setFilters] = useState<DropdownFilterProps[]>([
    {
      name: 'Active',
      value: false,
    },
  ]);

  const tableBodyList: any[] = filteredSubscriptionList.map(({ id, name }) => ({
    id,
    name,
    isActive: false, //! TODO: this property is not available in the response
    actions: [{ name: 'Edit', href: `/subscriptions/${id}/edit` }],
  }));
  const tableEmptyState = {
    title: 'No subscriptions',
    description: 'Get started by creating a new subscription.',
    href: '/subscriptions/create',
    buttonText: 'New Subscription',
  };

  useEffect(() => {
    if (!!subscriptions?.length) {
      setFilteredSubscriptionList(
        subscriptions.map(({ userName, subscriptionId }) => ({
          name: userName,
          id: subscriptionId,
          isActive: false,
        }))
      );
    }
  }, [subscriptions]);

  return (
    <>
      <MainLayout>
        <div className='flex flex-col'>
          <div className='flex align-start justify-between mb-6'>
            <h1 className='text-2xl md:text-3xl text-slate-800 font-bold'>
              Subscriptions
            </h1>
            <div className='flex gap-3'>
              {/* <DropdownFilter
                      align='right'
                      allItems={mappedSubscriptionList}
                      items={filteredSubscriptionList}
                      filters={filters}
                      setFilters={setFilters}
                      setFilteredItems={setFilteredSubscriptionList}
                      title='Filter'
                    /> */}
              <Button text='New' isLink href='/subscriptions/create' />
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
    </>
  );
});
