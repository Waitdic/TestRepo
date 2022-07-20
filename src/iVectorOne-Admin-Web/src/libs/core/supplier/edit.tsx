import { memo, FC, useState, useEffect, useMemo } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import { useDispatch, useSelector } from 'react-redux';
import { useForm, SubmitHandler } from 'react-hook-form';
//
import { RootState } from '@/store';
import { renderConfigurationFormFields } from '@/utils/render-configuration-form-fields';
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
import {
  getSubscriptionsWithSuppliers,
  getSupplierById,
  updateSupplier,
} from '../data-access';

type Props = {};

export const SupplierEdit: FC<Props> = memo(() => {
  const { pathname, search } = useLocation();

  const subscriptionId = search.split('=')[1];
  const supplierId = pathname.split('/')[2];
  const navigate = useNavigate();
  const dispatch = useDispatch();

  const user = useSelector((state: RootState) => state.app.user);
  const subscriptions = useSelector(
    (state: RootState) => state.app.subscriptions
  );
  console.log(subscriptions);

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
    if (!activeTenant || activeTenant == null) return;
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

  const fetchData = async () => {
    if (!activeTenant || activeTenant == null) return;
    await getSupplierById(
      { id: activeTenant.tenantId, key: activeTenant.tenantKey },
      Number(subscriptionId),
      Number(supplierId),
      () => {
        dispatch.app.setIsLoading(true);
      },
      (fetchedSupplier) => {
        dispatch.app.updateSubscriptions(
          subscriptions.map((sub) => {
            if (sub.subscriptionId === Number(subscriptionId)) {
              sub.suppliers = sub.suppliers.map((supplier) => {
                if (supplier.supplierID === Number(supplierId)) {
                  return fetchedSupplier;
                }
                return supplier;
              });
            }
            return sub;
          })
        );
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
    if (!!subscriptions?.length && !!user) {
      subscriptions.forEach((subscription) => {
        if (subscription.subscriptionId === Number(subscriptionId)) {
          setCurrentSubscription(subscription);
          const currSupplier = subscription.suppliers?.find(
            (supplier) => supplier.supplierID === Number(supplierId)
          );
          setCurrentSupplier(currSupplier || null);
          setValue('subscription', subscription.subscriptionId);
        }
      });
    }
  }, [subscriptions, fetchData, user]);

  useEffect(() => {
    if (!!currentSupplier) {
      setValue('supplier', currentSupplier.supplierID);
    }
  }, [currentSupplier, subscriptions, setValue]);

  return (
    <>
      <MainLayout>
        <>
          {/* Page header */}
          <div className='mb-8'>
            {/* Title */}
            <h1 className='text-2xl md:text-3xl text-slate-800 font-bold'>
              Edit Supplier {currentSupplier?.supplierName}
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
                          ({ subscriptionId: id, userName }) => ({
                            id,
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
                    {currentSubscription && (
                      <Select
                        id='supplier'
                        {...register('supplier', {
                          required: 'This field is required.',
                        })}
                        labelText='Supplier'
                        options={
                          currentSubscription.suppliers?.map((loginOption) => {
                            return {
                              id: loginOption.supplierID,
                              name: loginOption.supplierName,
                            };
                          }) || []
                        }
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
