import React, { useState, useMemo, useCallback, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useDispatch, useSelector } from 'react-redux';
import { sortBy } from 'lodash';
//
import { RootState } from '@/store';
import MainLayout from '@/layouts/Main';
import type { SelectOption, Tenant, User } from '@/types';
import { NotificationStatus, ButtonColors } from '@/constants';
import {
  Button,
  ConfirmModal,
  RoleGuard,
  Select,
  TableList,
} from '@/components';
import {
  getUserInfo,
  linkUserTenant,
  unlinkUserTenant,
} from '../data-access/user';
import { useSlug } from '@/utils/use-slug';
import { getTenants } from '../data-access/tenant';

type Props = {};

const headerList = [
  { name: 'Tenants', align: 'left' },
  { name: 'Actions', align: 'right' },
];

const MESSAGES = {
  onSuccess: {
    unlinkTenant: 'Tenant unlinked successfully',
    linkTenant: 'Tenant linked successfully',
  },
  onFailed: {
    unlinkTenant: 'Failed to unlink tenant',
    linkTenant: 'Failed to link tenant',
    userFetch: 'Failed to fetch user',
    tenantsFetch: 'Failed to fetch tenants',
  },
};

const UserView: React.FC<Props> = () => {
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const { slug } = useSlug();

  const user = useSelector((state: RootState) => state.app.user);
  const userKey = useSelector(
    (state: RootState) => state.app.awsAmplify.username
  );
  const isLoading = useSelector((state: RootState) => state.app.isLoading);

  const [userTenants, setUserTenants] = useState<Tenant[]>([]);
  const [tenantsOptions, setTenantsOptions] = useState<SelectOption[]>([]);
  const [tenants, setTenants] = useState<Tenant[]>([]);
  const [currentUser, setCurrentUser] = useState<User>();
  const [isUnlinking, setIsUnlinking] = useState(false);
  const [draftTenant, setDraftTenant] = useState<{
    userId: number;
    tenantId: number;
    name: string;
  }>({
    userId: -1,
    tenantId: -1,
    name: '',
  });

  const activeTenant = useMemo(() => {
    return user?.tenants.find((tenant) => tenant.isSelected);
  }, [user]);
  const isValidUser = useMemo(
    () => !!userKey && !!activeTenant,
    [userKey, activeTenant]
  );
  const tableBodyList = useMemo(
    () =>
      sortBy(
        userTenants?.map(({ tenantId, tenantKey, name, isActive }) => ({
          id: `${tenantId}+${tenantKey}`,
          name,
          isActive,
          actions: [
            {
              name: 'Unlink',
              onClick: () =>
                attemptUnlinkTenant(
                  tenantId,
                  currentUser?.userId as number,
                  name
                ),
            },
          ],
        })),
        'name'
      ),
    [userTenants, currentUser]
  );
  const selectableTenantsOptions = useMemo(() => {
    return tenantsOptions?.filter(
      (option) => !userTenants?.find(({ tenantId }) => tenantId === option.id)
    );
  }, [tenantsOptions, userTenants]);

  const attemptUnlinkTenant = (
    tenantId: number,
    userId: number,
    name: string
  ) => {
    setIsUnlinking(true);
    setDraftTenant({ tenantId, userId, name });
  };

  const handleUnlinkUserTenant = useCallback(
    async (tenantId: number, userId: number) => {
      if (!isValidUser || isLoading) return;
      await unlinkUserTenant(
        activeTenant?.tenantKey as string,
        userKey as string,
        tenantId,
        userId,
        () => {
          dispatch.app.setIsLoading(true);
        },
        () => {
          dispatch.app.setIsLoading(false);
          dispatch.app.setNotification({
            status: NotificationStatus.SUCCESS,
            message: MESSAGES.onSuccess.unlinkTenant,
          });

          fetchUser();
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
    },
    [isValidUser, isLoading]
  );

  const handleRemoveStoredLastUsedTenant = (tenantId: number) => {
    const storedTenants: Tenant[] = JSON.parse(
      localStorage.getItem('lastUsedTenants') || '[]'
    );
    const newStoredTenants = storedTenants.filter(
      (t) => t.tenantId !== tenantId
    );
    localStorage.setItem('lastUsedTenants', JSON.stringify(newStoredTenants));
  };

  const handleTenantUnlink = () => {
    setIsUnlinking(false);
    handleUnlinkUserTenant(draftTenant.tenantId, draftTenant.userId);
    handleRemoveStoredLastUsedTenant(draftTenant.tenantId);
  };

  const handleTenantSelect = useCallback(
    (tenantId: number) => {
      const selectedTenant = tenants.find((t) => tenantId === t.tenantId);
      if (selectedTenant) {
        setDraftTenant({
          userId: currentUser?.userId as number,
          tenantId: selectedTenant.tenantId,
          name: selectedTenant.companyName,
        });
      }
    },
    [tenants, currentUser]
  );

  const handleTenantLink = useCallback(async () => {
    if (!isValidUser || isLoading) return;
    await linkUserTenant(
      activeTenant?.tenantKey as string,
      userKey as string,
      draftTenant?.tenantId,
      draftTenant?.userId,
      () => {
        dispatch.app.setIsLoading(true);
      },
      () => {
        dispatch.app.setIsLoading(false);
        dispatch.app.setNotification({
          status: NotificationStatus.SUCCESS,
          message: MESSAGES.onSuccess.linkTenant,
        });

        fetchUser();
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
  }, [isValidUser, isLoading, draftTenant]);

  const fetchUser = useCallback(async () => {
    if (!isValidUser) return;
    await getUserInfo(
      userKey as string,
      userKey as string,
      slug as string,
      () => {
        dispatch.app.setIsLoading(true);
      },
      (fetchedUser) => {
        dispatch.app.setIsLoading(false);
        setCurrentUser(fetchedUser);
        setUserTenants(fetchedUser?.tenants || []);
      },
      (err, instance) => {
        dispatch.app.setIsLoading(false);
        dispatch.app.setNotification({
          status: NotificationStatus.ERROR,
          message: err,
          instance,
        });

        navigate('/users', { replace: true });
      }
    );
  }, [isValidUser]);

  const fetchTenants = useCallback(async () => {
    if (!isValidUser) return;
    await getTenants(
      userKey as string,
      activeTenant?.tenantKey as string,
      () => {
        dispatch.app.setIsLoading(true);
      },
      (fetchedTenants) => {
        dispatch.app.setIsLoading(false);
        const sortedTenants = sortBy(fetchedTenants, 'companyName');
        setTenants(fetchedTenants);
        setTenantsOptions(
          sortedTenants.map((tenant) => ({
            id: tenant.tenantId,
            name: tenant.companyName,
          }))
        );
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
  }, [isValidUser]);

  useEffect(() => {
    fetchUser();
    fetchTenants();
  }, [fetchUser, fetchTenants]);

  return (
    <>
      <RoleGuard withRedirect>
        <MainLayout title={`${currentUser?.fullName || ''}`}>
          <div className='bg-white shadow-lg rounded-sm mb-8'>
            <div className='flex flex-col md:flex-row md:-mr-px'>
              <div className='min-w-60'></div>
              <div className='w-full divide-y divide-gray-200 p-6'>
                <div className='mb-8 flex flex-col gap-5 md:w-1/2'>
                  <div>
                    {tenantsOptions.length > 0 && (
                      <div className='mb-5'>
                        <h2 className='text-2xl font-bold mb-3 text-dark'>
                          Link Tenant
                        </h2>
                        <div className='flex items-end w-full'>
                          <div className='flex-1'>
                            <Select
                              id='tenant-select'
                              name='tenant-select'
                              options={selectableTenantsOptions}
                              labelText='Select a tenant'
                              onUncontrolledChange={handleTenantSelect}
                              isFirstOptionEmpty
                              defaultValue={{
                                id: -1,
                                name: 'Select a tenant',
                              }}
                            />
                          </div>
                          <Button
                            text='Link'
                            color={ButtonColors.PRIMARY}
                            className='ml-4'
                            onClick={handleTenantLink}
                          />
                        </div>
                      </div>
                    )}
                    <h2 className='text-2xl font-bold mb-3 text-dark'>
                      Tenants
                    </h2>
                    {currentUser && (
                      <TableList
                        headerList={headerList}
                        bodyList={tableBodyList}
                        emptyState={{
                          title: 'No tenants found',
                          description: [
                            'You have not linked any tenants to this user',
                          ],
                        }}
                        isLoading={isLoading}
                      />
                    )}
                  </div>
                </div>
                <div className='flex justify-end mt-5 pt-5'>
                  <Button
                    text='Close'
                    color={ButtonColors.OUTLINE}
                    className='ml-4'
                    onClick={() => navigate('/users')}
                  />
                </div>
              </div>
            </div>
          </div>
        </MainLayout>
      </RoleGuard>

      {isUnlinking && (
        <ConfirmModal
          title='Tenant Unlink'
          description={
            <>
              Are you sure you want to unlink{' '}
              <strong>{draftTenant.name}</strong>?
            </>
          }
          show={isUnlinking}
          setShow={setIsUnlinking}
          onConfirm={handleTenantUnlink}
          confirmButtonText='Unlink'
        />
      )}
    </>
  );
};

export default React.memo(UserView);
