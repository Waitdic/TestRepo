import React, { useState, useMemo, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { useForm, SubmitHandler } from 'react-hook-form';
import { useDispatch, useSelector } from 'react-redux';
//
import type { NotificationState, Tenant } from '@/types';
import {
  NotificationStatus,
  ButtonColors,
  ButtonVariants,
  InputTypes,
} from '@/constants';
import MainLayout from '@/layouts/Main';
import { TextField, Button, Notification, RoleGuard } from '@/components';
import { createTenant } from '../data-access/tenant';
import { RootState } from '@/store';
import { refetchUserData } from '../data-access';

type Props = {};

const TenantCreate: React.FC<Props> = () => {
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<Tenant>();

  const userKey = useSelector(
    (state: RootState) => state.app.awsAmplify.username
  );
  const isLoading = useSelector((state: RootState) => state.app.isLoading);

  const [notification, setNotification] = useState<NotificationState>({
    status: NotificationStatus.SUCCESS,
    message: 'New Tenant created successfully.',
  });
  const [showNotification, setShowNotification] = useState(false);

  const isValidUser = useMemo(
    () => !!userKey || isLoading,
    [userKey, isLoading]
  );

  const refetchUser = useCallback(async () => {
    if (!isValidUser) return;
    await refetchUserData(
      userKey as string,
      () => {
        dispatch.app.setIsLoading(true);
      },
      (freshUser) => {
        dispatch.app.setIsLoading(false);
        dispatch.app.updateUser(freshUser);
        setTimeout(() => {
          navigate('/tenants');
        }, 500);
      },
      (err) => {
        console.error(err);
        dispatch.app.setIsLoading(false);
        setNotification({
          status: NotificationStatus.ERROR,
          message: 'Error while updating user data.',
        });
        setShowNotification(true);
      }
    );
  }, []);

  const onSubmit: SubmitHandler<Tenant> = useCallback(
    async (data) => {
      if (!isValidUser) return;
      await createTenant(
        userKey as string,
        data,
        () => {
          dispatch.app.setIsLoading(true);
        },
        () => {
          dispatch.app.setIsLoading(false);
          setNotification({
            status: NotificationStatus.SUCCESS,
            message: 'New Tenant created successfully.',
          });
          setShowNotification(true);
          refetchUser();
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
    },
    [isValidUser]
  );

  return (
    <>
      <RoleGuard withRedirect>
        <MainLayout title='Create Tenant'>
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
      </RoleGuard>

      {showNotification && (
        <Notification
          status={notification.status}
          description={notification.message}
          show={showNotification}
          setShow={setShowNotification}
        />
      )}
    </>
  );
};

export default React.memo(TenantCreate);
