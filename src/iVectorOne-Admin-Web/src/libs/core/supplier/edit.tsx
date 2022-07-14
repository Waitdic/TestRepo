import { memo, FC, useState, useEffect, useMemo } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import { useDispatch, useSelector } from 'react-redux';
import { useForm, SubmitHandler } from 'react-hook-form';
//
import { RootState } from '@/store';
import { renderConfigurationFormFields } from '@/utils/render-configuration-form-fields';
import { setDefaultConfigurationFormFields } from '@/utils/set-default-configuration-form-fields';
import { Supplier, SupplierFormFields, Subscription } from '@/types';
import MainLayout from '@/layouts/Main';
import { ButtonColors, ButtonVariants, NotificationStatus } from '@/constants';
import {
  SectionTitle,
  Select,
  Button,
  Spinner,
  Notification,
} from '@/components';
import { updateSupplier } from '../data-access';

type Props = {};

export const SupplierEdit: FC<Props> = memo(() => {
  const { pathname } = useLocation();
  const supplierId = pathname.split('/')[2];
  const navigate = useNavigate();
  const dispatch = useDispatch();

  const user = useSelector((state: RootState) => state.app.user);
  const subscriptions = useSelector(
    (state: RootState) => state.app.subscriptions
  );
  const isLoading = useSelector((state: RootState) => state.app.isLoading);

  const [currentSupplier, setCurrentSupplier] = useState(
    null as Supplier | null
  );
  const [currentSubscription, setCurrentSubscription] = useState(
    null as Subscription | null
  );
  const [isReady, setIsReady] = useState(false);
  const [showNotification, setShowNotification] = useState(false);
  const [notification, setNotification] = useState({
    status: NotificationStatus.SUCCESS,
    message: 'Supplier edited successfully.',
  });

  const {
    register,
    handleSubmit,
    setValue,
    formState: { errors },
  } = useForm<SupplierFormFields>();

  const activeTenant = useMemo(
    () => user?.tenants?.find((tenant) => tenant.isSelected),
    [user]
  );

  const onSubmit: SubmitHandler<SupplierFormFields> = (data, event) => {
    event?.preventDefault();
    if (!activeTenant || activeTenant == null) return;
    updateSupplier(
      { id: activeTenant.tenantId, key: activeTenant.tenantKey },
      data,
      () => {
        dispatch.app.setIsLoading(true);
      },
      (supplier) => {
        setNotification({
          status: NotificationStatus.SUCCESS,
          message: 'Supplier edited successfully.',
        });
        setShowNotification(true);
        console.log(supplier);
        dispatch.app.setIsLoading(false);
      },
      (error) => {
        setNotification({
          status: NotificationStatus.ERROR,
          message: error || 'Supplier edit failed.',
        });
        setShowNotification(true);
        dispatch.app.setIsLoading(false);
      }
    );
  };

  useEffect(() => {
    if (!!subscriptions?.length) {
      subscriptions.forEach((subscription, idx) => {
        subscription.suppliers.forEach((supplier) => {
          if (supplier.supplierID === Number(supplierId)) {
            setCurrentSubscription(subscriptions[idx]);
            setCurrentSupplier(supplier);
          }
        });
      });
      setIsReady(true);
    }
  }, [subscriptions, supplierId]);

  useEffect(() => {
    if (isReady) {
      if (!currentSupplier && !currentSubscription) {
        navigate('/suppliers');
        return;
      }
      setValue('subscription', currentSubscription?.subscriptionId || 0);
      setValue('supplier', currentSupplier?.supplierID || 0);
      if (!!currentSupplier?.configurations?.length) {
        setDefaultConfigurationFormFields(
          currentSupplier.configurations,
          setValue
        );
      }
    }
    if (!isLoading && !subscriptions.length) {
      navigate('/');
    }
  }, [currentSupplier, isReady, currentSubscription, isLoading, subscriptions]);

  return (
    <>
      <MainLayout>
        <>
          {/* Page header */}
          <div className='mb-8'>
            {/* Title */}
            <h1 className='text-2xl md:text-3xl text-slate-800 font-bold'>
              Edit Suppliers
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
                    {!!currentSubscription?.suppliers?.length ? (
                      <Select
                        id='supplier'
                        {...register('supplier', {
                          required: 'This field is required.',
                        })}
                        labelText='Supplier'
                        options={currentSubscription.suppliers.map(
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
                        currentSupplier?.configurations || [],
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
              : 'Edit Supplier'
          }
          description={notification.message}
          status={notification.status}
          show={showNotification}
          setShow={setShowNotification}
          autoHide={notification.status === NotificationStatus.ERROR}
        />
      )}
    </>
  );
});
