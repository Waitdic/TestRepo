import { memo, useState, useEffect, FC } from 'react';
//
import { Subscription } from '@/types';
import { NotificationStatus } from '@/constants';
import MainLayout from '@/layouts/Main';
import {
  ErrorBoundary,
  TableList,
  Button,
  Notification,
  SearchField,
} from '@/components';

type Props = {
  fetchedSubscriptionList: {
    subscriptions: Subscription[];
    error: string | null;
    isLoading: boolean;
  };
};

export const SubscriptionList: FC<Props> = memo(
  ({ fetchedSubscriptionList }) => {
    const [filteredSubscriptionList, setFilteredSubscriptionList] = useState<
      Subscription[]
    >(fetchedSubscriptionList.subscriptions);
    const [showError, setShowError] = useState<boolean>(false);

    const tableHeaderList = [
      { name: 'Name', align: 'left' },
      { name: 'Actions', align: 'right' },
    ];
    const tableBodyList = filteredSubscriptionList.map(
      ({ key, name, isActive }) => ({
        id: key,
        name,
        isActive,
        actions: [{ name: 'Edit', href: `/ivo/subscription/edit/${name}` }],
      })
    );
    const tableEmptyState = {
      title: 'No subscriptions',
      description: 'Get started by creating a new subscription.',
      href: '/ivo/subscription/create',
      buttonText: 'New Subscription',
    };

    useEffect(() => {
      if (fetchedSubscriptionList.error) {
        setShowError(true);
      } else {
        setShowError(false);
      }
      setFilteredSubscriptionList(fetchedSubscriptionList.subscriptions);
    }, [fetchedSubscriptionList]);

    return (
      <>
        <MainLayout>
          <div className='flex flex-col'>
            {/* Subscription */}
            {typeof fetchedSubscriptionList.error === 'string' ? (
              <ErrorBoundary />
            ) : (
              <>
                <div className='flex align-start justify-end mb-6'>
                  <div className='flex'>
                    <SearchField
                      list={fetchedSubscriptionList.subscriptions}
                      setList={setFilteredSubscriptionList}
                    />
                    <Button
                      text='New'
                      isLink
                      href='/ivo/subscription/create'
                      className='ml-3'
                    />
                  </div>
                </div>
                <TableList
                  headerList={tableHeaderList}
                  bodyList={tableBodyList}
                  isLoading={fetchedSubscriptionList.isLoading}
                  emptyState={tableEmptyState}
                />
              </>
            )}
          </div>
        </MainLayout>

        {showError && (
          <Notification
            title='Error'
            description={fetchedSubscriptionList.error as string}
            show={showError}
            setShow={setShowError}
            status={NotificationStatus.ERROR}
            autoHide={false}
          />
        )}
      </>
    );
  }
);
