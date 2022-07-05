import { memo, useEffect, useState, FC, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { useForm, SubmitHandler } from 'react-hook-form';
import axios from 'axios';
//
import { Subscription } from '@/types';
import { useSlug } from '@/utils/use-slug';
import {
  NotificationStatus,
  InputTypes,
  ButtonColors,
  ButtonVariants,
} from '@/constants';
import MainLayout from '@/layouts/Main';
import {
  ErrorBoundary,
  SectionTitle,
  Toggle,
  Notification,
  Select,
  Button,
  TextField,
  Spinner,
} from '@/components';
import { RootState } from '@/store';
import { useSelector } from 'react-redux';
import { Switch } from '@headlessui/react';
import classnames from 'classnames';

type NotificationState = {
  status: NotificationStatus;
  message: string;
};

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
              View Subscriptions
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
                          Username
                        </h4>
                        <p className='form-input'>
                          {currentSubscription?.userName}
                        </p>
                      </div>
                      <div className='flex-1 md:w-1/2'>
                        <h4 className='block text-sm font-medium mb-1'>
                          Password
                        </h4>
                        <p className='form-input break-words'>
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
                        <p className='form-input'>
                          {currentSubscription?.propertyTprequestLimit}
                        </p>
                      </div>
                      <div className='flex-1 md:w-1/2'>
                        <h4 className='block text-sm font-medium mb-1'>
                          Search Timeout Seconds
                        </h4>
                        <p className='form-input'>
                          {currentSubscription?.searchTimeoutSeconds}
                        </p>
                      </div>
                      <div className='flex-1 md:w-1/2'>
                        <h4 className='block text-sm font-medium mb-1'>
                          Currency Code
                        </h4>
                        <p className='form-input'>
                          {currentSubscription?.currencyCode}
                        </p>
                      </div>
                      <div className='flex-1 md:w-1/2'>
                        <div className='flex items-center justify-between'>
                          <h4 className='block text-sm font-medium mb-1'>
                            Log Main Search Error
                          </h4>
                          <Switch
                            checked={
                              currentSubscription?.logMainSearchError as boolean
                            }
                            onChange={() => void 0}
                            className={classnames(
                              currentSubscription?.logMainSearchError
                                ? 'bg-primary'
                                : 'bg-gray-200',
                              'pointer-events-none relative inline-flex flex-shrink-0 h-6 w-11 border-2 border-transparent rounded-full cursor-pointer transition-colors ease-in-out duration-200 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500'
                            )}
                          >
                            <span className='sr-only'>Use setting</span>
                            <span
                              aria-hidden='true'
                              className={classnames(
                                currentSubscription?.logMainSearchError
                                  ? 'translate-x-5'
                                  : 'translate-x-0',
                                'pointer-events-none inline-block h-5 w-5 rounded-full bg-white shadow transform ring-0 transition ease-in-out duration-200'
                              )}
                            />
                          </Switch>
                        </div>
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
                </div>
              </div>
            </div>
          </div>
        </>
      </MainLayout>
    </>
  );
});
