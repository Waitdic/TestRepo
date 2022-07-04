import { memo, useState, useEffect, FC, useMemo } from 'react';
//
import { Provider, Subscription } from '@/types';
import MainLayout from '@/layouts/Main';
import {
  EmptyState,
  Button,
  Spinner,
  SearchField,
  CardList,
  SettingsSidebar,
} from '@/components';
import { useSelector } from 'react-redux';
import { RootState } from '@/store';
import { uniqBy } from 'lodash';
import classNames from 'classnames';

type Props = {};

export const ProviderList: FC<Props> = memo(() => {
  const subscriptions = useSelector(
    (state: RootState) => state.app.subscriptions
  );

  const [providers, setProviders] = useState<Provider[]>([]);
  const [filteredProviderList, setFilteredProviderList] = useState<Provider[]>(
    []
  );
  const [activeSub, setActiveSub] = useState<Subscription | null>(null);

  const tableEmptyState = {
    title: 'No providers',
    description: 'Get started by creating a new provider.',
    href: '/providers/create',
    buttonText: 'New Provider',
  };

  const handleSetActiveSub = (subId: number) => {
    if (!subscriptions?.length) return;
    const selectedSub = subscriptions.find(
      (sub) => sub.subscriptionId === subId
    );
    setActiveSub(selectedSub as Subscription);
    setProviders(selectedSub?.providers as Provider[]);
  };

  useEffect(() => {
    if (!!subscriptions?.length) {
      setActiveSub(subscriptions[0]);
      setProviders(subscriptions[0].providers);
    }
  }, [subscriptions]);

  useEffect(() => {
    setFilteredProviderList(providers);
  }, [providers]);

  if (!subscriptions?.length) {
    return null;
  }

  return (
    <>
      <MainLayout>
        <>
          {/* Page header */}
          <div className='mb-8'>
            {/* Title */}
            <h1 className='text-2xl md:text-3xl text-slate-800 font-bold'>
              Providers
            </h1>
          </div>

          {/* Content */}
          <div className='bg-white shadow-lg rounded-sm mb-8'>
            <div className='flex flex-col md:flex-row md:-mr-px gap-6'>
              <div className='flex flex-nowrap overflow-x-scroll no-scrollbar md:block md:overflow-auto px-3 py-6 border-b md:border-b-0 md:border-r border-slate-200 min-w-60 md:space-y-3'>
                <div>
                  <div className='text-xs font-semibold text-slate-400 uppercase mb-3'>
                    Subscriptions
                  </div>
                  <ul className='flex flex-nowrap md:block mr-3 md:mr-0'>
                    {subscriptions.map(({ subscriptionId, userName }) => (
                      <li
                        key={subscriptionId}
                        className={classNames(
                          'mr-0.5 md:mr-0 md:mb-0.5 flex items-center px-2.5 py-2 rounded whitespace-nowrap cursor-pointer',
                          {
                            'bg-indigo-50':
                              activeSub?.subscriptionId === subscriptionId,
                          }
                        )}
                        onClick={() => handleSetActiveSub(subscriptionId)}
                      >
                        <span
                          className={`text-sm font-medium ${
                            activeSub?.subscriptionId === subscriptionId
                              ? 'text-indigo-500'
                              : 'hover:text-slate-700'
                          }`}
                        >
                          {userName}
                        </span>
                      </li>
                    ))}
                  </ul>
                </div>
              </div>
              <div className='py-6 pr-6 w-full'>
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
            </div>
          </div>
        </>
      </MainLayout>
    </>
  );
});
