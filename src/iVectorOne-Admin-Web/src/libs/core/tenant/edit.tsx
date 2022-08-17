import { memo, useEffect, useState, FC, useCallback, useMemo } from 'react';
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
import {
  TextField,
  Notification,
  Button,
  Toggle,
  ConfirmModal,
} from '@/components';
import {
  deleteTenant,
  getTenantById,
  updateTenant,
  updateTenantStatus,
} from '../data-access/tenant';

type NotificationState = {
  status: NotificationStatus;
  message: string;
};

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

export const TenantEdit: FC<Props> = memo(() => {
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const { slug } = useSlug();

  const isLoading = useSelector((state: RootState) => state.app.isLoading);
  const user = useSelector((state: RootState) => state.app.user);
  const appError = useSelector((state: RootState) => state.app.error);

  const [notification, setNotification] = useState<NotificationState>({
    status: NotificationStatus.SUCCESS,
    message: MESSAGES.onSuccess.update,
  });
  const [showNotification, setShowNotification] = useState<boolean>(false);
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
      Number(slug),
      () => {
        dispatch.app.setIsLoading(true);
      },
      (t) => {
        dispatch.app.setIsLoading(false);
        setTenant(t);
      },
      (err) => {
        console.error(err);
        dispatch.app.setIsLoading(false);
        setNotification({
          status: NotificationStatus.ERROR,
          message: 'Tenant not found',
        });
        navigate('/tenants');
      }
    );
  }, [activeUserTenant]);

  const onSubmit: SubmitHandler<Tenant> = async (data) => {
    if (!userIsValid) return;
    await updateTenant(
      activeUserTenant?.tenantKey as string,
      Number(slug),
      data,
      () => {
        dispatch.app.setIsLoading(true);
      },
      () => {
        dispatch.app.setIsLoading(false);
        setNotification({
          status: NotificationStatus.SUCCESS,
          message: MESSAGES.onSuccess.update,
        });
        setShowNotification(true);
        setTimeout(() => {
          navigate('/tenants');
        }, 500);
      },
      (err) => {
        console.error(err);
        dispatch.app.setIsLoading(false);
        setNotification({
          status: NotificationStatus.ERROR,
          message: MESSAGES.onFailed.update,
        });
        setShowNotification(true);
      }
    );
  };

  const handleToggleTenantStatus = async () => {
    if (!userIsValid) return;
    await updateTenantStatus(
      activeUserTenant?.tenantKey as string,
      Number(slug),
      !tenant?.isActive,
      () => {
        dispatch.app.setIsLoading(true);
      },
      () => {
        dispatch.app.setIsLoading(false);
        setTenant({
          ...(tenant as Tenant),
          isActive: !tenant?.isActive,
        });
        setNotification({
          status: NotificationStatus.SUCCESS,
          message: MESSAGES.onSuccess.status,
        });
        setShowNotification(true);
      },
      (err) => {
        console.error(err);
        dispatch.app.setIsLoading(false);
        setNotification({
          status: NotificationStatus.ERROR,
          message: MESSAGES.onFailed.status,
        });
        setShowNotification(true);
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
      Number(slug),
      () => {
        dispatch.app.setIsLoading(true);
      },
      () => {
        dispatch.app.setIsLoading(false);
        setNotification({
          status: NotificationStatus.SUCCESS,
          message: tenant?.isDeleted
            ? MESSAGES.onSuccess.delete[1]
            : MESSAGES.onSuccess.delete[0],
        });
        setShowNotification(true);
        setTenant({
          ...(tenant as Tenant),
          isDeleted: true,
        });
        setIsDeleting(false);
        setTimeout(() => {
          navigate('/tenants');
        }, 500);
      },
      () => {
        dispatch.app.setIsLoading(false);
        setNotification({
          status: NotificationStatus.ERROR,
          message: tenant?.isDeleted
            ? MESSAGES.onFailed.delete[1]
            : MESSAGES.onFailed.delete[0],
        });
        setShowNotification(true);
      }
    );
  };

  useEffect(() => {
    if (!!appError) {
      setNotification({
        status: NotificationStatus.ERROR,
        message: appError as string,
      });
      setShowNotification(true);
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
                <Toggle
                  id='isActive'
                  name='isActive'
                  labelText='Status'
                  defaultValue={tenant?.isActive}
                  onChange={handleToggleTenantStatus}
                  onBlur={handleToggleTenantStatus}
                  readOnly
                />
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

      {showNotification && (
        <Notification
          description={notification.message}
          show={showNotification}
          setShow={setShowNotification}
          status={notification.status}
        />
      )}

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
});
