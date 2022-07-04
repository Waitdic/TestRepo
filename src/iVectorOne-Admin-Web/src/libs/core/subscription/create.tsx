import { memo, FC, useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useForm, SubmitHandler } from 'react-hook-form';
import { pick } from 'lodash';
import axios from 'axios';
//
import { Subscription } from '@/types';
import {
  InputTypes,
  ButtonColors,
  ButtonVariants,
  NotificationStatus,
} from '@/constants';
import MainLayout from '@/layouts/Main';
import {
  ErrorBoundary,
  SectionTitle,
  Button,
  TextField,
  Select,
  Toggle,
  Notification,
} from '@/components';

type Props = {
  error: string | null;
};

export const SubscriptionCreate: FC<Props> = memo(({ error }) => {
  const navigate = useNavigate();
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<Subscription>();

  const [showNotification, setShowNotification] = useState(false);
  const [notification, setNotification] = useState({
    status: NotificationStatus.SUCCESS,
    message: 'Create New Subscription',
  });

  const onSubmit: SubmitHandler<Subscription> = async (data) => {
    const subscriptionData = pick(data, [
      'name',
      'userName',
      'password',
      'PropertyTPRequestLimit',
      'SearchTimeoutSeconds',
      'LogMainSearchError',
      'CurrencyCode',
    ]);

    try {
      const newSubscription = await axios.post(
        'http://localhost:3001/subsciption.create',
        subscriptionData
      );

      setNotification({
        status: NotificationStatus.SUCCESS,
        message: 'New Subscription created successfully.',
      });
      setShowNotification(true);
    } catch (error) {
      if (typeof error === 'string') {
        console.error(error.toUpperCase());
        setNotification({
          status: NotificationStatus.ERROR,
          message: error.toUpperCase(),
        });
      } else if (error instanceof Error) {
        console.error(error.message);
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
          {/* Create Subscription */}
          <div className='mb-6'>
            {typeof error === 'string' ? (
              <ErrorBoundary />
            ) : (
              <>
                <h2 className='md:text-3xl text-2xl font-semibold sm:font-medium text-gray-900 mb-5 pb-3 md:mb-8 md:pb-6'>
                  New Subscription
                </h2>
                <form
                  className='w-full divide-y divide-gray-200'
                  onSubmit={handleSubmit(onSubmit)}
                  autoComplete='turnedOff'
                >
                  <div className='mb-8 flex flex-col gap-5'>
                    <div className='flex-1'>
                      <SectionTitle title='Subscription' />
                    </div>
                    {/* TODO */}
                    {/* <div className='flex-1 md:w-3/4'>
                      <TextField
                        id='name'
                        {...register('name', {
                          required: 'This field is required.',
                        })}
                        labelText='Name'
                        isDirty={errors.name ? true : false}
                        errorMsg={errors.name?.message}
                      />
                    </div> */}
                    <div className='flex-1 md:w-1/2'>
                      <TextField
                        id='userName'
                        {...register('userName', {
                          required: 'This field is required.',
                        })}
                        labelText='Username'
                        isDirty={errors.userName ? true : false}
                        errorMsg={errors.userName?.message}
                      />
                    </div>
                    <div className='flex-1 md:w-1/2'>
                      <TextField
                        id='password'
                        type={InputTypes.PASSWORD}
                        {...register('password', {
                          required: 'This field is required.',
                          minLength: {
                            value: 8,
                            message: 'Password must be at least 8 characters.',
                          },
                        })}
                        labelText='Password'
                        isDirty={errors.password ? true : false}
                        errorMsg={errors.password?.message}
                      />
                    </div>
                    <div className='flex-1'>
                      <SectionTitle title='Settings' />
                    </div>
                    <div className='flex-1 md:w-1/2'>
                      <TextField
                        id='propertyTprequestLimit'
                        type={InputTypes.NUMBER}
                        {...register('propertyTprequestLimit', {
                          required: 'This field is required.',
                        })}
                        labelText='Property TP Request Limit'
                        isDirty={errors.propertyTprequestLimit ? true : false}
                        errorMsg={errors.propertyTprequestLimit?.message}
                      />
                    </div>
                    <div className='flex-1 md:w-1/2'>
                      <TextField
                        id='searchTimeoutSeconds'
                        type={InputTypes.NUMBER}
                        {...register('searchTimeoutSeconds', {
                          required: 'This field is required.',
                        })}
                        labelText='Search Timeout Seconds'
                        isDirty={errors.searchTimeoutSeconds ? true : false}
                        errorMsg={errors.searchTimeoutSeconds?.message}
                      />
                    </div>
                    <div className='flex-1 md:w-1/2'>
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
                    <div className='flex-1 md:w-1/2'>
                      <Toggle
                        id='logMainSearchError'
                        {...register('logMainSearchError')}
                        labelText='Log Main Search Error'
                        isDirty={errors.logMainSearchError ? true : false}
                        errorMsg={errors.logMainSearchError?.message}
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
              </>
            )}
          </div>
        </div>
      </MainLayout>

      {showNotification && (
        <Notification
          title={
            notification.status === NotificationStatus.ERROR
              ? 'Error'
              : 'Create New Subscription'
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