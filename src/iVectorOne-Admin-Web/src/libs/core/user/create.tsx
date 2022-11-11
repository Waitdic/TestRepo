import React, { useMemo, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { useForm, SubmitHandler } from 'react-hook-form';
import { useDispatch, useSelector } from 'react-redux';
//
import { NotificationStatus, ButtonColors, ButtonVariants } from '@/constants';
import MainLayout from '@/layouts/Main';
import { TextField, Button, RoleGuard } from '@/components';
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
  const userKey = useSelector(
    (state: RootState) => state.app.awsAmplify.username
  );
  const isLoading = useSelector((state: RootState) => state.app.isLoading);

  const activeTenant = useMemo(() => {
    return user?.tenants.find((tenant) => tenant.isSelected);
  }, [user]);
  const isValidUser = useMemo(() => {
    return !!userKey && !isLoading && !!activeTenant;
  }, [userKey, isLoading, activeTenant]);

  const onSubmit: SubmitHandler<UserFormFields> = useCallback(
    async (data) => {
      if (!isValidUser) return;
      await createUser(
        activeTenant?.tenantKey as string,
        userKey as string,
        data,
        () => {
          dispatch.app.setIsLoading(true);
        },
        () => {
          dispatch.app.setIsLoading(false);
          dispatch.app.setNotification({
            status: NotificationStatus.SUCCESS,
            message: 'New User created successfully.',
          });

          setTimeout(() => {
            navigate('/');
          }, 500);
        },
        (err, instance) => {
          dispatch.app.setIsLoading(false);
          console.error(err);
          dispatch.app.setNotification({
            status: NotificationStatus.ERROR,
            message: err,
            instance,
          });
        }
      );
    },
    [isValidUser]
  );

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
                      labelText='Subject'
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
    </>
  );
};

export default React.memo(UserCreate);
