import { memo, useEffect, useState, FC, useCallback, useMemo } from 'react';
import { useNavigate } from 'react-router-dom';
import { useDispatch, useSelector } from 'react-redux';
//
import { Subscription } from '@/types';
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
import { getSubscriptionById } from '../data-access';

type Props = {};

export const SubscriptionView: FC<Props> = memo(() => {
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const { slug } = useSlug();

  const error = useSelector((state: RootState) => state.app.error);
  const subscriptions = useSelector(
    (state: RootState) => state.app.subscriptions
  );
  const user = useSelector((state: RootState) => state.app.user);

  const activeTenant = useMemo(
    () => user?.tenants.find((tenant) => tenant.isSelected),
    [user]
  );

  const [showNotification, setShowNotification] = useState(false);
  const [currentSubscription, setCurrentSubscription] = useState(
    null as Subscription | null
  );

  const loadSubscription = useCallback(() => {
    if (subscriptions.length > 0) {
      const currSubscription = subscriptions.find(
        (sub) => sub.subscriptionId === Number(slug)
      );

      if (!currSubscription) {
        navigate('/subscriptions');
      } else {
        setCurrentSubscription(currSubscription);
      }
    }
  }, [subscriptions, navigate, slug]);

  const fetchSubscriptionById = useCallback(async () => {
    if (!activeTenant || activeTenant == null) return;
    await getSubscriptionById(
      { id: activeTenant.tenantId, key: activeTenant.tenantKey },
      Number(slug),
      () => {
        dispatch.app.setIsLoading(true);
      },
      (subscription) => {
        setCurrentSubscription(subscription);
        dispatch.app.setIsLoading(false);
      },
      (err) => {
        dispatch.app.setError(err);
        dispatch.app.setIsLoading(false);
      }
    );
  }, [activeTenant, slug]);

  useEffect(() => {
    if (!!subscriptions?.length) {
      loadSubscription();
    } else {
      fetchSubscriptionById();
    }
  }, [subscriptions, loadSubscription, fetchSubscriptionById]);

  return (
    <>
      <MainLayout title={`View Subscription ${currentSubscription?.userName}`}>
        <div className='bg-white shadow-lg rounded-sm mb-8'>
          <div className='flex flex-col md:flex-row md:-mr-px'>
            <div className='min-w-60'></div>
            <div className='grow p-6 space-y-6 w-full divide-y divide-gray-200'>
              <div className='flex flex-col gap-5 mb-8'>
                <div className='flex-1'>
                  <SectionTitle title='Authentication' />
                </div>
                {!!currentSubscription ? (
                  <>
                    <div className='flex-1 md:w-1/2'>
                      <h4 className='block text-sm font-medium mb-1'>
                        Username
                      </h4>
                      <CopyField value={currentSubscription.userName} />
                    </div>
                    <div className='flex-1 md:w-1/2'>
                      <h4 className='block text-sm font-medium mb-1'>
                        Password
                      </h4>
                      <CopyField value={currentSubscription.password} />
                    </div>
                    <div className='flex-1'>
                      <SectionTitle title='Settings' />
                    </div>
                    <div className='flex-1 md:w-1/2'>
                      <h4 className='block text-sm font-medium mb-1'>
                        Property TP Request Limit
                      </h4>
                      <p className='text-sm'>
                        {currentSubscription.propertyTprequestLimit}
                      </p>
                    </div>
                    <div className='flex-1 md:w-1/2'>
                      <h4 className='block text-sm font-medium mb-1'>
                        Search Timeout Seconds
                      </h4>
                      <p className='text-sm'>
                        {currentSubscription.searchTimeoutSeconds}
                      </p>
                    </div>
                    <div className='flex-1 md:w-1/2'>
                      <h4 className='block text-sm font-medium mb-1'>
                        Currency Code
                      </h4>
                      <p className='text-sm'>
                        {currentSubscription.currencyCode}
                      </p>
                    </div>
                    <div className='flex-1 md:w-1/2'>
                      <div className='flex items-center justify-between'>
                        <h4 className='block text-sm font-medium mb-1'>
                          Log Main Search Error
                        </h4>
                        <YesOrNo
                          isActive={!!currentSubscription.logMainSearchError}
                        />
                      </div>
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
          title='Data fetching error'
          description={error as string}
          show={showNotification}
          setShow={setShowNotification}
          status={NotificationStatus.ERROR}
        />
      )}
    </>
  );
});
