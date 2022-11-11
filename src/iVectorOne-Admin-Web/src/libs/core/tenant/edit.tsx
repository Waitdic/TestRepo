import React, { useEffect, useState, useCallback, useMemo } from 'react';
import { useNavigate } from 'react-router-dom';
import { useForm, SubmitHandler } from 'react-hook-form';
import { useDispatch, useSelector } from 'react-redux';
//
import type { Tenant } from '@/types';
import { RootState } from '@/store';
import {
  NotificationStatus,
  ButtonColors,
  ButtonVariants,
  InputTypes,
} from '@/constants';
import { useSlug } from '@/utils/use-slug';
import MainLayout from '@/layouts/Main';
import { TextField, Button, ConfirmModal, RoleGuard } from '@/components';
import {
  deleteTenant,
  getTenantById,
  updateTenant,
} from '../data-access/tenant';

type Props = {};

const MESSAGES = {
  onSuccess: {
    update: 'Tenant updated successfully',
    delete: ['Tenant deleted successfully.', 'Tenant restored successfully.'],
    status: 'Tenant status updated successfully',
  },
  onFailed: {
    update: 'Failed to update tenant',
    delete: ['Failed to delete tenant', 'Failed to restore tenant'],
    status: 'Failed to update tenant status',
  },
};

