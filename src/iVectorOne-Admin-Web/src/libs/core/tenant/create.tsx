import { memo, useState, FC, useEffect, useMemo } from 'react';
import { useNavigate } from 'react-router-dom';
import { useForm, SubmitHandler } from 'react-hook-form';
//
import type { NotificationState, Tenant } from '@/types';
import {
  NotificationStatus,
  ButtonColors,
  ButtonVariants,
  InputTypes,
  PHONE_REGEX,
} from '@/constants';
import MainLayout from '@/layouts/Main';
import { TextField, Button, Notification } from '@/components';
import { createTenant } from '../data-access/tenant';
import { useDispatch, useSelector } from 'react-redux';
import { RootState } from '@/store';

type Props = {
  error: string | null;
};

export const TenantCreate: FC<Props> = memo(({ error }) => {
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<Tenant>();

  const user = useSelector((state: RootState) => state.app.user);
  const isLoading = useSelector((state: RootState) => state.app.isLoading);

  const [notification, setNotification] = useState<NotificationState>({
    status: NotificationStatus.SUCCESS,
    message: 'New Tenant created successfully.',
  });
  const [showNotification, setShowNotification] = useState(false);

  const activeTenant = useMemo(() => {
    return user?.tenants.find((tenant) => tenant.isSelected);
  }, [user]);

  const onSubmit: SubmitHandler<Tenant> = async (data) => {
    if (!activeTenant || isLoading) return;
    await createTenant(
      activeTenant?.tenantKey,
      data,
      () => {
        dispatch.app.setIsLoading(true);
      },
      (tenant) => {
        dispatch.app.setIsLoading(false);
        if (!tenant.success) {
          setNotification({
            status: NotificationStatus.ERROR,
            message: 'Tenant creation failed.',
          });
          setShowNotification(true);
          return;
        }
        setNotification({
          status: NotificationStatus.SUCCESS,
          message: 'New Tenant created successfully.',
        });
        setShowNotification(true);
        setTimeout(() => {
          navigate('/tenants');
        }, 500);
      },
      (err) => {
        dispatch.app.setIsLoading(false);
        console.error(err);
        setNotification({
          status: NotificationStatus.ERROR,
          message: 'Tenant creation failed.',
        });
        setShowNotification(true);
      }
    );
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
      <MainLayout title='Create Tenant'>
        <div className='bg-white shadow-lg rounded-sm mb-8'>
          <div className='flex flex-col md:flex-row md:-mr-px'>
            <div className='min-w-60'></div>
            <form
              className='w-full divide-y divide-gray-200 p-6'
              onSubmit={handleSubmit(onSubmit)}
              autoComplete='turnedOff'
            >
              <div className='flex flex-col gap-5'>
                <div>
                  <TextField
                    id='companyName'
                    {...register('companyName', {
                      required: 'This field is required.',
                    })}
                    labelText='Company Name'
                    isDirty={!!errors.companyName}
                    errorMsg={errors.companyName?.message}
                    required
                  />
                </div>
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
                      pattern: {
                        value: PHONE_REGEX,
                        message: 'Invalid phone number.',
                      },
                    })}
                    labelText='Contact Telephone'
                    isDirty={!!errors.contactTelephone}
                    errorMsg={errors.contactTelephone?.message}
                    required
                  />
                </div>
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
