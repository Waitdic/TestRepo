import { memo, FC, useState, useEffect, useMemo, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { useDispatch, useSelector } from 'react-redux';
import { useForm, SubmitHandler } from 'react-hook-form';
import axios from 'axios';
//
import { RootState } from '@/store';
import { renderConfigurationFormFields } from '@/utils/render-configuration-form-fields';
import {
  SelectOption,
  Subscription,
  Supplier,
  SupplierConfiguration,
  SupplierFormFields,
} from '@/types';
import { ButtonColors, ButtonVariants, NotificationStatus } from '@/constants';
import MainLayout from '@/layouts/Main';
import {
  SectionTitle,
  Select,
  Button,
  Notification,
  ErrorBoundary,
} from '@/components';
import {
  createSupplier,
  getConfigurationsBySupplier,
  getSubscriptions,
  getSubscriptionsWithSuppliersAndConfigurations,
  getSuppliersBySubscription,
} from '../data-access';
import { sortBy, uniqBy } from 'lodash';

type Props = {};

export const SupplierCreate: FC<Props> = memo(() => {
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const user = useSelector((state: RootState) => state.app.user);
  const subscriptions = useSelector(
    (state: RootState) => state.app.subscriptions
  );

  const {
    register,
    handleSubmit,
    setValue,
    getValues,
    formState: { errors },
  } = useForm<SupplierFormFields>();

  const activeTenant = useMemo(
    () => user?.tenants?.find((tenant) => tenant.isSelected),
    [user]
  );

  const [suppliers, setSuppliers] = useState<Supplier[]>([]);
  const [draftSupplier, setDraftSupplier] = useState<{
    subscriptionId: number;
    supplierId: number;
    configurations: SupplierConfiguration[];
  }>({
    subscriptionId: -1,
    supplierId: -1,
    configurations: [],
  });
  const [showNotification, setShowNotification] = useState(false);
  const [notification, setNotification] = useState({
    status: NotificationStatus.SUCCESS,
    message: 'New Supplier created successfully.',
  });

  const sortedSubscriptions = useMemo(
    () => sortBy(subscriptions, 'userName'),
    [subscriptions]
  );
  const sortedSuppliers = useMemo(
    () => sortBy(suppliers, 'supplierName'),
    [suppliers]
  );

  const onSubmit: SubmitHandler<SupplierFormFields> = async (data) => {
    if (!activeTenant) return;
    await createSupplier(
      {
        id: activeTenant.tenantId,
        key: activeTenant.tenantKey,
      },
      draftSupplier.subscriptionId,
      draftSupplier.supplierId,
      data,
      () => {
        dispatch.app.setIsLoading(true);
      },
      (newSupplier) => {
        console.log(newSupplier);
        dispatch.app.setIsLoading(false);
        setShowNotification(true);
      },
      (err) => {
        dispatch.app.setError(err);
        dispatch.app.setIsLoading(true);
      }
    );
  };

  const handleSubscriptionChange = (optionId: number) => {
    const selectedSub = subscriptions.find(
      (subscription) => subscription.subscriptionId === optionId
    );
    if (selectedSub) {
      const supplierIds = selectedSub.suppliers.map(
        (supplier) => supplier.supplierID
      );
      const allSuppliers = uniqBy(
        subscriptions.flatMap((sub) => sub.suppliers),
        'supplierID'
      );
      const _suppliers = allSuppliers.filter(
        (supplier) => !supplierIds.includes(supplier.supplierID)
      );
      setSuppliers(_suppliers);
    }
    setDraftSupplier({
      ...draftSupplier,
      subscriptionId: optionId,
    });
  };

  const handleSupplierChange = (optionId: number) => {
    setDraftSupplier({
      ...draftSupplier,
      supplierId: optionId,
      configurations:
        suppliers.find((supplier) => supplier.supplierID === optionId)
          ?.configurations || [],
    });
  };

  const fetchData = useCallback(async () => {
    if (!activeTenant) return;
    await getSubscriptionsWithSuppliersAndConfigurations(
      { id: activeTenant.tenantId, key: activeTenant.tenantKey },
      () => {
        dispatch.app.setIsLoading(true);
      },
      (subs) => {
        dispatch.app.updateSubscriptions(subs);
        dispatch.app.setIsLoading(false);
      },
      (err) => {
        dispatch.app.setError(err);
        dispatch.app.setIsLoading(false);
        setNotification({
          status: NotificationStatus.ERROR,
          message: 'Subscriptions and suppliers data fetching failed.',
        });
        setShowNotification(true);
      }
    );
  }, [activeTenant]);

  useEffect(() => {
    fetchData();
    setValue('subscription', 0);
    setValue('supplier', 0);

    return () => {
      setShowNotification(false);
      setValue('subscription', 0);
      setValue('supplier', 0);
      setSuppliers([]);
      setDraftSupplier({
        subscriptionId: -1,
        supplierId: -1,
        configurations: [],
      });
      dispatch.app.setError(null);
    };
  }, [fetchData]);

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
                  <Select
                    id='subscription'
                    {...register('subscription', {
                      required: 'This field is required.',
                    })}
                    labelText='Subscription'
                    options={sortedSubscriptions?.map(
                      ({ subscriptionId, userName }) => ({
                        id: subscriptionId,
                        name: userName,
                      })
                    )}
                    isFirstOptionEmpty
                    onUncontrolledChange={handleSubscriptionChange}
                  />
                </div>
                <div className='flex-1'>
                  <Select
                    id='supplier'
                    {...register('supplier', {
                      required: 'This field is required.',
                    })}
                    labelText='Supplier'
                    options={
                      sortedSuppliers?.map((loginOption) => ({
                        id: loginOption.supplierID,
                        name: loginOption?.supplierName,
                      })) || []
                    }
                    isFirstOptionEmpty
                    onUncontrolledChange={handleSupplierChange}
                  />
                </div>
                {!!draftSupplier?.configurations?.length && (
                  <div className='border-t border-gray-200 mt-2 pt-5'>
                    <SectionTitle title='Settings' />
                    <div className='flex flex-col gap-5 mt-5'>
                      {renderConfigurationFormFields(
                        draftSupplier.configurations,
                        register,
                        errors
                      )}
                    </div>
                  </div>
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
        </div>
      </MainLayout>

      {showNotification && (
        <Notification
          title='Supplier creation'
          description={notification.message}
          status={notification.status}
          show={showNotification}
          setShow={setShowNotification}
        />
      )}
    </>
  );
});
