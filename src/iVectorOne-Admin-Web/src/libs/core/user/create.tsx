import React, { useState, useMemo } from 'react';
import { useNavigate } from 'react-router-dom';
import { useForm, SubmitHandler } from 'react-hook-form';
import { useDispatch, useSelector } from 'react-redux';
//
import type { NotificationState } from '@/types';
import { NotificationStatus, ButtonColors, ButtonVariants } from '@/constants';
import MainLayout from '@/layouts/Main';
import { TextField, Button, Notification, RoleGuard } from '@/components';
import { RootState } from '@/store';
import { createUser } from '../data-access/user';

type Props = {};

export type UserFormFields = {
  userName: string;
  subject: string;
};

const UserCreate: React.FC<Props> = () => {
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<UserFormFields>();

  const user = useSelector((state: RootState) => state.app.user);
  const isLoading = useSelector((state: RootState) => state.app.isLoading);

  const [notification, setNotification] = useState<NotificationState>();
  const [showNotification, setShowNotification] = useState(false);

  const activeTenant = useMemo(() => {
    return user?.tenants.find((tenant) => tenant.isSelected);
  }, [user]);

  const onSubmit: SubmitHandler<UserFormFields> = async (data) => {
    if (!activeTenant || isLoading) return;
    await createUser(
      activeTenant?.tenantKey,
      data,
      () => {
        dispatch.app.setIsLoading(true);
      },
      () => {
        dispatch.app.setIsLoading(false);
        setNotification({
          status: NotificationStatus.SUCCESS,
          message: 'New User created successfully.',
        });
        setShowNotification(true);
        setTimeout(() => {
          navigate('/');
        }, 500);
      },
      (err) => {
        dispatch.app.setIsLoading(false);
        console.error(err);
        setNotification({
          status: NotificationStatus.ERROR,
          message: 'User creation failed.',
        });
        setShowNotification(true);
      }
    );
  };

  return (
    <>
      <RoleGuard withRedirect>
        <MainLayout title='Create User'>
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
                      id='userName'
                      {...register('userName', {
                        required: 'This field is required.',
                      })}
                      labelText='User Name'
                      isDirty={!!errors.userName}
                      errorMsg={errors.userName?.message}
                      required
                    />
                  </div>
                  <div>
                    <TextField
                      id='subject'
                      {...register('subject', {
                        required: 'This field is required.',
                      })}
                      labelText='Contact Email'
                      isDirty={!!errors.subject}
                      errorMsg={errors.subject?.message}
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
          status={notification?.status}
          description={notification?.message as string}
          show={showNotification}
          setShow={setShowNotification}
        />
      )}
    </>
  );
};

export default React.memo(UserCreate);
