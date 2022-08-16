import { memo, useEffect, useState, FC, useCallback, useMemo } from 'react';
import { useNavigate } from 'react-router-dom';
import { useForm, SubmitHandler } from 'react-hook-form';
import axios from 'axios';
import { useDispatch, useSelector } from 'react-redux';
//
import { Account } from '@/types';
import { useSlug } from '@/utils/use-slug';
import {
  NotificationStatus,
  InputTypes,
  ButtonColors,
  ButtonVariants,
} from '@/constants';
import MainLayout from '@/layouts/Main';
import {
  SectionTitle,
  Toggle,
  Notification,
  Select,
  Button,
  TextField,
  Spinner,
  ConfirmModal,
} from '@/components';
import { RootState } from '@/store';
import {
  deleteAccount,
  getAccountById,
  updateAccount,
} from '../data-access/account';
import { log } from 'console';

type NotificationState = {
  status: NotificationStatus;
  message: string;
};

type Props = {};

interface AccountFields extends Account {
  confirmPassword: string;
}

const MESSAGES = {
  onSuccess: {
    update: 'Account updated successfully',
    delete: 'Account deleted successfully',
  },
  onFailed: {
    update: 'Failed to update account',
    delete: 'Failed to delete account',
  },
};

export const AccountEdit: FC<Props> = memo(() => {
  const dispatch = useDispatch();
  const user = useSelector((state: RootState) => state.app.user);
  const appError = useSelector((state: RootState) => state.app.error);
  const navigate = useNavigate();
  const { slug } = useSlug();

  const [currentAccount, setCurrentAccount] = useState(null as Account | null);
  const [notification, setNotification] = useState<NotificationState>();
  const [showNotification, setShowNotification] = useState<boolean>(false);
  const [isDeleting, setIsDeleting] = useState<boolean>(false);

  const activeTenant = useMemo(
    () => user?.tenants.find((tenant: any) => tenant.isSelected),
    [user?.tenants]
  );
  const userIsValid = useMemo(
    () => !!activeTenant && !!currentAccount,
    [activeTenant, currentAccount]
  );

  const {
    register,
    handleSubmit,
    setValue,
    formState: { errors },
  } = useForm<AccountFields>();

  const fetchAccountById = useCallback(async () => {
    if (!activeTenant) return;
    await getAccountById(
      { id: activeTenant.tenantId, key: activeTenant.tenantKey },
      Number(slug),
      () => {
        dispatch.app.setIsLoading(true);
      },
      (account) => {
        setCurrentAccount(account);
        dispatch.app.setIsLoading(false);
      },
      (err) => {
        dispatch.app.setError(err);
        dispatch.app.setIsLoading(false);
      }
    );
  }, [activeTenant, slug]);

  const onSubmit: SubmitHandler<AccountFields> = async (data) => {
    if (userIsValid) return;
    await updateAccount(
      {
        id: activeTenant?.tenantId as number,
        key: activeTenant?.tenantKey as string,
      },
      Number(slug),
      {
        UserName: data.userName,
        Password: data.password,
        PropertyTpRequestLimit: data.propertyTprequestLimit.toString(),
        SearchTimeoutSeconds: data.searchTimeoutSeconds.toString(),
        CurrencyCode: data.currencyCode,
      },
      () => {
        dispatch.app.setIsLoading(true);
      },
      () => {
        dispatch.app.setIsLoading(false);
        setNotification({
          status: NotificationStatus.SUCCESS,
          message: MESSAGES.onSuccess.update,
        });
        setShowNotification(true);
        setTimeout(() => {
          navigate('/accounts');
        }, 500);
      },
      () => {
        dispatch.app.setIsLoading(false);
        setNotification({
          status: NotificationStatus.ERROR,
          message: MESSAGES.onFailed.update,
        });
        setShowNotification(true);
      }
    );
  };

  const handleDeleteAccount = useCallback(async () => {
    if (!userIsValid) return;
    await deleteAccount(
      {
        id: activeTenant?.tenantId as number,
        key: activeTenant?.tenantKey as string,
      },
      currentAccount?.subscriptionId as number,
      () => {
        dispatch.app.setIsLoading(true);
      },
      () => {
        dispatch.app.setIsLoading(false);
        setNotification({
          status: NotificationStatus.SUCCESS,
          message: MESSAGES.onSuccess.delete,
        });
        setShowNotification(true);
        setIsDeleting(false);
        setTimeout(() => {
          navigate('/accounts');
        }, 500);
      },
      () => {
        dispatch.app.setIsLoading(false);
        setNotification({
          status: NotificationStatus.ERROR,
          message: MESSAGES.onFailed.delete,
        });
        setShowNotification(true);
      }
    );
  }, [activeTenant, currentAccount]);

  useEffect(() => {
    if (appError) {
      setNotification({
        status: NotificationStatus.ERROR,
        message: appError as string,
      });
      setShowNotification(true);
    }
  }, [appError]);

  useEffect(() => {
    if (!currentAccount) return;
    setValue('userName', currentAccount?.userName);
    setValue('password', currentAccount?.password);
    setValue('propertyTprequestLimit', currentAccount?.propertyTprequestLimit);
    setValue('searchTimeoutSeconds', currentAccount?.searchTimeoutSeconds);
    setValue('logMainSearchError', currentAccount?.logMainSearchError);
    setValue('currencyCode', currentAccount?.currencyCode);
  }, [currentAccount]);

  useEffect(() => {
    fetchAccountById();
  }, [fetchAccountById]);

  return (
    <>
      <MainLayout title='Edit Account'>
        <div className='bg-white shadow-lg rounded-sm mb-8'>
          <div className='flex flex-col md:flex-row md:-mr-px'>
            <div className='min-w-60'></div>
            <form
              className='grow p-6 space-y-6 w-full divide-y divide-gray-200'
              onSubmit={handleSubmit(onSubmit)}
              autoComplete='turnedOff'
            >
              <div className='flex flex-col gap-5 mb-8'>
                <div className='flex-1'>
                  <SectionTitle title='Account' />
                </div>
                <div className='flex-1 md:w-1/2'>
                  <TextField
                    id='userName'
                    {...register('userName', {
                      required: 'This field is required.',
                    })}
                    labelText='Username'
                    isDirty={!!errors.userName}
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
                    isDirty={!!errors.password}
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
                    labelText='Maximum Single Request Property Search Limit'
                    isDirty={!!errors.propertyTprequestLimit}
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
                    labelText='Search Timeout (seconds)'
                    isDirty={!!errors.searchTimeoutSeconds}
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
                    isDirty={!!errors.logMainSearchError}
                    errorMsg={errors.logMainSearchError?.message}
                    defaultValue={currentAccount?.logMainSearchError as boolean}
                    readOnly
                  />
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

      {showNotification && (
        <Notification
          description={notification?.message as string}
          show={showNotification}
          setShow={setShowNotification}
          status={notification?.status}
        />
      )}

      {isDeleting && (
        <ConfirmModal
          title='Delete Account'
          description={
            <>
              Are you sure you want to delete{' '}
              <strong>{currentAccount?.userName}</strong>?
            </>
          }
          show={isDeleting}
          setShow={setIsDeleting}
          onConfirm={handleDeleteAccount}
        />
      )}
    </>
  );
});
