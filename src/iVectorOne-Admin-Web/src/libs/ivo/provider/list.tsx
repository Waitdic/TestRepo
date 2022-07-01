import { memo, useState, useEffect, FC, useMemo } from 'react';
//
import { Provider } from '@/types';
import MainLayout from '@/layouts/Main';
import {
  EmptyState,
  Button,
  Spinner,
  SearchField,
  CardList,
} from '@/components';
import { useSelector } from 'react-redux';
import { RootState } from '@/store';

type Props = {};

export const ProviderList: FC<Props> = memo(() => {
  const subscriptions = useSelector(
    (state: RootState) => state.app.subscriptions
  );
  const providers = useMemo(() => {
    return subscriptions.flatMap((subscription) => subscription.providers);
  }, [subscriptions]);

  const [filteredProviderList, setFilteredProviderList] =
    useState<Provider[]>(providers);

  const tableEmptyState = {
    title: 'No providers',
    description: 'Get started by creating a new provider.',
    href: '/providers/create',
    buttonText: 'New Provider',
  };

  useEffect(() => {
    setFilteredProviderList(providers);
  }, [providers]);

  if (!subscriptions?.length) {
    return null;
  }

  return (
    <>
      <MainLayout>
        <div className='flex flex-col'>
          <>
            <div className='flex align-start justify-end mb-6'>
              <div className='flex'>
                {/* <SearchField
                  list={providers}
                  setList={setFilteredProviderList}
                /> */}
                <Button
                  text='New'
                  isLink
                  href='/providers/create'
                  className='ml-3'
                />
              </div>
            </div>
            <div className='flex flex-col gap-8'>
              {filteredProviderList.length ? (
                <CardList
                  bodyList={filteredProviderList.map(
                    ({ name, supplierID }) => ({
                      id: supplierID,
                      name,
                      actions: [
                        {
                          name: 'Edit',
                          href: `/providers/${supplierID}/edit`,
                        },
                      ],
                    })
                  )}
                  emptyState={tableEmptyState}
                  statusIsPlaceholder
                />
              ) : (
                <EmptyState
                  title='No subscriptions'
                  description='Get started by creating a new subscription.'
                  href='/providers/create'
                  buttonText='New Subscription'
                />
              )}
            </div>
          </>
        </div>
      </MainLayout>
    </>
  );
});
