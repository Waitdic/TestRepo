import { memo, FC, useState, useEffect, useMemo } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import { useSelector } from 'react-redux';
import { useForm, SubmitHandler } from 'react-hook-form';
import axios from 'axios';
//
import { RootState } from '@/store';
import { renderConfigurationFormFields } from '@/utils/render-configuration-form-fields';
import { setDefaultConfigurationFormFields } from '@/utils/set-default-configuration-form-fields';
import {
  Provider,
  ProviderConfiguration,
  ProviderFormFields,
  Subscription,
} from '@/types';
import MainLayout from '@/layouts/Main';
import { ButtonColors, ButtonVariants, NotificationStatus } from '@/constants';
import {
  ErrorBoundary,
  SectionTitle,
  Select,
  Button,
  Spinner,
  Notification,
} from '@/components';
import ApiCall from '@/axios';

type Props = {};

export const ProviderEdit: FC<Props> = memo(({}) => {
  const { pathname } = useLocation();
  const providerId = pathname.split('/')[2];
  const navigate = useNavigate();
  const subscriptions = useSelector(
    (state: RootState) => state.app.subscriptions
  );
  const isLoading = useSelector((state: RootState) => state.app.isLoading);

  const [currentProvider, setCurrentProvider] = useState(
    null as Provider | null
  );
  const [currentSubscription, setCurrentSubscription] = useState(
    null as Subscription | null
  );
  const [isReady, setIsReady] = useState(false);

  const {
    register,
    handleSubmit,
    setValue,
    formState: { errors },
  } = useForm<ProviderFormFields>();

  const [showNotification, setShowNotification] = useState(false);
  const [notification, setNotification] = useState({
    status: NotificationStatus.SUCCESS,
    message: 'Provider edited successfully.',
  });

  const onSubmit: SubmitHandler<ProviderFormFields> = async (data) => {
    try {
      const updatedProvider = await axios.patch(
        'http://localhost:3001/Provider.create',
        data
      );

      console.error(updatedProvider);
      setNotification({
        status: NotificationStatus.SUCCESS,
        message: 'Provider edited successfully.',
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
    if (!!subscriptions?.length) {
      subscriptions.forEach((subscription, idx) => {
        subscription.providers.forEach((provider) => {
          if (provider.supplierID === Number(providerId)) {
            setCurrentSubscription(subscriptions[idx]);
            setCurrentProvider(provider);
          }
        });
      });
      setIsReady(true);
    }
  }, [subscriptions, providerId]);

  useEffect(() => {
    if (isReady) {
      if (!currentProvider && !currentSubscription) {
        navigate('/providers');
        return;
      }
      setValue('subscription', currentSubscription?.subscriptionId || 0);
      setValue('provider', currentProvider?.supplierID || 0);
      if (!!currentProvider?.configurations?.length) {
        setDefaultConfigurationFormFields(
          currentProvider.configurations,
          setValue
        );
      }
    }
    if (!isLoading && !subscriptions.length) {
      navigate('/');
    }
  }, [currentProvider, isReady, currentSubscription, isLoading, subscriptions]);

  return (
    <>
      <MainLayout>
        <>
          {/* Page header */}
          <div className='mb-8'>
            {/* Title */}
            <h1 className='text-2xl md:text-3xl text-slate-800 font-bold'>
              Edit Providers
            </h1>
          </div>

          {/* Content */}
          <div className='bg-white shadow-lg rounded-sm mb-8'>
            <div className='flex flex-col md:flex-row md:-mr-px'>
              <div className='min-w-60'></div>
              <form
                className='grow p-6 w-full divide-y divide-gray-200'
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
                        disabled
                      />
                    ) : (
                      <Spinner />
                    )}
                  </div>
                  <div className='flex-1'>
                    {!!currentSubscription?.providers?.length ? (
                      <Select
                        id='provider'
                        {...register('provider', {
                          required: 'This field is required.',
                        })}
                        labelText='Provider'
                        options={currentSubscription.providers.map(
                          (loginOption) => ({
                            id: loginOption.supplierID,
                            name: loginOption.name,
                          })
                        )}
                        disabled
                      />
                    ) : (
                      <Spinner />
                    )}
                  </div>
                  <div className='border-t border-gray-200 mt-2 pt-5'>
                    <SectionTitle title='Settings' />
                    <div className='flex flex-col gap-5 mt-5'>
                      {renderConfigurationFormFields(
                        currentProvider?.configurations || [],
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
          </div>
        </>
      </MainLayout>

      {showNotification && (
        <Notification
          title={
            notification.status === NotificationStatus.ERROR
              ? 'Error'
              : 'Edit Provider'
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