const TenantEdit: React.FC<Props> = () => {
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const { slug } = useSlug();

  const isLoading = useSelector((state: RootState) => state.app.isLoading);
  const user = useSelector((state: RootState) => state.app.user);
  const userKey = useSelector(
    (state: RootState) => state.app.awsAmplify.username
  );
  const appError = useSelector((state: RootState) => state.app.error);

  const [tenant, setTenant] = useState<Tenant | null>(null);
  const [isDeleting, setIsDeleting] = useState<boolean>(false);

  const activeUserTenant = useMemo(() => {
    return user?.tenants.find((userTenant) => userTenant.isSelected);
  }, [user]);
  const userIsValid = useMemo(
    () => !!activeUserTenant && !!tenant && !isLoading,
    [activeUserTenant, tenant, isLoading]
  );

  const {
    register,
    handleSubmit,
    setValue,
    formState: { errors },
  } = useForm<Tenant>();

  const fetchTenant = useCallback(async () => {
    if (!activeUserTenant) return;
    await getTenantById(
      {
        id: activeUserTenant.tenantId,
        key: activeUserTenant.tenantKey,
      },
      userKey as string,
      Number(slug),
      () => {
        dispatch.app.setIsLoading(true);
      },
      (t) => {
        dispatch.app.setIsLoading(false);
        setTenant(t);
      },
      (err, instance) => {
        console.error(err);
        dispatch.app.setIsLoading(false);
        dispatch.app.setNotification({
          status: NotificationStatus.ERROR,
          message: err,
          instance,
        });
        navigate('/tenants');
      }
    );
  }, [activeUserTenant]);

  const onSubmit: SubmitHandler<Tenant> = async (data) => {
    if (!userIsValid) return;
    await updateTenant(
      activeUserTenant?.tenantKey as string,
      userKey as string,
      Number(slug),
      data,
      () => {
        dispatch.app.setIsLoading(true);
      },
      () => {
        dispatch.app.setIsLoading(false);
        dispatch.app.setNotification({
          status: NotificationStatus.SUCCESS,
          message: MESSAGES.onSuccess.update,
        });

        setTimeout(() => {
          navigate('/tenants');
        }, 500);
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
  };

  const handleAttemptTenantDelete = async () => {
    if (!userIsValid) return;
    setIsDeleting(true);
  };

  const handleDeleteTenant = async () => {
    await deleteTenant(
      activeUserTenant?.tenantKey as string,
      userKey as string,
      Number(slug),
      () => {
        dispatch.app.setIsLoading(true);
      },
      () => {
        dispatch.app.setIsLoading(false);
        dispatch.app.setNotification({
          status: NotificationStatus.SUCCESS,
          message: tenant?.isDeleted
            ? MESSAGES.onSuccess.delete[1]
            : MESSAGES.onSuccess.delete[0],
        });
        setTenant({
          ...(tenant as Tenant),
          isDeleted: true,
        });
        setIsDeleting(false);
        setTimeout(() => {
          navigate('/tenants');
        }, 500);
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
  };

  useEffect(() => {
    if (!!appError) {
      dispatch.app.setNotification({
        status: NotificationStatus.ERROR,
        message: appError as string,
      });
    }
  }, [appError]);

  useEffect(() => {
    if (!!tenant) {
      setValue('contactEmail', tenant?.contactEmail);
      setValue('contactName', tenant?.contactName);
      setValue('contactTelephone', tenant?.contactTelephone);
    }
  }, [tenant]);

  useEffect(() => {
    if (!!activeUserTenant) {
      fetchTenant();
    }
  }, [activeUserTenant, fetchTenant]);

  return (
    <>
      <RoleGuard withRedirect>
        <MainLayout title={`${tenant?.companyName || ''}`}>
          <div className='bg-white shadow-lg rounded-sm mb-8'>
            <div className='flex flex-col md:flex-row md:-mr-px'>
              <div className='min-w-60'></div>
              <form
                className='w-full divide-y divide-gray-200 p-6'
                onSubmit={handleSubmit(onSubmit)}
                autoComplete='turnedOff'
              >
                <div className='mb-8 flex flex-col gap-5 md:w-1/2'>
                  <div>
                    <TextField
                      id='contactEmail'
                      type={InputTypes.EMAIL}
                      {...register('contactEmail', {
                        required: 'This field is required.',
                      })}
                      labelText='Contact Email'
                      isDirty={!!errors.contactEmail}
                      errorMsg={errors.contactEmail?.message}
                      required
                    />
                  </div>
                  <div>
                    <TextField
                      id='contactName'
                      {...register('contactName', {
                        required: 'This field is required.',
                      })}
                      labelText='Contact Name'
                      isDirty={!!errors.contactName}
                      errorMsg={errors.contactName?.message}
                      required
                    />
                  </div>
                  <div>
                    <TextField
                      id='contactTelephone'
                      type={InputTypes.PHONE}
                      {...register('contactTelephone', {
                        required: 'This field is required.',
                      })}
                      labelText='Contact Telephone'
                      isDirty={!!errors.contactTelephone}
                      errorMsg={errors.contactTelephone?.message}
                      required
                    />
                  </div>
                  <div className='flex justify-between items-center'>
                    <label className='text-sm font-medium text-dark'>
                      Status
                    </label>
                    <p className='text-sm font-medium text-dark'>
                      {tenant?.isActive ? 'Active' : 'Inactive'}
                    </p>
                  </div>
                </div>
                <div className='flex justify-end mt-5 pt-5'>
                  <Button
                    text='Cancel'
                    color={ButtonColors.OUTLINE}
                    className='ml-4'
                    onClick={() => navigate(-1)}
                  />
                  {!tenant?.isDeleted && (
                    <Button
                      text='Delete'
                      color={ButtonColors.DANGER}
                      className='ml-4'
                      onClick={handleAttemptTenantDelete}
                    />
                  )}
                  <Button
                    type={ButtonVariants.SUBMIT}
                    text='Save'
                    className='ml-4'
                  />
                </div>
              </form>
            </div>
          </div>
        </MainLayout>
      </RoleGuard>

      {isDeleting && (
        <ConfirmModal
          title='Delete Tenant'
          description={
            <>
              Are you sure you want to delete{' '}
              <strong>{tenant?.companyName}</strong>?
            </>
          }
          show={isDeleting}
          setShow={setIsDeleting}
          onConfirm={handleDeleteTenant}
        />
      )}
    </>
  );
};

export default React.memo(TenantEdit);
