import React, { useState, useEffect, useMemo, useCallback } from 'react';
import { useDispatch, useSelector } from 'react-redux';
//
import { RootState } from '@/store';
import type { Tenant } from '@/types';
import MainLayout from '@/layouts/Main';
import { NotificationStatus } from '@/constants';
import { CardList, RoleGuard } from '@/components';
import { getTenants, updateTenantStatus } from '../data-access/tenant';

type Props = {};

const tableEmptyState = {
  title: 'No Tenants',
  description: ['Get started by creating a new tenant.'],
  href: '/tenants/create',
  buttonText: 'New Tenant',
};

const MESSAGES = {
  onSuccess: {
    status: 'Tenant status updated successfully',
    fetch: 'Fetching tenants',
  },
  onFailed: {
    status: 'Failed to update tenant status',
    fetch: 'Failed to fetch tenants',
  },
};

const TenantList: React.FC<Props> = () => {
  const dispatch = useDispatch();

  const isLoading = useSelector((state: RootState) => state.app.isLoading);
  const user = useSelector((state: RootState) => state.app.user);
  const userKey = useSelector(
    (state: RootState) => state.app.awsAmplify.username
  );

  const [tenants, setTenants] = useState<Tenant[]>([]);

  const activeTenant = useMemo(() => {
    return user?.tenants.find((tenant) => tenant.isSelected);
  }, [user]);
  const userIsValid = useMemo(
    () => !!activeTenant && !!tenants && !isLoading,
    [activeTenant, tenants, isLoading]
  );

  const handleToggleTenantStatus = useCallback(
    async (tenantId: number, isActive: boolean) => {
      if (!userIsValid) return;
      await updateTenantStatus(
        activeTenant?.tenantKey as string,
        userKey as string,
        tenantId,
        !isActive,
        () => {
          dispatch.app.setIsLoading(true);
        },
        () => {
          dispatch.app.setIsLoading(false);
          setTenants(
            tenants.map((tenant) => {
              if (tenant.tenantId === tenantId) {
                tenant.isActive = !isActive;
              }
              return tenant;
            })
          );
          dispatch.app.setNotification({
            status: NotificationStatus.SUCCESS,
            message: MESSAGES.onSuccess.status,
          });
        },
        (err, instance) => {
          console.error(err);
          dispatch.app.setIsLoading(false);
          dispatch.app.setNotification({
            status: NotificationStatus.ERROR,
            message: err,
            instance,
          });
        }
      );
    },
    [activeTenant, tenants, isLoading, dispatch]
  );

  const tableBodyList = useMemo(
    () =>
      tenants?.map(({ tenantId, companyName, isActive }) => ({
        id: tenantId,
        name: companyName,
        isActive,
        actions: [
          { name: 'Edit', href: `/tenants/${tenantId}/edit` },
          {
            name: isActive ? 'Disable' : 'Enable',
            onToggle: handleToggleTenantStatus,
          },
        ],
      })),
    [tenants]
  );

  const fetchTenants = useCallback(async () => {
    if (!activeTenant) return;
    await getTenants(
      userKey as string,
      activeTenant?.tenantKey,
      () => {
        dispatch.app.setIsLoading(true);
      },
      (_tenants) => {
        dispatch.app.setIsLoading(false);
        setTenants(_tenants);
      },
      (err, instance) => {
        dispatch.app.setIsLoading(false);
        dispatch.app.setNotification({
          status: NotificationStatus.ERROR,
          message: err,
          instance,
        });
      }
    );
  }, [activeTenant]);

  useEffect(() => {
    if (!!activeTenant) {
      fetchTenants();
    }
  }, [activeTenant, fetchTenants]);

  return (
    <>
      <RoleGuard withRedirect>
        <MainLayout title='Tenants' addNew addNewHref='/tenants/create'>
          <div className='flex flex-col h-full'>
            <CardList
              bodyList={tableBodyList}
              isLoading={isLoading}
              emptyState={tableEmptyState}
            />
          </div>
        </MainLayout>
      </RoleGuard>
    </>
  );
};

export default React.memo(TenantList);
