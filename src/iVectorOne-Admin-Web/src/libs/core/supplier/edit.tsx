import { memo, FC, useState, useEffect, useMemo } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import { useDispatch, useSelector } from 'react-redux';
import { useForm, SubmitHandler } from 'react-hook-form';
//
import {
  getSubscriptionWithSupplierAndConfigurations,
  updateSupplier,
} from '../data-access';
import { RootState } from '@/store';
import { renderConfigurationFormFields } from '@/utils/render-configuration-form-fields';
import { Supplier, SupplierFormFields, Subscription } from '@/types';
import { ButtonColors, ButtonVariants, NotificationStatus } from '@/constants';
import MainLayout from '@/layouts/Main';
import {
  SectionTitle,
  Select,
  Button,
  Spinner,
  Notification,
} from '@/components';

type Props = {};

export const SupplierEdit: FC<Props> = memo(() => {
  const { pathname, search } = useLocation();

  const subscriptionId = search.split('=')[1];
  const supplierId = pathname.split('/')[2];
  const navigate = useNavigate();
  const dispatch = useDispatch();

  const user = useSelector((state: RootState) => state.app.user);

  const [showNotification, setShowNotification] = useState(false);
  const [notification, setNotification] = useState({
    status: NotificationStatus.SUCCESS,
    message: 'Supplier edited successfully.',
  });
  const [currentSubscription, setCurrentSubscription] =
    useState<Subscription | null>(null);
  const [currentSupplier, setCurrentSupplier] = useState<Supplier | null>(null);

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
    if (!activeTenant) return;
    updateSupplier(
      { id: activeTenant.tenantId, key: activeTenant.tenantKey },
      data,
      () => {
        dispatch.app.setIsLoading(true);
      },
      (_supplier) => {
        setNotification({
          status: NotificationStatus.SUCCESS,
          message: 'Supplier modified successfully.',
        });
        setShowNotification(true);
        dispatch.app.setIsLoading(false);
        setTimeout(() => {
          navigate('/suppliers');
        }, 800);
      },
      (_error) => {
        setNotification({
          status: NotificationStatus.ERROR,
          message: 'Supplier edit failed.',
        });
        setShowNotification(true);
        dispatch.app.setIsLoading(false);
      }
    );
  };

  const fetchData = async () => {
    if (!activeTenant) return;
    await getSubscriptionWithSupplierAndConfigurations(
      { id: activeTenant.tenantId, key: activeTenant.tenantKey },
      Number(subscriptionId),
      Number(supplierId),
      () => {
        dispatch.app.setIsLoading(true);
      },
      (subscription, supplier, configurations) => {
        setCurrentSubscription(subscription);
        setCurrentSupplier({ ...supplier, configurations });
        dispatch.app.setIsLoading(false);
      },
      (err) => {
        dispatch.app.setError(err);
        dispatch.app.setIsLoading(false);
      }
    );
  };

  useEffect(() => {
    if (!!user) {
      fetchData();
    }
  }, [user]);

  useEffect(() => {
    if (!!currentSupplier && !!currentSubscription) {
      setValue('subscription', currentSubscription.subscriptionId);
      setValue('supplier', currentSupplier.supplierID);
    }
  }, [currentSupplier, currentSubscription, setValue]);

  return (
    <>
      <MainLayout title={`Edit Supplier ${currentSupplier?.name}`}>
        <div className='bg-white shadow-lg rounded-sm mb-8'>
          <div className='flex flex-col md:flex-row md:-mr-px'>
            <div className='min-w-60'></div>
            <form
              className='grow p-6 w-full divide-y divide-gray-200'
              onSubmit={handleSubmit(onSubmit)}
            >
              <div className='mb-8 flex flex-col gap-5 md:w-1/2'>
                <div className='flex-1'>
                  {currentSubscription !== null ? (
                    <Select
                      id='subscription'
                      {...register('subscription', {
                        required: 'This field is required.',
                      })}
                      labelText='Subscription'
                      options={[
                        {
                          id: currentSubscription.subscriptionId,
                          name: currentSubscription.userName,
                        },
                      ]}
                      disabled
                    />
                  ) : (
                    <Spinner />
                  )}
                </div>
                <div className='flex-1'>
                  {currentSupplier !== null && (
                    <Select
                      id='supplier'
                      {...register('supplier', {
                        required: 'This field is required.',
                      })}
                      labelText='Supplier'
                      options={[
                        {
                          id: currentSupplier.supplierID,
                          name: currentSupplier.name,
                        },
                      ]}
                      disabled
                    />
                  )}
                </div>
                <div className='border-t border-gray-200 mt-2 pt-5'>
                  <SectionTitle title='Settings' />
                  <div className='flex flex-col gap-5 mt-5'>
                    {!!currentSupplier?.configurations?.length &&
                      renderConfigurationFormFields(
                        currentSupplier.configurations || [],
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
