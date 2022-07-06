import { memo, useEffect, useState, FC, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { useSelector } from 'react-redux';
//
import { Subscription } from '@/types';
import { useSlug } from '@/utils/use-slug';
import { ButtonColors } from '@/constants';
import MainLayout from '@/layouts/Main';
import { SectionTitle, Button, Spinner, YesOrNo } from '@/components';
import { RootState } from '@/store';

type Props = {};

export const SubscriptionView: FC<Props> = memo(({}) => {
  const navigate = useNavigate();
  const { slug } = useSlug();

  const subscriptions = useSelector(
    (state: RootState) => state.app.subscriptions
  );

  const [currentSubscription, setCurrentSubscription] = useState(
    null as Subscription | null
  );

  const loadSubscription = useCallback(() => {
    if (subscriptions.length > 0) {
      const currentSubscription = subscriptions.find(
        (sub) => sub.subscriptionId === Number(slug)
      );

      if (!currentSubscription) {
        navigate('/subscriptions');
      } else {
        setCurrentSubscription(currentSubscription);
      }
    }
  }, [subscriptions, navigate, slug]);

  useEffect(() => {
    loadSubscription();
  }, [subscriptions, navigate, slug, loadSubscription]);

  return (
    <>
      <MainLayout>
        <>
          {/* Page header */}
          <div className='mb-8'>
            {/* Title */}
            <h1 className='text-2xl md:text-3xl text-slate-800 font-bold'>
              View Subscription
            </h1>
          </div>

          {/* Content */}
          <div className='bg-white shadow-lg rounded-sm mb-8'>
            <div className='flex flex-col md:flex-row md:-mr-px'>
              <div className='min-w-60'></div>
              <div className='grow p-6 space-y-6 w-full divide-y divide-gray-200'>
                <div className='flex flex-col gap-5 mb-8'>
                  <div className='flex-1'>
                    <SectionTitle title='Subscription' />
                  </div>
                  {subscriptions.length > 0 ? (
                    <>
                      <div className='flex-1 md:w-1/2'>
                        <h4 className='block text-sm font-medium mb-1'>
                          Authentication
                        </h4>
                        <p className='text-sm'>
                          {currentSubscription?.userName}
                        </p>
                      </div>
                      <div className='flex-1 md:w-1/2'>
                        <h4 className='block text-sm font-medium mb-1'>
                          Password
                        </h4>
                        <p className='text-sm break-words'>
                          {currentSubscription?.password}
                        </p>
                      </div>
                      <div className='flex-1'>
                        <SectionTitle title='Settings' />
                      </div>
                      <div className='flex-1 md:w-1/2'>
                        <h4 className='block text-sm font-medium mb-1'>
                          Property TP Request Limit
                        </h4>
                        <p className='text-sm'>
                          {currentSubscription?.propertyTprequestLimit}
                        </p>
                      </div>
                      <div className='flex-1 md:w-1/2'>
                        <h4 className='block text-sm font-medium mb-1'>
                          Search Timeout Seconds
                        </h4>
                        <p className='text-sm'>
                          {currentSubscription?.searchTimeoutSeconds}
                        </p>
                      </div>
                      <div className='flex-1 md:w-1/2'>
                        <h4 className='block text-sm font-medium mb-1'>
                          Currency Code
                        </h4>
                        <p className='text-sm'>
                          {currentSubscription?.currencyCode}
                        </p>
                      </div>
                      <div className='flex-1 md:w-1/2'>
                        <div className='flex items-center justify-between'>
                          <h4 className='block text-sm font-medium mb-1'>
                            Log Main Search Error
                          </h4>
                          <YesOrNo
                            isActive={!!currentSubscription?.logMainSearchError}
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
        </>
      </MainLayout>
    </>
  );
});
