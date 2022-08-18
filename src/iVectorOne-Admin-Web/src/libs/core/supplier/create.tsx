import React, { useState, useEffect, useMemo, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { useDispatch, useSelector } from 'react-redux';
import { useForm, SubmitHandler } from 'react-hook-form';
import { sortBy } from 'lodash';
//
import { RootState } from '@/store';
import { renderConfigurationFormFields } from '@/utils/render-configuration-form-fields';
import { Supplier, SupplierConfiguration, SupplierFormFields } from '@/types';
import { ButtonColors, ButtonVariants, NotificationStatus } from '@/constants';
import MainLayout from '@/layouts/Main';
import {
  SectionTitle,
  Select,
  Button,
  Notification,
  Spinner,
} from '@/components';
import {
  createSupplier,
  getConfigurationsBySupplier,
  getSuppliers,
} from '../data-access/supplier';
import { getAccountsWithSuppliers } from '../data-access/account';

type Props = {};

const SupplierCreate: React.FC<Props> = () => {
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const user = useSelector((state: RootState) => state.app.user);
  const accounts = useSelector((state: RootState) => state.app.accounts);
  const isLoading = useSelector((state: RootState) => state.app.isLoading);

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

  const sortedAccounts = useMemo(
    () =>
      sortBy(accounts, [
        function (o) {
          return o.userName?.toLowerCase?.();
        },
      ]),
    [accounts]
  );

  const sortedSuppliers = useMemo(() => {
    if (draftSupplier.subscriptionId === -1) {
      return [];
    } else {
      return sortBy(suppliers, [
        function (o) {
          return o.name?.toLowerCase?.();
        },
      ]);
    }
  }, [suppliers, draftSupplier]);

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
      (_newSupplier) => {
        dispatch.app.setIsLoading(false);
        setShowNotification(true);
        setTimeout(() => {
          navigate('/suppliers');
        }, 800);
      },
      (err) => {
        dispatch.app.setError(err);
        dispatch.app.setIsLoading(true);
      }
    );
  };

  const handleAccountChange = (optionId: number) => {
    const selectedSub = accounts.find(
      (account) => account.subscriptionId === optionId
    );
    if (selectedSub) {
      const supplierIds = selectedSub?.suppliers?.map(
        (supplier) => supplier.supplierID
      );
      const selectableSuppliers = suppliers.filter(
        (supplier) => !supplierIds.includes(supplier.supplierID)
      );
      setSuppliers(selectableSuppliers);
      setDraftSupplier({
        ...draftSupplier,
        subscriptionId: optionId,
      });
    } else {
      setValue('account', 0);
      setValue('supplier', 0);
      setDraftSupplier({
        ...draftSupplier,
        subscriptionId: -1,
        supplierId: -1,
        configurations: [],
      });
    }
  };

  const handleSupplierChange = async (optionId: number) => {
    if (!activeTenant) return;
    await getConfigurationsBySupplier(
      { id: activeTenant.tenantId, key: activeTenant.tenantKey },
      optionId,
      () => {
        dispatch.app.setIsLoading(true);
      },
      (configurations) => {
        setDraftSupplier({
          ...draftSupplier,
          supplierId: optionId,
          configurations,
        });
        dispatch.app.setIsLoading(false);
      },
      (err) => {
        handleError(err);
      }
    );
  };

  const handleError = (err: string | null) => {
    console.error(err);
    dispatch.app.setError(err);
    dispatch.app.setIsLoading(false);
    setNotification({
      status: NotificationStatus.ERROR,
      message: 'Data fetching failed.',
    });
    setShowNotification(true);
  };

  const fetchAccountsWithSuppliers = useCallback(async () => {
    if (!activeTenant) return;
    await Promise.all([
      getAccountsWithSuppliers(
        { id: activeTenant.tenantId, key: activeTenant.tenantKey },
        () => {
          dispatch.app.setIsLoading(true);
        },
        (accs) => {
          dispatch.app.updateAccounts(accs);
          dispatch.app.setIsLoading(false);
        },
        (err) => {
          handleError(err);
        }
      ),
      getSuppliers(
        { id: activeTenant.tenantId, key: activeTenant.tenantKey },
        () => {
          dispatch.app.setIsLoading(true);
        },
        (supps) => {
          setSuppliers(supps);
          dispatch.app.setIsLoading(false);
        },
        (err) => {
          handleError(err);
        }
      ),
    ]);
  }, [activeTenant]);

  useEffect(() => {
    fetchAccountsWithSuppliers();
    setValue('account', 0);
    setValue('supplier', 0);

    return () => {
      setShowNotification(false);
      setValue('account', 0);
      setValue('supplier', 0);
      setSuppliers([]);
      setDraftSupplier({
        subscriptionId: -1,
        supplierId: -1,
        configurations: [],
      });
      dispatch.app.setError(null);
    };
  }, [fetchAccountsWithSuppliers]);

  return (
    <>
      <MainLayout title='Create Supplier'>
        <div className='bg-white shadow-lg rounded-sm mb-8'>
          <div className='flex flex-col md:flex-row md:-mr-px'>
            <div className='min-w-60'></div>
            <form
              className='grow p-6 w-full divide-y divide-gray-200'
              onSubmit={handleSubmit(onSubmit)}
            >
              <div className='mb-8 flex flex-col gap-5 md:w-1/2'>
                <div className='flex-1'>
                  <Select
                    id='account'
                    {...register('account', {
                      required: 'This field is required.',
                    })}
                    labelText='Account'
                    options={sortedAccounts?.map(
                      ({ subscriptionId, userName }) => ({
                        id: subscriptionId,
                        name: userName,
                      })
                    )}
                    isFirstOptionEmpty
                    onUncontrolledChange={handleAccountChange}
                  />
                </div>
                <div className='flex-1'>
                  <Select
                    id='supplier'
                    {...register('supplier', {
                      required: 'This field is required.',
                    })}
                    labelText='Supplier'
                    options={sortedSuppliers?.map((loginOption) => ({
                      id: loginOption.supplierID || 0,
                      name: loginOption?.name || '',
                    }))}
                    isFirstOptionEmpty
                    onUncontrolledChange={handleSupplierChange}
                  />
                </div>
                {!!draftSupplier?.configurations?.length && !isLoading ? (
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
                ) : (
                  isLoading && (
                    <div className='relative w-8 h-8'>
                      <Spinner />
                    </div>
                  )
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

      <Notification
        description={notification.message}
        status={notification.status}
        show={showNotification}
        setShow={setShowNotification}
      />
    </>
  );
};

export default React.memo(SupplierCreate);
