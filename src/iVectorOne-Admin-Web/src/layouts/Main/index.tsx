import React, { useEffect, useState } from 'react';
import classNames from 'classnames';
import { useSelector } from 'react-redux';
import { useLocation, useNavigate } from 'react-router-dom';
//
import { Sidebar, Header, Spinner, Button, Notification } from '@/components';
import { RootState } from '@/store';
import { NotificationStatus } from '@/constants';

type Props = {
  title?: string;
  addNew?: boolean;
  addNewHref?: string;
  children: React.ReactNode;
  bg?: string;
  padding?: string;
  minHeight?: string;
  maxWidth?: string;
};

const Dashboard: React.FC<Props> = ({
  title,
  addNew = false,
  addNewHref,
  children,
  bg = 'slate-100',
  padding = 'px-4 sm:px-6 lg:px-8 py-8',
  minHeight = 'min-h-initial',
  maxWidth = 'max-w-9xl',
}) => {
  const navigate = useNavigate();
  const location = useLocation();

  const isLoading = useSelector((state: RootState) => state.app.isLoading);
  const isIncompleteSetup = useSelector(
    (state: RootState) => state.app.incompleteSetup
  );
  const notification = useSelector(
    (state: RootState) => state.app.notification
  );

  const [sidebarOpen, setSidebarOpen] = useState(false);

  useEffect(() => {
    const { pathname } = location;
    if (isIncompleteSetup && pathname !== '/') {
      if (!pathname.includes('support')) {
        navigate('/');
      }
    }
  }, [isIncompleteSetup, location]);

  return (
    <>
      <div className='flex h-screen overflow-hidden w-full z-[0]'>
        <Sidebar sidebarOpen={sidebarOpen} setSidebarOpen={setSidebarOpen} />
        <div
          id='main-layout-area'
          className={classNames(
            'relative flex flex-col flex-1 overflow-y-auto overflow-x-hidden',
            { [`bg-${bg}`]: bg }
          )}
        >
          <Header sidebarOpen={sidebarOpen} setSidebarOpen={setSidebarOpen} />
          <main
            className={classNames('w-full mx-auto', {
              [minHeight]: minHeight,
              [maxWidth]: maxWidth,
              [padding]: padding,
            })}
          >
            <>
              {!!title && (
                <div
                  className={classNames('mb-8', {
                    'flex align-start justify-between ': addNew,
                  })}
                >
                  {
                    <h1 className='text-2xl md:text-3xl text-dark-heading font-bold'>
                      {title}
                    </h1>
                  }
                  {addNew && (
                    <div className='flex gap-3'>
                      <Button text='New' isLink href={addNewHref} />
                    </div>
                  )}
                </div>
              )}
              {children}
            </>
          </main>
        </div>
        {isLoading && (
          <div className='fixed bottom-6 right-6'>
            <Spinner />
          </div>
        )}
      </div>

      {notification !== null && (
        <Notification
          status={notification.status}
          description={notification.message}
          autoHide={notification.status !== NotificationStatus.ERROR}
          title={notification?.title}
          errorInstance={notification?.instance}
        />
      )}
    </>
  );
};
export default React.memo(Dashboard);
