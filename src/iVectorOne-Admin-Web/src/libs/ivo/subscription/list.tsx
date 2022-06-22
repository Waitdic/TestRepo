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

interface SubscriptionListItem {
  name: string;
  id: number;
  isActive?: boolean;
  actions?: { name: string; href: string }[];
}

type Props = {
  fetchedSubscriptionList: {
    subscriptions: Subscription[];
    error: string | null;
    isLoading: boolean;
  };
};

export const SubscriptionList: FC<Props> = memo(
  ({ fetchedSubscriptionList }) => {
    const mappedSubscriptionList = fetchedSubscriptionList.subscriptions?.map(
      ({ userName, subscriptionId }) => ({
        name: userName,
        id: subscriptionId,
      })
    );
    const [filteredSubscriptionList, setFilteredSubscriptionList] = useState<
      SubscriptionListItem[]
    >(mappedSubscriptionList);
    const [showError, setShowError] = useState<boolean>(false);

    const tableHeaderList = [
      { name: 'Name', align: 'left' },
      { name: 'Actions', align: 'right' },
    ];
    const tableBodyList: any[] = filteredSubscriptionList.map(
      ({ id, name }) => ({
        id,
        name,
        isActive: false, //! TODO: this property is not available in the response
        actions: [{ name: 'Edit', href: `/ivo/subscription/edit/${id}` }],
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
      setFilteredSubscriptionList(mappedSubscriptionList);
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
                      list={mappedSubscriptionList}
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
