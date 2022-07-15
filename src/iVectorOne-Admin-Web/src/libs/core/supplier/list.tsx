import { memo, useState, useEffect, FC } from 'react';
import { sortBy } from 'lodash';
import classNames from 'classnames';
import { useNavigate } from 'react-router-dom';
import { useSelector } from 'react-redux';
//
import { Supplier, Subscription } from '@/types';
import MainLayout from '@/layouts/Main';
import { EmptyState, CardList } from '@/components';
import { RootState } from '@/store';

type Props = {};

export const SupplierList: FC<Props> = memo(() => {
  const navigate = useNavigate();

  const subscriptions = useSelector(
    (state: RootState) => state.app.subscriptions
  );
  const isLoading = useSelector((state: RootState) => state.app.isLoading);

  const [suppliers, setSuppliers] = useState<Supplier[]>([]);
  const [filteredSupplierList, setFilteredSupplierList] = useState<Supplier[]>(
    []
  );
  const [activeSub, setActiveSub] = useState<Subscription | null>(null);

  const tableEmptyState = {
    title: 'No suppliers',
    description: 'Get started by creating a new Supplier.',
    href: '/suppliers/create',
    buttonText: 'New Supplier',
  };

  const handleSetActiveSub = (subId: number) => {
    if (!subscriptions?.length) return;
    const selectedSub = subscriptions.find(
      (sub) => sub.subscriptionId === subId
    );
    setActiveSub(selectedSub as Subscription);
    setSuppliers(sortBy(selectedSub?.suppliers, 'supplierName'));
    const mainLayoutArea = document.getElementById('main-layout-area');
    if (!!mainLayoutArea?.scrollTop) {
      mainLayoutArea.scrollTop = 0;
    }
  };

  useEffect(() => {
    if (!!subscriptions?.length) {
      const sortedSubscriptions = sortBy(subscriptions, 'userName');
      setActiveSub(sortedSubscriptions[0]);
      setSuppliers(sortedSubscriptions[0].suppliers);
    }
  }, [subscriptions]);

  useEffect(() => {
    setFilteredSupplierList(suppliers);
  }, [suppliers]);

  useEffect(() => {
    if (!isLoading && !subscriptions?.length) {
      navigate('/');
    }
  }, [isLoading, subscriptions]);

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
              Suppliers
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
                    {sortBy(subscriptions, 'userName').map(
                      ({ subscriptionId, userName }) => (
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
                      )
                    )}
                  </ul>
                </div>
              </div>
              <div className='py-6 pr-6 w-full'>
                {filteredSupplierList.length ? (
                  <CardList
                    bodyList={filteredSupplierList.map(
                      ({ supplierName, supplierID }) => ({
                        id: supplierID,
                        name: supplierName,
                        actions: [
                          {
                            name: 'Edit',
                            href: `/suppliers/${supplierID}/edit`,
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
                    href='/suppliers/create'
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
