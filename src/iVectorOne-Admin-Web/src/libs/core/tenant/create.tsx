import { memo, useState, FC, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useForm, SubmitHandler } from 'react-hook-form';
import axios from 'axios';
//
import { NotificationState, Tenant } from '@/types';
import { NotificationStatus, ButtonColors, ButtonVariants } from '@/constants';
import MainLayout from '@/layouts/Main';
import { ErrorBoundary, TextField, Button, Notification } from '@/components';

type Props = {
  error: string | null;
};

export const TenantCreate: FC<Props> = memo(({ error }) => {
  const navigate = useNavigate();
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<Tenant>();

  const [notification, setNotification] = useState<NotificationState>({
    status: NotificationStatus.SUCCESS,
    message: 'New Tenant created successfully.',
  });
  const [showNotification, setShowNotification] = useState(false);

  const onSubmit: SubmitHandler<Tenant> = async (data) => {
    console.log('Form is valid and submitted.', data);

    try {
      const newTenant = await axios.post(
        'http://localhost:3001/tenant.create',
        data
      );

      console.log(newTenant);
      setNotification({
        status: NotificationStatus.SUCCESS,
        message: 'New Tenant created successfully.',
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

  useEffect(() => {
    if (typeof error === 'string') {
      setNotification({
        status: NotificationStatus.ERROR,
        message: error,
      });
      setShowNotification(true);
    }
  }, [error]);

  return (
    <>
      <MainLayout>
        <div className='flex flex-col'>
          {/* Create Tenants */}
          <div className='mb-6'>
            {typeof error === 'string' ? (
              <ErrorBoundary />
            ) : (
              <>
                <h2 className='md:text-3xl text-2xl font-semibold sm:font-medium text-gray-900 mb-5 pb-3 md:mb-8 md:pb-6'>
                  New Tenant
                </h2>
                <form
                  className='w-full divide-y divide-gray-200'
                  onSubmit={handleSubmit(onSubmit)}
                  autoComplete='turnedOff'
                >
                  <div className='mb-8 md:w-3/4'>
                    <TextField
                      id='newTenant'
                      {...register('name', {
                        required: 'This field is required.',
                      })}
                      labelText='Name'
                      isDirty={errors.name ? true : false}
                      errorMsg={errors.name?.message}
                    />
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
        </div>
      </MainLayout>

      {showNotification && (
        <Notification
          title={typeof error === 'string' ? 'Error' : 'Create New Tenant'}
          description={notification.message}
          show={showNotification}
          setShow={setShowNotification}
          status={notification.status}
          autoHide={typeof error === 'string' ? false : true}
        />
      )}
    </>
  );
});
