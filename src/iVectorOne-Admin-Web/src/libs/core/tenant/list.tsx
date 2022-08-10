import { memo, useState, useEffect, FC, useMemo, useCallback } from 'react';
import { useDispatch, useSelector } from 'react-redux';
//
import { RootState } from '@/store';
import type { Tenant } from '@/types';
import MainLayout from '@/layouts/Main';
import { NotificationStatus } from '@/constants';
import { ErrorBoundary, Notification, CardList } from '@/components';
import { getTenants } from '../data-access/tenant';

type Props = {
  error: string | null;
};

const tableEmptyState = {
  title: 'No Tenants',
  description: ['Get started by creating a new tenant.'],
  href: '/tenants/create',
  buttonText: 'New Tenant',
};

export const TenantList: FC<Props> = memo(({ error }) => {
  const dispatch = useDispatch();

  const isLoading = useSelector((state: RootState) => state.app.isLoading);
  const user = useSelector((state: RootState) => state.app.user);
  const appError = useSelector((state: RootState) => state.app.error);

  const [tenants, setTenants] = useState<Tenant[]>([]);
  const [showError, setShowError] = useState<boolean>(false);

  const activeTenant = useMemo(() => {
    return user?.tenants.find((tenant) => tenant.isSelected);
  }, [user]);
  const tableBodyList = useMemo(
    () =>
      tenants?.map(({ tenantId, companyName }) => ({
        id: tenantId,
        name: companyName,
        actions: [{ name: 'Edit', href: `/tenants/${tenantId}/edit` }],
      })),
    [tenants]
  );

  const fetchTenants = useCallback(async () => {
    if (!activeTenant) return;
    await getTenants(
      { id: activeTenant.tenantId, key: activeTenant.tenantKey },
      () => {
        dispatch.app.setIsLoading(true);
      },
      (_tenants) => {
        dispatch.app.setIsLoading(false);
        setTenants(_tenants);
      },
      (err) => {
        dispatch.app.setIsLoading(false);
        dispatch.app.setError(err);
      }
    );
  }, [activeTenant]);

  useEffect(() => {
    if (error) {
      setShowError(true);
    } else {
      setShowError(false);
    }
  }, [error]);

  useEffect(() => {
    if (!!activeTenant) {
      fetchTenants();
    }
  }, [activeTenant, fetchTenants]);

  return (
    <>
      <MainLayout title='Tenants' addNew addNewHref='/tenants/create'>
        <div className='flex flex-col h-full'>
          {/* Tenants */}
          {error ? (
            <ErrorBoundary />
          ) : (
            <>
              <CardList
                bodyList={tableBodyList}
                isLoading={isLoading}
                emptyState={tableEmptyState}
                statusIsPlaceholder
              />
            </>
          )}
        </div>
      </MainLayout>

      {showError ||
        (!!appError && (
          <Notification
            title='Error'
            description={(error as string) || (appError as string)}
            show={showError || !!appError}
            setShow={setShowError}
            status={NotificationStatus.ERROR}
            autoHide={false}
          />
        ))}
    </>
  );
});
