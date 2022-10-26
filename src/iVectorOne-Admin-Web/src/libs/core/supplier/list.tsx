import React, { useState, useEffect, useMemo, useCallback } from 'react';
import { capitalize, sortBy } from 'lodash';
import classNames from 'classnames';
import { useDispatch, useSelector } from 'react-redux';
//
import { RootState } from '@/store';
import { Supplier, Account } from '@/types';
import MainLayout from '@/layouts/Main';
import { EmptyState, CardList, Modal, Button } from '@/components';
import { getAccounts } from '../data-access/account';
import { getSuppliersByAccount, testSupplier } from '../data-access/supplier';
import { ButtonColors, NotificationStatus } from '@/constants';

type Props = {};

const tableEmptyState = {
  title: 'No Suppliers',
  description: ['It appears you have not configured any suppliers yet.'],
  href: '/suppliers/create',
  buttonText: 'New Supplier',
};

const SupplierList: React.FC<Props> = () => {
  const dispatch = useDispatch();

  const user = useSelector((state: RootState) => state.app.user);
  const accounts = useSelector((state: RootState) => state.app.accounts);
  const userKey = useSelector(
    (state: RootState) => state.app.awsAmplify.username
  );
  const isLoading = useSelector((state: RootState) => state.app.isLoading);

  const [filteredSuppliersList, setFilteredSuppliersList] = useState<
    Supplier[] | null
  >(null);
  const [activeAcc, setActiveAcc] = useState<Account | null>(null);
  const [testDetails, setTestDetails] = useState({
    name: '',
    isTesting: false,
    status: '',
  });

  const activeTenant = useMemo(
    () => user?.tenants?.find((tenant) => tenant.isSelected),
    [user]
  );

  const fetchAccounts = useCallback(async () => {
    if (!activeTenant) return;
    await getAccounts(
      { id: activeTenant.tenantId, key: activeTenant.tenantKey },
      userKey as string,
      () => {
        dispatch.app.setIsLoading(true);
      },
      (fetchedAccounts) => {
        dispatch.app.updateAccounts(fetchedAccounts);
        dispatch.app.setIsLoading(false);
      },
      (err) => {
        dispatch.app.setError(err);
        dispatch.app.setIsLoading(false);
      }
    );
  }, [activeTenant]);

  const handleSetActiveAcc = useCallback(
    async (accId: number) => {
      if (!accounts?.length || !activeTenant) return;
      const selectedAcc = accounts.find((acc) => acc.accountId === accId);
      if (!selectedAcc) return;
      setActiveAcc(selectedAcc);
      await getSuppliersByAccount(
        { id: activeTenant.tenantId, key: activeTenant.tenantKey },
        userKey as string,
        selectedAcc.accountId,
        () => {
          dispatch.app.setIsLoading(true);
        },
        (fetchedSuppliers) => {
          dispatch.app.updateAccounts(
            accounts.map((acc) => {
              if (acc.accountId === selectedAcc.accountId) {
                return { ...acc, suppliers: fetchedSuppliers };
              }
              return acc;
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
    },
    [accounts, activeTenant]
  );

  const handleTesting = useCallback(
    async (name: string, supplierId: number, accountId: number) => {
      if (!activeTenant || !userKey) return;
      setTestDetails({ name, isTesting: true, status: 'Running test...' });
      await testSupplier(
        activeTenant?.tenantKey,
        userKey,
        activeTenant.tenantId,
        accountId,
        supplierId,
        (status) => {
          setTestDetails({
            name,
            isTesting: true,
            status: status,
          });
        },
        (err, instance) => {
          setTestDetails({
            name,
            isTesting: true,
            status: err,
          });
          dispatch.app.setNotification({
            status: NotificationStatus.ERROR,
            message: err,
            instance,
          });
        }
      );
    },
    []
  );

  useEffect(() => {
    fetchAccounts();
  }, [fetchAccounts]);

  return (
    <>
      <MainLayout title='Suppliers' addNew addNewHref='/suppliers/create'>
        <div className='bg-white shadow-lg rounded-sm mb-8'>
          <div className='flex flex-col md:flex-row md:-mr-px gap-6'>
            <div className='flex flex-nowrap overflow-x-scroll no-scrollbar md:block md:overflow-auto px-3 py-6 border-b md:border-b-0 md:border-r border-slate-200 min-w-60 md:space-y-3'>
              <div>
                <div className='text-xs font-semibold text-slate-400 uppercase mb-3'>
                  Accounts
                </div>
                <ul className='flex flex-nowrap md:block mr-3 md:mr-0'>
                  {sortBy(accounts, [
                    function (o) {
                      return o.userName?.toLowerCase?.();
                    },
                  ])?.map(({ accountId, userName }) => (
                    <li
                      key={accountId}
                      className={classNames(
                        'mr-0.5 md:mr-0 md:mb-0.5 flex items-center px-2.5 py-2 rounded whitespace-nowrap cursor-pointer',
                        {
                          'bg-indigo-50': activeAcc?.accountId === accountId,
                        }
                      )}
                      onClick={() => handleSetActiveAcc(accountId)}
                    >
                      <span
                        className={`text-sm font-medium ${
                          activeAcc?.accountId === accountId
                            ? 'text-indigo-500'
                            : 'hover:text-dark'
                        }`}
                      >
                        {userName}
                      </span>
                    </li>
                  ))}
                </ul>
              </div>
            </div>
            <div className='pl-6 md:pl-0 py-6 pr-6 w-full'>
              {!!filteredSuppliersList?.length ? (
                <CardList
                  bodyList={sortBy(
                    filteredSuppliersList.map(
                      ({ name, supplierID, enabled }) => ({
                        id: supplierID as number,
                        isActive: enabled,
                        name,
                        actions: [
                          {
                            name: 'Edit',
                            href: `/suppliers/${supplierID}/edit?accountId=${activeAcc?.accountId}`,
                          },
                          {
                            name: 'Test',
                            onClick: () =>
                              handleTesting(
                                name as string,
                                supplierID as number,
                                activeAcc?.accountId as number
                              ),
                          },
                        ],
                      })
                    ),
                    'name'
                  )}
                  isLoading={isLoading}
                  emptyState={tableEmptyState}
                />
              ) : (
                <EmptyState {...tableEmptyState} />
              )}
            </div>
          </div>
        </div>
      </MainLayout>

      {testDetails.isTesting && (
        <Modal transparent flex>
          <div className='bg-white rounded shadow-lg overflow-auto max-w-lg w-full max-h-full'>
            <div className='px-5 py-3 border-b border-slate-200'>
              <div className='flex justify-between items-center'>
                <div className='font-semibold text-slate-800'>
                  Test Supplier - {capitalize(testDetails.name)}
                </div>
              </div>
            </div>
            <p className='p-5'>{testDetails.status}</p>
            <div className='flex justify-end px-5 pb-5'>
              <Button
                text='Close'
                onClick={() => {
                  setTestDetails({
                    name: '',
                    isTesting: false,
                    status: '',
                  });
                }}
                color={ButtonColors.PRIMARY}
              />
            </div>
          </div>
        </Modal>
      )}
    </>
  );
};

export default React.memo(SupplierList);
