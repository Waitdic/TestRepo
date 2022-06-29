import { memo, useState, useEffect, FC } from 'react';
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
        isActive: false,
      })
    );
    const [filteredSubscriptionList, setFilteredSubscriptionList] = useState<
      SubscriptionListItem[]
    >(mappedSubscriptionList);
    const [showError, setShowError] = useState<boolean>(false);
    const [filters, setFilters] = useState<DropdownFilterProps[]>([
      {
        name: 'Active',
        value: false,
      },
    ]);
    const [modalOpen, setModalOpen] = useState(false);

    const tableBodyList: any[] = filteredSubscriptionList.map(
      ({ id, name }) => ({
        id,
        name,
        isActive: false, //! TODO: this property is not available in the response
        actions: [{ name: 'Edit', href: `/subscriptions/edit/${id}` }],
      })
    );
    const tableEmptyState = {
      title: 'No subscriptions',
      description: 'Get started by creating a new subscription.',
      href: '/subscriptions/create',
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
                  <div className='flex gap-3'>
                    <Button
                      color={ButtonColors.OUTLINE}
                      text='Search'
                      onClick={() => {
                        setModalOpen(true);
                      }}
                    />
                    <ModalSearch
                      id='subscriptionSearch'
                      searchId='subscriptionSearch'
                      modalOpen={modalOpen}
                      setModalOpen={setModalOpen}
                    />
                    <DropdownFilter
                      align='right'
                      allItems={mappedSubscriptionList}
                      items={filteredSubscriptionList}
                      filters={filters}
                      setFilters={setFilters}
                      setFilteredItems={setFilteredSubscriptionList}
                      title='Filter'
                    />
                    <Button
                      text='New'
                      isLink
                      href='/subscriptions/create'
                      icon={
                        <svg
                          className='w-4 h-4 fill-current opacity-50 shrink-0 mr-1'
                          viewBox='0 0 16 16'
                        >
                          <path d='M15 7H9V1c0-.6-.4-1-1-1S7 .4 7 1v6H1c-.6 0-1 .4-1 1s.4 1 1 1h6v6c0 .6.4 1 1 1s1-.4 1-1V9h6c.6 0 1-.4 1-1s-.4-1-1-1z' />
                        </svg>
                      }
                    />
                  </div>
                </div>
                <CardList
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
