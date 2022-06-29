import { memo, FC, useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useSelector } from 'react-redux';
import { useForm, SubmitHandler } from 'react-hook-form';
import axios from 'axios';
//
import { RootState } from '@/store';
import { renderConfigurationFormFields } from '@/utils/render-configuration-form-fields';
import { setDefaultConfigurationFormFields } from '@/utils/set-default-configuration-form-fields';
import { ProviderConfiguration, ProviderFormFields } from '@/types';
import { ButtonColors, ButtonVariants, NotificationStatus } from '@/constants';
import MainLayout from '@/layouts/Main';
import {
  ErrorBoundary,
  SectionTitle,
  Select,
  Button,
  Spinner,
  Notification,
} from '@/components';

type Props = {
  error: string | null;
};

export const ProviderCreate: FC<Props> = memo(({ error }) => {
  // const {
  //   configurations,
  //   error: providerInfoError,
  //   isLoading,
  // } = useProviderInfo();

  //! Temporary
  const providerInfoError = null;
  const isLoading = false;
  const configurations: ProviderConfiguration[] = [
    {
      defaultValue: '',
      description: '',
      key: 'string_field',
      maximum: 50,
      minimum: 10,
      name: 'Username',
      order: 1,
      required: true,
      type: 'string',
    },
    {
      defaultValue: '',
      description: '',
      key: 'email_field',
      maxLength: 100,
      minLength: 6,
      name: 'Email Address',
      order: 2,
      required: true,
      type: 'email',
    },
    {
      defaultValue: '',
      description: '',
      key: 'password_field',
      maxLength: 100,
      minLength: 8,
      name: 'Password',
      order: 3,
      required: true,
      type: 'password',
    },
    {
      description: '',
      dropdownOptions: [
        { id: 'en', name: 'English' },
        { id: 'de', name: 'German' },
      ],
      key: 'dropdown_field',
      name: 'Language',
      order: 4,
      type: 'dropdown',
    },
    {
      description: '',
      dropdownOptions: [
        { id: 'eur', name: 'EUR' },
        { id: 'usd', name: 'USD' },
      ],
      key: 'dropdown_field',
      name: 'Currency',
      order: 5,
      type: 'dropdown',
    },
    {
      defaultValue: false,
      description: '',
      key: 'boolean_field',
      name: 'Allow Cancellations',
      order: 6,
      type: 'boolean',
    },
    {
      description: '',
      key: 'string_field',
      name: 'Supplier Reference',
      order: 7,
      required: true,
      type: 'string',
    },
    {
      defaultValue: false,
      description: '',
      key: 'boolean_field',
      name: 'Use GZip',
      order: 8,
      type: 'boolean',
    },
    {
      description: '',
      key: 'uri_field',
      name: 'Search URL',
      order: 9,
      required: true,
      type: 'uri',
    },
    {
      description: '',
      key: 'uri_field',
      name: 'Book URL',
      order: 10,
      required: true,
      type: 'uri',
    },
    {
      description: '',
      key: 'uri_field',
      name: 'Cancel URL',
      order: 11,
      required: true,
      type: 'uri',
    },
    {
      defaultValue: 0,
      description: '',
      key: 'number_field',
      maximum: 30,
      minimum: 0,
      name: 'Offset Cancellation Days',
      order: 12,
      required: true,
      type: 'number',
    },
    {
      description: '',
      dropdownOptions: [
        { id: 'gb', name: 'GB' },
        { id: 'us', name: 'USA' },
      ],
      key: 'dropdown_field',
      name: 'Nationality',
      order: 13,
      type: 'dropdown',
    },
    {
      description: '',
      key: 'uri_field',
      name: 'Pre Book URL',
      order: 14,
      required: true,
      type: 'uri',
    },
    {
      description: '',
      key: 'string_field',
      name: 'Accommodation Types',
      order: 15,
      required: true,
      type: 'string',
    },
    {
      defaultValue: false,
      description: '',
      key: 'boolean_field',
      name: 'Request Package Rates',
      order: 16,
      type: 'boolean',
    },
  ];

  const navigate = useNavigate();
  const subscriptions = useSelector(
    (state: RootState) => state.app.subscriptions
  );
  const providers = useSelector((state: RootState) => state.app.providers);
  const {
    register,
    handleSubmit,
    setValue,
    formState: { errors },
  } = useForm<ProviderFormFields>();

  const [showNotification, setShowNotification] = useState(false);
  const [notification, setNotification] = useState({
    status: NotificationStatus.SUCCESS,
    message: 'New Provider created successfully.',
  });

  const onSubmit: SubmitHandler<ProviderFormFields> = async (data) => {
    try {
      const newProvider = await axios.post(
        'http://localhost:3001/provider.create',
        data
      );

      setNotification({
        status: NotificationStatus.SUCCESS,
        message: 'New Provider created successfully.',
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
    if (!isLoading) {
      if (providerInfoError) {
        setNotification({
          status: NotificationStatus.ERROR,
          message: 'Provider Info fetching failed.',
        });
        setShowNotification(true);

        return;
      }

      if (configurations.length > 0) {
        setDefaultConfigurationFormFields(configurations, setValue);
      }

      if (subscriptions.length > 0) {
        setValue('subscription', subscriptions[0].userName);
      }
    }
  }, [isLoading, providerInfoError, configurations, subscriptions, setValue]);

  return (
    <>
      <MainLayout>
        <div className='flex flex-col'>
          {/* Create Provider */}
          {error ? (
            <ErrorBoundary />
          ) : (
            <div className='mb-6'>
              <h2 className='md:text-3xl text-2xl font-semibold sm:font-medium text-gray-900 mb-5 pb-3 md:mb-8 md:pb-6'>
                New Provider
              </h2>
              <form
                className='w-full divide-y divide-gray-200'
                onSubmit={handleSubmit(onSubmit)}
              >
                <div className='mb-8 flex flex-col gap-5 md:w-1/2'>
                  <div className='flex-1'>
                    {subscriptions.length > 0 ? (
                      <Select
                        id='subscription'
                        {...register('subscription', {
                          required: 'This field is required.',
                        })}
                        labelText='Subscription'
                        options={subscriptions.map(
                          ({ subscriptionId, userName }) => ({
                            id: subscriptionId,
                            name: userName,
                          })
                        )}
                      />
                    ) : (
                      <Spinner />
                    )}
                  </div>
                  <div className='flex-1'>
                    {providers.length > 0 ? (
                      <Select
                        id='provider'
                        {...register('provider', {
                          required: 'This field is required.',
                        })}
                        labelText='Provider'
                        options={providers.map((loginOption) => ({
                          id: loginOption.name,
                          name: loginOption.name,
                        }))}
                      />
                    ) : (
                      <Spinner />
                    )}
                  </div>
                  <div className='border-t border-gray-200 mt-2 pt-5'>
                    <SectionTitle title='Settings' />
                    <div className='flex flex-col gap-5 mt-5'>
                      {renderConfigurationFormFields(
                        configurations,
                        register,
                        errors
                      )}
                    </div>
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
          )}
        </div>
      </MainLayout>

      {showNotification && (
        <Notification
          title={
            notification.status === NotificationStatus.ERROR
              ? 'Error'
              : 'Create New Provider'
          }
          description={notification.message}
          status={notification.status}
          show={showNotification}
          setShow={setShowNotification}
          autoHide={
            notification.status === NotificationStatus.ERROR ? false : true
          }
        />
      )}
    </>
  );
});
