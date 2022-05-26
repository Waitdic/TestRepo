import { memo, useEffect, useState, FC, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { useForm, SubmitHandler } from 'react-hook-form';
import axios from 'axios';
//
import { Tenant } from '@/types';
import { useSlug } from '@/utils/use-slug';
import { NotificationStatus, ButtonColors, ButtonVariants } from '@/constants';
//
import MainLayout from '@/layouts/Main';
import {
  ErrorBoundary,
  TextField,
  Spinner,
  Notification,
  Button,
} from '@/components';

type NotificationState = {
  status: NotificationStatus;
  message: string;
};

type Props = {
  fetchedTenantList: {
    tenantList: Tenant[];
    error: string | null;
    isLoading: boolean;
  };
};

export const TenantEdit: FC<Props> = memo(({ fetchedTenantList }) => {
  const navigate = useNavigate();
  const { slug } = useSlug();

  const [notification, setNotification] = useState<NotificationState>({
    status: NotificationStatus.SUCCESS,
    message: 'Tenant edited successfully.',
  });
  const [showNotification, setShowNotification] = useState<boolean>(false);

  const {
    register,
    handleSubmit,
    setValue,
    formState: { errors },
  } = useForm<Tenant>();

  const onSubmit: SubmitHandler<Tenant> = async (data) => {
    console.log('Form is valid and submitted.', data);

    try {
      const updatedTenant = await axios.patch(
        'http://localhost:3001/tenant.edit/100',
        data
      );

      console.log(updatedTenant);
      setNotification({
        status: NotificationStatus.SUCCESS,
        message: 'Tenant edited successfully.',
      });
      setShowNotification(true);
    } catch (error) {
      if (typeof error === 'string') {
        console.log(error.toUpperCase());
        setNotification({
          status: NotificationStatus.ERROR,
          message: error.toUpperCase(),
        });
      } else if (error instanceof Error) {
        console.log(error.message);
        setNotification({
          status: NotificationStatus.ERROR,
          message: error.message,
        });
      }
      setShowNotification(true);
    }
  };

  const loadTenant = useCallback(() => {
    if (fetchedTenantList.isLoading) return;

    if (fetchedTenantList?.tenantList.length > 0) {
      const currentTenant = fetchedTenantList?.tenantList.filter(
        (tenant) => tenant.tenantId === slug
      )[0];

      if (!currentTenant) {
        navigate('/tenant/list');
      } else {
        setValue('name', currentTenant.name);
      }
    }
  }, [fetchedTenantList, navigate, setValue, slug]);

  useEffect(() => {
    loadTenant();

    if (fetchedTenantList.error) {
      setNotification({
        status: NotificationStatus.ERROR,
        message: fetchedTenantList.error,
      });
      setShowNotification(true);
    }
  }, [fetchedTenantList, navigate, setValue, slug, loadTenant]);

  return (
    <>
      <MainLayout>
        <div className='flex flex-col'>
          {/* Edit Tenant */}
          {fetchedTenantList.error ? (
            <ErrorBoundary />
          ) : (
            <>
              <h2 className='md:text-3xl text-2xl font-semibold sm:font-medium text-gray-900 mb-5 pb-3 md:mb-8 md:pb-6'>
                Edit Tenant
              </h2>
              <form
                className='w-full divide-y divide-gray-200'
                onSubmit={handleSubmit(onSubmit)}
                autoComplete='turnedOff'
              >
                <div className='mb-8 md:w-3/4'>
                  {!fetchedTenantList.isLoading &&
                  fetchedTenantList.tenantList.length > 0 ? (
                    <TextField
                      id='newTenant'
                      {...register('name', {
                        required: 'This field is required.',
                      })}
                      labelText='Name'
                      isDirty={errors.name ? true : false}
                      errorMsg={errors.name?.message}
                    />
                  ) : (
                    <Spinner />
                  )}
                </div>
                <div className='flex justify-end mt-5 pt-5'>
                  <Button
                    text='Cancel'
                    color={ButtonColors.OUTLINE}
                    className='ml-4'
                    onClick={() => navigate(-1)}
                  />
                  <Button
                    type={ButtonVariants.SUBMIT}
                    text='Save'
                    className='ml-4'
                  />
                </div>
              </form>
            </>
          )}
        </div>
      </MainLayout>

      {showNotification && (
        <Notification
          title={
            notification.status === NotificationStatus.ERROR
              ? 'Error'
              : 'Edit Tenant'
          }
          description={notification.message}
          show={showNotification}
          setShow={setShowNotification}
          status={notification.status}
          autoHide={false}
        />
      )}
    </>
  );
});
