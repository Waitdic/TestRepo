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
} from '@/components';
import { RootState } from '@/store';
import { getAccountById } from '../data-access';

type NotificationState = {
  status: NotificationStatus;
  message: string;
};

type Props = {};

interface AccountFields extends Account {
  confirmPassword: string;
}

export const AccountEdit: FC<Props> = memo(() => {
  const dispatch = useDispatch();
  const user = useSelector((state: RootState) => state.app.user);
  const error = useSelector((state: RootState) => state.app.error);
  const navigate = useNavigate();
  const { slug } = useSlug();

  const accounts = useSelector((state: RootState) => state.app.accounts);

  const [currentAccount, setCurrentAccount] = useState(null as Account | null);
  const [notification, setNotification] = useState<NotificationState>({
    status: NotificationStatus.SUCCESS,
    message: 'Account edited successfully.',
  });
  const [showNotification, setShowNotification] = useState<boolean>(false);

  const activeTenant = useMemo(
    () => user?.tenants.find((tenant: any) => tenant.isSelected),
    [user?.tenants]
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

  //! Temporary placeholder onSubmit function for account edit
  const onSubmit: SubmitHandler<AccountFields> = async (data) => {
    try {
      const _updatedAccount = await axios.patch(
        `http://localhost:3001/account/edit/${slug}`,
        data
      );
      setNotification({
        status: NotificationStatus.SUCCESS,
        message: 'Account edited successfully.',
      });
      setShowNotification(true);
    } catch (err) {
      console.error(err);
      setShowNotification(true);
    }
  };

  const loadAccount = useCallback(() => {
    if (!!accounts?.length) {
      const selectedAccount = accounts.find(
        (acc) => acc.subscriptionId === Number(slug)
      );
      if (!selectedAccount) {
        navigate('/accounts');
        return;
      }
      setCurrentAccount(selectedAccount);
      setValue('userName', selectedAccount.userName);
      setValue(
        'propertyTprequestLimit',
        selectedAccount.propertyTprequestLimit
      );
      setValue('searchTimeoutSeconds', selectedAccount.searchTimeoutSeconds);
      setValue('logMainSearchError', selectedAccount.logMainSearchError);
      setValue('currencyCode', selectedAccount.currencyCode);
    }
  }, [accounts, navigate, setValue, slug]);

  useEffect(() => {
    if (error) {
      setNotification({
        status: NotificationStatus.ERROR,
        message: error as string,
      });
      setShowNotification(true);
    }
  }, [error]);

  useEffect(() => {
    if (currentAccount !== null) {
      setValue('userName', currentAccount.userName);
      setValue('password', currentAccount.password);
      setValue('propertyTprequestLimit', currentAccount.propertyTprequestLimit);
      setValue('searchTimeoutSeconds', currentAccount.searchTimeoutSeconds);
      setValue('logMainSearchError', currentAccount.logMainSearchError);
      setValue('currencyCode', currentAccount.currencyCode);
    }
  }, [currentAccount]);

  useEffect(() => {
    loadAccount();
  }, [accounts, loadAccount]);

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
                {accounts.length > 0 ? (
                  <>
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
                        labelText='Search Timeout Seconds'
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
                        defaultValue={
                          currentAccount?.logMainSearchError as boolean
                        }
                      />
                    </div>
                  </>
                ) : (
                  <Spinner />
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
          title='Edit Subscription'
          description={notification.message}
          show={showNotification}
          setShow={setShowNotification}
          status={notification.status}
          autoHide={false}
        />
      )}
    </>
  );
});