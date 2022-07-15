import { memo, FC, useState, useEffect, useMemo } from 'react';
import { useNavigate } from 'react-router-dom';
import { useSelector } from 'react-redux';
import { useForm, SubmitHandler } from 'react-hook-form';
import axios from 'axios';
//
import { RootState } from '@/store';
import { renderConfigurationFormFields } from '@/utils/render-configuration-form-fields';
import { setDefaultConfigurationFormFields } from '@/utils/set-default-configuration-form-fields';
import { SupplierConfiguration, SupplierFormFields } from '@/types';
import {
  ButtonColors,
  ButtonVariants,
  ConfigurationFormFieldTypes,
  NotificationStatus,
} from '@/constants';
import MainLayout from '@/layouts/Main';
import {
  SectionTitle,
  Select,
  Button,
  Spinner,
  Notification,
} from '@/components';

type Props = {};

export const SupplierCreate: FC<Props> = memo(() => {
  //! Temporary
  const supplierInfoError = null;
  const configurations: SupplierConfiguration[] = [
    {
      supplierSubscriptionAttributeID: Number(
        Math.round(Math.random() * 100).toFixed(0)
      ),
      defaultValue: '',
      description: '',
      key: 'string_field',
      maximum: 50,
      minimum: 10,
      name: 'Username',
      order: 1,
      required: true,
      type: ConfigurationFormFieldTypes.STRING,
    },
    {
      supplierSubscriptionAttributeID: Number(
        Math.round(Math.random() * 100).toFixed(0)
      ),
      defaultValue: '',
      description: '',
      key: 'email_field',
      maxLength: 100,
      minLength: 6,
      name: 'Email Address',
      order: 2,
      required: true,
      type: ConfigurationFormFieldTypes.EMAIL,
    },
    {
      supplierSubscriptionAttributeID: Number(
        Math.round(Math.random() * 100).toFixed(0)
      ),
      defaultValue: '',
      description: '',
      key: 'password_field',
      maxLength: 100,
      minLength: 8,
      name: 'Password',
      order: 3,
      required: true,
      type: ConfigurationFormFieldTypes.PASSWORD,
    },
    {
      supplierSubscriptionAttributeID: Number(
        Math.round(Math.random() * 100).toFixed(0)
      ),
      description: '',
      dropdownOptions: [
        { id: 'en', name: 'English' },
        { id: 'de', name: 'German' },
      ],
      key: 'dropdown_field',
      name: 'Language',
      order: 4,
      type: ConfigurationFormFieldTypes.DROPDOWN,
    },
    {
      supplierSubscriptionAttributeID: Number(
        Math.round(Math.random() * 100).toFixed(0)
      ),
      description: '',
      dropdownOptions: [
        { id: 'eur', name: 'EUR' },
        { id: 'usd', name: 'USD' },
      ],
      key: 'dropdown_field',
      name: 'Currency',
      order: 5,
      type: ConfigurationFormFieldTypes.DROPDOWN,
    },
    {
      supplierSubscriptionAttributeID: Number(
        Math.round(Math.random() * 100).toFixed(0)
      ),
      defaultValue: false,
      description: '',
      key: 'boolean_field',
      name: 'Allow Cancellations',
      order: 6,
      type: ConfigurationFormFieldTypes.BOOLEAN,
    },
    {
      supplierSubscriptionAttributeID: Number(
        Math.round(Math.random() * 100).toFixed(0)
      ),
      description: '',
      key: 'string_field',
      name: 'Supplier Reference',
      order: 7,
      required: true,
      type: ConfigurationFormFieldTypes.STRING,
    },
    {
      supplierSubscriptionAttributeID: Number(
        Math.round(Math.random() * 100).toFixed(0)
      ),
      defaultValue: false,
      description: '',
      key: 'boolean_field',
      name: 'Use GZip',
      order: 8,
      type: ConfigurationFormFieldTypes.BOOLEAN,
    },
    {
      supplierSubscriptionAttributeID: Number(
        Math.round(Math.random() * 100).toFixed(0)
      ),
      description: '',
      key: 'uri_field',
      name: 'Search URL',
      order: 9,
      required: true,
      type: ConfigurationFormFieldTypes.URI,
    },
    {
      supplierSubscriptionAttributeID: Number(
        Math.round(Math.random() * 100).toFixed(0)
      ),
      description: '',
      key: 'uri_field',
      name: 'Book URL',
      order: 10,
      required: true,
      type: ConfigurationFormFieldTypes.URI,
    },
    {
      supplierSubscriptionAttributeID: Number(
        Math.round(Math.random() * 100).toFixed(0)
      ),
      description: '',
      key: 'uri_field',
      name: 'Cancel URL',
      order: 11,
      required: true,
      type: ConfigurationFormFieldTypes.URI,
    },
    {
      supplierSubscriptionAttributeID: Number(
        Math.round(Math.random() * 100).toFixed(0)
      ),
      defaultValue: 0,
      description: '',
      key: 'number_field',
      maximum: 30,
      minimum: 0,
      name: 'Offset Cancellation Days',
      order: 12,
      required: true,
      type: ConfigurationFormFieldTypes.NUMBER,
    },
    {
      supplierSubscriptionAttributeID: Number(
        Math.round(Math.random() * 100).toFixed(0)
      ),
      description: '',
      dropdownOptions: [
        { id: 'gb', name: 'GB' },
        { id: 'us', name: 'USA' },
      ],
      key: 'dropdown_field',
      name: 'Nationality',
      order: 13,
      type: ConfigurationFormFieldTypes.DROPDOWN,
    },
    {
      supplierSubscriptionAttributeID: Number(
        Math.round(Math.random() * 100).toFixed(0)
      ),
      description: '',
      key: 'uri_field',
      name: 'Pre Book URL',
      order: 14,
      required: true,
      type: ConfigurationFormFieldTypes.URI,
    },
    {
      supplierSubscriptionAttributeID: Number(
        Math.round(Math.random() * 100).toFixed(0)
      ),
      description: '',
      key: 'string_field',
      name: 'Accommodation Types',
      order: 15,
      required: true,
      type: ConfigurationFormFieldTypes.STRING,
    },
    {
      supplierSubscriptionAttributeID: Number(
        Math.round(Math.random() * 100).toFixed(0)
      ),
      defaultValue: false,
      description: '',
      key: 'boolean_field',
      name: 'Request Package Rates',
      order: 16,
      type: ConfigurationFormFieldTypes.BOOLEAN,
    },
  ];

  const navigate = useNavigate();

  const subscriptions = useSelector(
    (state: RootState) => state.app.subscriptions
  );
  const isLoading = useSelector((state: RootState) => state.app.isLoading);

  const suppliers = useMemo(
    () => subscriptions.flatMap((subscription) => subscription.suppliers),
    [subscriptions]
  );

  const {
    register,
    handleSubmit,
    setValue,
    formState: { errors },
  } = useForm<SupplierFormFields>();

  const [showNotification, setShowNotification] = useState(false);
  const [notification, setNotification] = useState({
    status: NotificationStatus.SUCCESS,
    message: 'New Supplier created successfully.',
  });

  const onSubmit: SubmitHandler<SupplierFormFields> = async (data) => {
    try {
      const newSupplier = await axios.post(
        'http://localhost:3001/supplier.create',
        data
      );

      setNotification({
        status: NotificationStatus.SUCCESS,
        message: 'New supplier created successfully.',
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
      if (!subscriptions?.length) {
        navigate('/');
        return;
      }
      if (supplierInfoError) {
        setNotification({
          status: NotificationStatus.ERROR,
          message: 'Supplier Info fetching failed.',
        });
        setShowNotification(true);
        return;
      }

      if (configurations.length > 0) {
        setDefaultConfigurationFormFields(configurations, setValue);
      }

      if (subscriptions.length > 0) {
        setValue('subscription', subscriptions[0].subscriptionId);
      }
    }
  }, [isLoading, supplierInfoError, configurations, subscriptions, setValue]);

  return (
    <>
      <MainLayout>
        <div className='flex flex-col'>
          {/* Create Supplier */}
          <div className='mb-6'>
            <h2 className='md:text-3xl text-2xl font-semibold sm:font-medium text-gray-900 mb-5 pb-3 md:mb-8 md:pb-6'>
              New Supplier
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
                  {suppliers.length > 0 ? (
                    <Select
                      id='supplier'
                      {...register('supplier', {
                        required: 'This field is required.',
                      })}
                      labelText='Supplier'
                      options={suppliers.map((loginOption) => ({
                        id: loginOption.supplierID,
                        name: loginOption.supplierName,
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
        </div>
      </MainLayout>

      {showNotification && (
        <Notification
          title={
            notification.status === NotificationStatus.ERROR
              ? 'Error'
              : 'Create New Supplier'
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
