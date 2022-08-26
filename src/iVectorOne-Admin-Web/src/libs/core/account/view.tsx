import React, { useEffect, useState, useCallback, useMemo } from 'react';
import { useNavigate } from 'react-router-dom';
import { useDispatch, useSelector } from 'react-redux';
//
import { Account } from '@/types';
import { useSlug } from '@/utils/use-slug';
import { ButtonColors, NotificationStatus } from '@/constants';
import MainLayout from '@/layouts/Main';
import {
  SectionTitle,
  Button,
  Spinner,
  YesOrNo,
  Notification,
  CopyField,
} from '@/components';
import { RootState } from '@/store';
import { getAccountById } from '../data-access/account';

type Props = {};

const AccountView: React.FC<Props> = () => {
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const { slug } = useSlug();

  const error = useSelector((state: RootState) => state.app.error);
  const accounts = useSelector((state: RootState) => state.app.accounts);
  const user = useSelector((state: RootState) => state.app.user);
  const userKey = useSelector(
    (state: RootState) => state.app.awsAmplify.username
  );

  const activeTenant = useMemo(
    () => user?.tenants.find((tenant) => tenant.isSelected),
    [user]
  );

  const [showNotification, setShowNotification] = useState(false);
  const [currentAccount, setCurrentAccount] = useState(null as Account | null);

  const loadAccount = useCallback(() => {
    if (accounts.length > 0) {
      const currAccount = accounts.find(
        (acc) => acc.accountId === Number(slug)
      );

      if (!currAccount) {
        navigate('/accounts');
      } else {
        setCurrentAccount(currAccount);
      }
    }
  }, [accounts, navigate, slug]);

  const fetchAccountById = useCallback(async () => {
    if (!activeTenant || activeTenant == null) return;
    await getAccountById(
      { id: activeTenant.tenantId, key: activeTenant.tenantKey },
      userKey as string,
      Number(slug),
      () => {
        dispatch.app.setIsLoading(true);
      },
      (acc) => {
        setCurrentAccount(acc);
        dispatch.app.setIsLoading(false);
      },
      (err) => {
        dispatch.app.setError(err);
        dispatch.app.setIsLoading(false);
      }
    );
  }, [activeTenant, slug]);

  useEffect(() => {
    if (!!accounts?.length) {
      loadAccount();
    } else {
      fetchAccountById();
    }
  }, [accounts, loadAccount, fetchAccountById]);

  return (
    <>
      <MainLayout title={`${currentAccount?.userName || ''}`}>
        <div className='bg-white shadow-lg rounded-sm mb-8'>
          <div className='flex flex-col md:flex-row md:-mr-px'>
            <div className='min-w-60'></div>
            <div className='grow p-6 space-y-6 w-full divide-y divide-gray-200'>
              <div className='flex flex-col gap-5 mb-8'>
                <div className='flex-1'>
                  <SectionTitle title='Login Credentials' />
                </div>
                {!!currentAccount ? (
                  <>
                    <div className='flex-1 md:w-1/2'>
                      <h4 className='block text-sm font-medium mb-1'>
                        Username
                      </h4>
                      <CopyField value={currentAccount.userName} />
                    </div>
                    <div className='flex-1 md:w-1/2 border-b border-gray-200 pb-5'>
                      <h4 className='block text-sm font-medium mb-1'>
                        Password
                      </h4>
                      <CopyField value={currentAccount.password} />
                    </div>
                    <div className='flex-1'>
                      <SectionTitle title='Settings' />
                    </div>
                    <div className='flex-1 md:w-1/2'>
                      <h4 className='block text-sm font-medium mb-1'>
                        Maximum Single Request Property Search Limit
                      </h4>
                      <p className='text-sm'>
                        {currentAccount.propertyTprequestLimit}
                      </p>
                    </div>
                    <div className='flex-1 md:w-1/2'>
                      <h4 className='block text-sm font-medium mb-1'>
                        Search Timeout (seconds)
                      </h4>
                      <p className='text-sm'>
                        {currentAccount.searchTimeoutSeconds}
                      </p>
                    </div>
                    <div className='flex-1 md:w-1/2'>
                      <h4 className='block text-sm font-medium mb-1'>
                        Currency Code
                      </h4>
                      <p className='text-sm'>{currentAccount.currencyCode}</p>
                    </div>
                  </>
                ) : (
                  <Spinner />
                )}
              </div>
              <div className='flex justify-end mt-5 pt-5'>
                <Button
                  text='Close'
                  color={ButtonColors.OUTLINE}
                  className='ml-4'
                  onClick={() => navigate(-1)}
                />
              </div>
            </div>
          </div>
        </div>
      </MainLayout>

      {showNotification && (
        <Notification
          description={error as string}
          show={showNotification}
          setShow={setShowNotification}
          status={NotificationStatus.ERROR}
        />
      )}
    </>
  );
};

export default React.memo(AccountView);
