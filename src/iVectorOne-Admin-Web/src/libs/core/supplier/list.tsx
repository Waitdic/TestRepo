import { memo, useState, useEffect, FC, useMemo, useCallback } from 'react';
import { sortBy } from 'lodash';
import classNames from 'classnames';
import { useDispatch, useSelector } from 'react-redux';
//
import { Supplier, Subscription } from '@/types';
import MainLayout from '@/layouts/Main';
import { EmptyState, CardList, Notification, Button } from '@/components';
import { RootState } from '@/store';
import { getSubscriptions, getSuppliersBySubscription } from '../data-access';
import { NotificationStatus } from '@/constants';

type Props = {};

const tableEmptyState = {
  title: 'No Suppliers',
  description: ['It appears you have not configured any suppliers yet.'],
  href: '/suppliers/create',
  buttonText: 'New Supplier',
};

export const SupplierList: FC<Props> = memo(() => {
  const dispatch = useDispatch();

  const error = useSelector((state: RootState) => state.app.error);
  const user = useSelector((state: RootState) => state.app.user);
  const subscriptions = useSelector(
    (state: RootState) => state.app.subscriptions
  );

  const [filteredSuppliersList, setFilteredSuppliersList] = useState<
    Supplier[] | null
  >(null);
  const [activeSub, setActiveSub] = useState<Subscription | null>(null);

  const activeTenant = useMemo(
    () => user?.tenants?.find((tenant) => tenant.isSelected),
    [user]
  );

  const fetchData = useCallback(async () => {
    if (!activeTenant) return;
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

  const handleSetActiveSub = async (subId: number) => {
    if (!subscriptions?.length || !activeTenant) return;
    const selectedSub = subscriptions.find(
      (sub) => sub.subscriptionId === subId
    );
    if (!selectedSub) return;
    setActiveSub(selectedSub);
    await getSuppliersBySubscription(
      { id: activeTenant.tenantId, key: activeTenant.tenantKey },
      selectedSub.subscriptionId,
      () => {
        dispatch.app.setIsLoading(true);
      },
      (fetchedSuppliers) => {
        dispatch.app.updateSubscriptions(
          subscriptions.map((sub) => {
            if (sub.subscriptionId === selectedSub.subscriptionId) {
              return { ...sub, suppliers: fetchedSuppliers };
            }
            return sub;
          })
        );
        dispatch.app.setIsLoading(false);
        setFilteredSuppliersList(sortBy(fetchedSuppliers, 'name'));
      },
      (err) => {
        dispatch.app.setError(err);
        dispatch.app.setIsLoading(false);
      }
    );

    const mainLayoutArea = document.getElementById('main-layout-area');
    if (!!mainLayoutArea?.scrollTop) {
      mainLayoutArea.scrollTop = 0;
    }
  };

  useEffect(() => {
    if (!!user) {
      fetchData();
    }
  }, [fetchData, user]);

  return (
    <>
      <MainLayout>
        <>
          {/* Page header */}
          <div className='flex align-start justify-between mb-6'>
            {/* Title */}
            <h1 className='text-2xl md:text-3xl text-slate-800 font-bold'>
              Suppliers
            </h1>
            <div className='flex gap-3'>
              <Button text='New' isLink href='/suppliers/create' />
            </div>
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
                    {sortBy(subscriptions, 'userName')?.map(
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
                {!!filteredSuppliersList?.length ? (
                  <CardList
                    bodyList={sortBy(
                      filteredSuppliersList.map(({ name, supplierID }) => ({
                        id: supplierID,
                        name,
                        actions: [
                          {
                            name: 'Edit',
                            href: `/suppliers/${supplierID}/edit?subscriptionId=${activeSub?.subscriptionId}`,
                          },
                        ],
                      })),
                      'name'
                    )}
                    emptyState={tableEmptyState}
                    statusIsPlaceholder
                  />
                ) : (
                  filteredSuppliersList !== null && (
                    <EmptyState {...tableEmptyState} />
                  )
                )}
              </div>
            </div>
          </div>
        </>
      </MainLayout>
      {!!error && (
        <Notification
          title='Data fetching error'
          description={error as string}
          show={!!error}
          status={NotificationStatus.ERROR}
        />
      )}
    </>
  );
});
