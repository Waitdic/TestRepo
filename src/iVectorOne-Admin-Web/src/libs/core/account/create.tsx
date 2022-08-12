import { memo, FC, useState, useEffect, useMemo } from 'react';
import { useNavigate } from 'react-router-dom';
import { useForm, SubmitHandler } from 'react-hook-form';
import { useDispatch, useSelector } from 'react-redux';
//
import { RootState } from '@/store';
import { Account } from '@/types';
import {
  InputTypes,
  ButtonColors,
  ButtonVariants,
  NotificationStatus,
} from '@/constants';
import MainLayout from '@/layouts/Main';
import {
  SectionTitle,
  Button,
  TextField,
  Select,
  Notification,
} from '@/components';
import { createAccount } from '../data-access/account';

type Props = {};

export const AccountCreate: FC<Props> = memo(() => {
  const dispatch = useDispatch();
  const navigate = useNavigate();

  const user = useSelector((state: RootState) => state.app.user);
  const isLoading = useSelector((state: RootState) => state.app.isLoading);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<Account>();

  const [showNotification, setShowNotification] = useState(false);
  const [notification, setNotification] = useState({
    status: NotificationStatus.SUCCESS,
    message: 'Create New Account',
  });

  const activeTenant = useMemo(
    () => user?.tenants?.find((tenant) => tenant.isSelected),
    [user]
  );

  const onSubmit: SubmitHandler<Account> = async (data) => {
    if (!activeTenant) return;
    await createAccount(
      {
        id: activeTenant.tenantId,
        key: activeTenant.tenantKey,
      },
      {
        UserName: data.userName,
        PropertyTpRequestLimit: data.propertyTprequestLimit.toString(),
        SearchTimeoutSeconds: data.searchTimeoutSeconds.toString(),
        CurrencyCode: data.currencyCode,
      },
      () => {
        dispatch.app.setIsLoading(true);
      },
      () => {
        dispatch.app.setIsLoading(false);
        setNotification({
          status: NotificationStatus.SUCCESS,
          message: 'New Account Created',
        });
        setShowNotification(true);
        setTimeout(() => {
          navigate('/accounts');
        }, 500);
      },
      () => {
        dispatch.app.setIsLoading(false);
        setNotification({
          status: NotificationStatus.ERROR,
          message: 'Error Creating Account',
        });
        setShowNotification(true);
      }
    );
  };

  useEffect(() => {
    if (!isLoading && !user?.fullName) {
      navigate('/');
    }
  }, [isLoading, user]);

  return (
    <>
      <MainLayout title='New Account'>
        <div className='bg-white shadow-lg rounded-sm mb-8'>
          <div className='flex flex-col md:flex-row md:-mr-px'>
            <div className='min-w-60'></div>
            <form
              className='grow p-6 w-full divide-y divide-gray-200'
              onSubmit={handleSubmit(onSubmit)}
            >
              <div className='mb-8 flex flex-col gap-5 md:w-1/2'>
                <div className='flex-1'>
                  <SectionTitle title='Account' />
                </div>
                <div className='flex-1'>
                  <TextField
                    id='userName'
                    {...register('userName', {
                      required: 'This field is required.',
                    })}
                    labelText='Username'
                    isDirty={!!errors.userName}
                    errorMsg={errors.userName?.message}
                  />
                </div>
                <div className='flex-1'>
                  <SectionTitle title='Settings' />
                </div>
                <div className='flex-1'>
                  <TextField
                    id='propertyTprequestLimit'
                    type={InputTypes.NUMBER}
                    {...register('propertyTprequestLimit', {
                      required: 'This field is required.',
                    })}
                    labelText='Maximum Single Request Property Search Limit'
                    isDirty={!!errors.propertyTprequestLimit}
                    errorMsg={errors.propertyTprequestLimit?.message}
                  />
                </div>
                <div className='flex-1'>
                  <TextField
                    id='searchTimeoutSeconds'
                    type={InputTypes.NUMBER}
                    {...register('searchTimeoutSeconds', {
                      required: 'This field is required.',
                    })}
                    labelText='Search Timeout (seconds)'
                    isDirty={!!errors.searchTimeoutSeconds}
                    errorMsg={errors.searchTimeoutSeconds?.message}
                  />
                </div>
                <div className='flex-1'>
                  <Select
                    id='currencyCode'
                    {...register('currencyCode', {
                      required: 'This field is required.',
                    })}
                    labelText='Currency Code'
                    options={[
                      {
                        id: 'GBP',
                        name: 'GBP',
                      },
                      {
                        id: 'USD',
                        name: 'USD',
                      },
                      {
                        id: 'EUR',
                        name: 'EUR',
                      },
                    ]}
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
          title={
            notification.status === NotificationStatus.ERROR
              ? 'Error'
              : 'Create New Account'
          }
          description={notification.message}
          status={notification.status}
          show={showNotification}
          setShow={setShowNotification}
        />
      )}
    </>
  );
});
