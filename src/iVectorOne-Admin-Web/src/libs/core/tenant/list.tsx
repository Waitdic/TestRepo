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
import { useSelector } from 'react-redux';
import { RootState } from '@/store';

type Props = {
  error: string | null;
};

export const TenantList: FC<Props> = memo(({ error }) => {
  const tenants = useSelector((state: RootState) => state.app.user?.tenants);
  const isLoading = useSelector((state: RootState) => state.app.isLoading);

  const [filteredTenantList, setFilteredTenantList] = useState<Tenant[]>([]);
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
    if (error) {
      setShowError(true);
    } else {
      setShowError(false);
    }
    setFilteredTenantList(tenants || []);
  }, [tenants]);

  return (
    <>
      <MainLayout
      // title='Tenant List'
      >
        <div className='flex flex-col h-full'>
          {/* Tenants */}

          {error ? (
            <ErrorBoundary />
          ) : (
            <>
              <div className='flex align-start justify-end mb-6'>
                <div className='flex'>
                  <SearchField
                    list={tenants || []}
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
                isLoading={isLoading}
                emptyState={tableEmptyState}
              />
            </>
          )}
        </div>
      </MainLayout>

      {showError && (
        <Notification
          title='Error'
          description={error as string}
          show={showError}
          setShow={setShowError}
          status={NotificationStatus.ERROR}
          autoHide={false}
        />
      )}
    </>
  );
});
