import { memo, useEffect, useState, FC, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { useForm, SubmitHandler } from 'react-hook-form';
import axios from 'axios';
//
import { Subscription } from '@/types';
import { useSlug } from '@/utils/use-slug';
import {
  NotificationStatus,
  InputTypes,
  ButtonColors,
  ButtonVariants,
} from '@/constants';
import MainLayout from '@/layouts/Main';
import {
  ErrorBoundary,
  SectionTitle,
  Toggle,
  Notification,
  Select,
  Button,
  TextField,
  Spinner,
} from '@/components';
import { RootState } from '@/store';
import { useSelector } from 'react-redux';

type NotificationState = {
  status: NotificationStatus;
  message: string;
};

type Props = {};

interface SubscriptionFields extends Subscription {
  confirmPassword: string;
}

export const SubscriptionEdit: FC<Props> = memo(({}) => {
  const navigate = useNavigate();
  const { slug } = useSlug();

  const subscriptions = useSelector(
    (state: RootState) => state.app.subscriptions
  );

  const [notification, setNotification] = useState<NotificationState>({
    status: NotificationStatus.SUCCESS,
    message: 'Subscription edited successfully.',
  });
  const [showNotification, setShowNotification] = useState<boolean>(false);

  const {
    register,
    handleSubmit,
    setValue,
    formState: { errors },
  } = useForm<SubscriptionFields>();

  const onSubmit: SubmitHandler<SubscriptionFields> = async (data) => {
    try {
      const updatedSubscription = await axios.patch(
        `http://localhost:3001/subscription/edit/${slug}`,
        data
      );

      setNotification({
        status: NotificationStatus.SUCCESS,
        message: 'Subscription edited successfully.',
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

  const loadSubscription = useCallback(() => {
    if (subscriptions.length > 0) {
      const currentSubscription = subscriptions.filter(
        (sub) => sub.subscriptionId === Number(slug)
      )[0];

      if (!currentSubscription) {
        navigate('/subscriptions');
      } else {
        //! TODO setValue('name', currentSubscription.name);
        setValue('userName', currentSubscription.userName);
        setValue(
          'propertyTprequestLimit',
          currentSubscription.propertyTprequestLimit
        );
        setValue(
          'searchTimeoutSeconds',
          currentSubscription.searchTimeoutSeconds
        );
        setValue('logMainSearchError', currentSubscription.logMainSearchError);
        setValue('currencyCode', currentSubscription.currencyCode);
      }
    }
  }, [subscriptions, navigate, setValue, slug]);

  useEffect(() => {
    loadSubscription();
  }, [subscriptions, navigate, setValue, slug, loadSubscription]);

  return (
    <>
      <MainLayout maxWidth='max-w-3xl'>
        <div className='flex flex-col'>
          <h1 className='text-2xl md:text-3xl text-slate-800 font-bold mb-5 pb-3 md:mb-8 md:pb-6'>
            Edit Subscriptions
          </h1>
          <form
            className='w-full divide-y divide-gray-200'
            onSubmit={handleSubmit(onSubmit)}
            autoComplete='turnedOff'
          >
            <div className='flex flex-col gap-5 mb-8'>
              <div className='flex-1'>
                <SectionTitle title='Subscription' />
              </div>
              {subscriptions.length > 0 ? (
                <>
                  <div className='flex-1 md:w-3/4'>
                    {/* //! TODO */}
                    {/* <TextField
                            id='name'
                            {...register('name', {
                              required: 'This field is required.',
                            })}
                            labelText='Name'
                            isDirty={errors.name ? true : false}
                            errorMsg={errors.name?.message}
                          /> */}
                  </div>
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
                </>
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
        </div>
      </MainLayout>

      {showNotification && (
        <Notification
          title='Edit Subscription'
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
