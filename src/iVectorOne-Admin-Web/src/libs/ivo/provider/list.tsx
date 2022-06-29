import { memo, useState, useEffect, FC } from 'react';
//
import { Provider } from '@/types';
import { NotificationStatus } from '@/constants';
import MainLayout from '@/layouts/Main';
import {
  EmptyState,
  ErrorBoundary,
  TableList,
  Button,
  Spinner,
  Notification,
  SearchField,
} from '@/components';

type Props = {
  fetchedProviderList: {
    providers: Provider[];
    error: string | null;
    isLoading: boolean;
  };
};

export const ProviderList: FC<Props> = memo(({ fetchedProviderList }) => {
  const [filteredProviderList, setFilteredProviderList] = useState<Provider[]>(
    fetchedProviderList.providers
  );
  const [showError, setShowError] = useState<boolean>(false);

  const tableHeaderList = [
    { name: 'Name', align: 'left' },
    { name: 'Actions', align: 'right' },
  ];
  const tableEmptyState = {
    title: 'No providers',
    description: 'Get started by creating a new provider.',
    href: '/provider/create',
    buttonText: 'New Provider',
  };

  useEffect(() => {
    if (fetchedProviderList.error) {
      setShowError(true);
    } else {
      setShowError(false);
    }
    setFilteredProviderList(fetchedProviderList.providers);
  }, [fetchedProviderList]);

  return (
    <>
      <MainLayout>
        <div className='flex flex-col col-span-12'>
          {/* Providers */}
          {fetchedProviderList.error ? (
            <ErrorBoundary />
          ) : (
            <>
              <div className='flex align-start justify-end mb-6'>
                <div className='flex'>
                  <SearchField
                    list={fetchedProviderList.providers}
                    setList={setFilteredProviderList}
                  />
                  <Button
                    text='New'
                    isLink
                    href='/provider/create'
                    className='ml-3'
                  />
                </div>
              </div>
              <div className='flex flex-col gap-8'>
                {!fetchedProviderList.isLoading &&
                filteredProviderList.length ? (
                  filteredProviderList.map(
                    ({ name: subscriptionName, providers }) => (
                      <div key={subscriptionName}>
                        <div className='flex justify-between items-center mb-3'>
                          <h3 className='text-xl font-medium uppercase'>
                            {subscriptionName}
                          </h3>
                        </div>
                        <TableList
                          headerList={tableHeaderList}
                          bodyList={providers.map(({ name, isActive }) => ({
                            id: name,
                            name,
                            isActive,
                            actions: [
                              {
                                name: 'Edit',
                                href: `/provider/edit/${name}`,
                              },
                            ],
                          }))}
                          emptyState={tableEmptyState}
                        />
                      </div>
                    )
                  )
                ) : fetchedProviderList.isLoading ? (
                  <div className='p-4 text-center'>
                    <Spinner />
                  </div>
                ) : (
                  <EmptyState
                    title='No subscriptions'
                    description='Get started by creating a new subscription.'
                    href='/provider/create'
                    buttonText='New Subscription'
                  />
                )}
              </div>
            </>
          )}
        </div>
      </MainLayout>

      {showError && (
        <Notification
          title='Error'
          description={fetchedProviderList.error as string}
          show={showError}
          setShow={setShowError}
          status={NotificationStatus.ERROR}
          autoHide={false}
        />
      )}
    </>
  );
});
