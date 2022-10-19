import React, { useState, useEffect, useMemo, useCallback } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import { useDispatch, useSelector } from 'react-redux';
import { useForm, SubmitHandler } from 'react-hook-form';
import { AiOutlineCloseCircle } from 'react-icons/ai';
//
import {
  deleteSupplier,
  testSupplier,
  updateSupplier,
} from '../data-access/supplier';
import { getAccountWithSupplierAndConfigurations } from '../data-access/account';
import { RootState } from '@/store';
import { renderConfigurationFormFields } from '@/utils/render-configuration-form-fields';
import type { Supplier, SupplierFormFields, Account } from '@/types';
import { ButtonColors, ButtonVariants, NotificationStatus } from '@/constants';
import MainLayout from '@/layouts/Main';
import { SectionTitle, Button, ConfirmModal, Modal } from '@/components';

type Props = {};

const MESSAGES = {
  onSuccess: {
    update: 'Supplier updated successfully',
    delete: 'Supplier deleted successfully',
  },
  onFailed: {
    update: 'Failed to update supplier',
    delete: 'Failed to delete supplier',
  },
};

const SupplierEdit: React.FC<Props> = () => {
  const { pathname, search } = useLocation();

  const accountId = search.split('=')[1];
  const supplierId = pathname.split('/')[2];
  const navigate = useNavigate();
  const dispatch = useDispatch();

  const user = useSelector((state: RootState) => state.app.user);
  const userKey = useSelector(
    (state: RootState) => state.app.awsAmplify.username
  );

  const [currentAccount, setCurrentAccount] = useState<Account | null>(null);
  const [currentSupplier, setCurrentSupplier] = useState<Supplier | null>(null);
  const [originalSupplier, setOriginalSupplier] = useState<Supplier | null>(
    null
  );
  const [isDeleting, setIsDeleting] = useState<boolean>(false);
  const [testDetails, setTestDetails] = useState({
    name: '',
    isTesting: false,
    status: '',
  });

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

  const onSubmit: SubmitHandler<SupplierFormFields> = useCallback(
    (data, event) => {
      event?.preventDefault();
      if (!activeTenant) return;
      updateSupplier({
        tenant: { id: activeTenant.tenantId, key: activeTenant.tenantKey },
        userKey: userKey as string,
        accountId: Number(currentAccount?.accountId),
        supplierId: Number(currentSupplier?.supplierID),
        data,
        onInit: () => {
          dispatch.app.setIsLoading(true);
        },
        onSuccess: () => {
          dispatch.app.setIsLoading(false);
          dispatch.app.setNotification({
            status: NotificationStatus.SUCCESS,
            message: MESSAGES.onSuccess.update,
          });

          setTimeout(() => {
            navigate('/suppliers');
          }, 500);
        },
        onFailed: (err, instance) => {
          dispatch.app.setIsLoading(false);
          dispatch.app.setNotification({
            status: NotificationStatus.ERROR,
            message: err,
            instance,
          });
        },
      });
    },
    [activeTenant, currentAccount, currentSupplier, userKey]
  );

  const handleDeleteSupplier = useCallback(async () => {
    if (!activeTenant) return;
    await deleteSupplier(
      { id: activeTenant?.tenantId, key: activeTenant?.tenantKey },
      userKey as string,
      Number(currentAccount?.accountId),
      Number(currentSupplier?.supplierID),
      () => {
        dispatch.app.setIsLoading(true);
      },
      () => {
        dispatch.app.setIsLoading(false);
        setIsDeleting(false);
        dispatch.app.setNotification({
          status: NotificationStatus.SUCCESS,
          message: MESSAGES.onSuccess.delete,
        });
        setTimeout(() => {
          navigate('/suppliers');
        }, 500);
      },
      (err, instance) => {
        dispatch.app.setIsLoading(false);
        dispatch.app.setNotification({
          status: NotificationStatus.ERROR,
          message: err,
          instance,
        });
      }
    );
  }, [activeTenant, currentAccount, currentSupplier, userKey]);

  const handleTesting = useCallback(async () => {
    const originalConfigurationsValues = originalSupplier?.configurations
      ?.map((c) => c.value)
      .join('');
    const currentConfigurationsValues = getValues('configurations').join('');

    if (originalConfigurationsValues !== currentConfigurationsValues) {
      dispatch.app.setNotification({
        status: NotificationStatus.ERROR,
        message: 'Please save the configurations before testing',
      });
      return;
    }
    if (!activeTenant || !userKey) return;

    setTestDetails({
      name: currentSupplier?.name as string,
      isTesting: true,
      status: 'Running test...',
    });
    await testSupplier(
      activeTenant?.tenantKey,
      userKey,
      activeTenant.tenantId,
      currentAccount?.accountId as number,
      currentSupplier?.supplierID as number,
      (status) => {
        setTestDetails({
          name: currentSupplier?.name as string,
          isTesting: true,
          status: status,
        });
      },
      (err, instance) => {
        setTestDetails({
          name: currentSupplier?.name as string,
          isTesting: true,
          status: err,
        });
        dispatch.app.setNotification({
          status: NotificationStatus.ERROR,
          message: err,
          instance,
        });
      }
    );
  }, [originalSupplier, currentSupplier]);

  const fetchData = async () => {
    if (!activeTenant) return;
    await getAccountWithSupplierAndConfigurations(
      { id: activeTenant.tenantId, key: activeTenant.tenantKey },
      userKey as string,
      Number(accountId),
      Number(supplierId),
      () => {
        dispatch.app.setIsLoading(true);
      },
      (account, configurations, supplier) => {
        setCurrentAccount(account);
        setCurrentSupplier({ ...supplier, configurations });
        setOriginalSupplier({ ...supplier, configurations });
        dispatch.app.setIsLoading(false);
      },
      (err, instance) => {
        dispatch.app.setIsLoading(false);
        dispatch.app.setNotification({
          status: NotificationStatus.ERROR,
          message: err,
          instance,
        });
        navigate('/suppliers');
      }
    );
  };

  useEffect(() => {
    if (!!user) {
      fetchData();
    }
    return () => {
      setCurrentAccount(null);
      setCurrentSupplier(null);
      setOriginalSupplier(null);
      dispatch.app.resetNotification();
    };
  }, [user]);

  useEffect(() => {
    if (!!currentSupplier && !!currentAccount) {
      setValue('account', currentAccount.accountId);
      setValue('supplier', currentSupplier.supplierID as number);
    }
    return () => {
      setValue('account', 0);
      setValue('supplier', 0);
    };
  }, [currentSupplier, currentAccount, setValue]);

  return (
    <>
      <MainLayout
        title={`${currentAccount?.userName || ''} ${
          currentSupplier?.name || ''
        }`}
      >
        <div className='bg-white shadow-lg rounded-sm mb-8'>
          <div className='flex flex-col md:flex-row md:-mr-px'>
            <div className='min-w-60'></div>
            <form
              className='grow p-6 w-full divide-y divide-gray-200'
              onSubmit={handleSubmit(onSubmit)}
            >
              <div className='mb-8 flex flex-col gap-5 md:w-1/2'>
                <SectionTitle title='Settings' />
                <div className='flex flex-col gap-5'>
                  {!!currentSupplier?.configurations?.length &&
                    renderConfigurationFormFields(
                      currentSupplier.configurations || [],
                      register,
                      errors
                    )}
                </div>
              </div>
              <div className='flex justify-end mt-5 pt-5'>
                <Button
                  text='Test'
                  color={ButtonColors.OUTLINE}
                  className='ml-4'
                  onClick={handleTesting}
                />
                <Button
                  text='Cancel'
                  color={ButtonColors.OUTLINE}
                  className='ml-4'
                  onClick={() => navigate(-1)}
                />
                <Button
                  text='Delete'
                  color={ButtonColors.DANGER}
                  className='ml-4'
                  onClick={() => setIsDeleting(true)}
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

      {isDeleting && (
        <ConfirmModal
          title='Delete Supplier'
          description={
            <>
              Are you sure you want to delete{' '}
              <strong>{currentSupplier?.name}</strong>?
            </>
          }
          show={isDeleting}
          setShow={setIsDeleting}
          onConfirm={handleDeleteSupplier}
        />
      )}

      {testDetails.isTesting && (
        <Modal transparent flex>
          <div className='bg-white rounded shadow-lg overflow-auto max-w-lg w-full max-h-full'>
            <div className='px-5 py-3 border-b border-slate-200'>
              <div className='flex justify-between items-center'>
                <div className='font-semibold text-slate-800'>
                  {testDetails.name}
                </div>
              </div>
            </div>
            <p className='p-5'>{testDetails.status}</p>
            <div className='flex justify-end px-5 pb-5'>
              <Button
                text='Close'
                onClick={() => {
                  setTestDetails({
                    name: '',
                    isTesting: false,
                    status: '',
                  });
                }}
                color={ButtonColors.PRIMARY}
              />
            </div>
          </div>
        </Modal>
      )}
    </>
  );
};

export default React.memo(SupplierEdit);
