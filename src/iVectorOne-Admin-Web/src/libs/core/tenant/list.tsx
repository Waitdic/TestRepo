import { memo, useState, useEffect, FC } from 'react';
//
import { Tenant } from '@/types';
import { NotificationStatus } from '@/constants';
//
import MainLayout from '@/layouts/Main';
import {
  ErrorBoundary,
  TableList,
  Button,
  SearchField,
  Notification,
} from '@/components';

type Props = {
  fetchedTenantList: {
    tenantList: Tenant[];
    error: string | null;
    isLoading: boolean;
  };
};

export const TenantList: FC<Props> = memo(({ fetchedTenantList }) => {
  const [filteredTenantList, setFilteredTenantList] = useState<Tenant[]>(
    fetchedTenantList.tenantList
  );
  const [showError, setShowError] = useState<boolean>(false);

  const tableHeaderList = [
    { name: 'Name', align: 'left' },
    { name: 'Actions', align: 'right' },
  ];
  const tableBodyList = filteredTenantList.map(({ tenantId, name }) => ({
    id: tenantId,
    name,
    actions: [{ name: 'Edit', href: `/tenant/edit/${tenantId}` }],
  }));
  const tableEmptyState = {
    title: 'No tenants',
    description: 'Get started by creating a new tenant.',
    href: '/tenant/create',
    buttonText: 'New Tenant',
  };

  useEffect(() => {
    if (fetchedTenantList.error) {
      setShowError(true);
    } else {
      setShowError(false);
    }
    setFilteredTenantList(fetchedTenantList.tenantList);
  }, [fetchedTenantList]);

  return (
    <>
      <MainLayout
      // title='Tenant List'
      >
        <div className='flex flex-col h-full'>
          {/* Tenants */}

          {fetchedTenantList.error ? (
            <ErrorBoundary />
          ) : (
            <>
              <div className='flex align-start justify-end mb-6'>
                <div className='flex'>
                  <SearchField
                    list={fetchedTenantList.tenantList}
                    setList={setFilteredTenantList}
                  />
                  <Button
                    text='New'
                    isLink
                    href='/tenant/create'
                    className='ml-3'
                  />
                </div>
              </div>
              <TableList
                headerList={tableHeaderList}
                bodyList={tableBodyList}
                isLoading={fetchedTenantList.isLoading}
                emptyState={tableEmptyState}
              />
            </>
          )}
        </div>
      </MainLayout>

      {showError && (
        <Notification
          title='Error'
          description={fetchedTenantList.error as string}
          show={showError}
          setShow={setShowError}
          status={NotificationStatus.ERROR}
          autoHide={false}
        />
      )}
    </>
  );
});
