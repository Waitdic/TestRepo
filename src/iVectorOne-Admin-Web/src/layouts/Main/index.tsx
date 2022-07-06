import React, { useEffect, useState } from 'react';
import classNames from 'classnames';
import { useSelector } from 'react-redux';
//
import { Sidebar, Header } from '@/components';
import { RootState } from '@/store';
import { useNavigate } from 'react-router-dom';

type Props = {
  children: React.ReactNode;
  bg?: string;
  padding?: string;
  minHeight?: string;
  maxWidth?: string;
  authGuard?: boolean;
};

const Dashboard: React.FC<Props> = ({
  children,
  bg = 'slate-100',
  padding = 'px-4 sm:px-6 lg:px-8 py-8',
  minHeight = 'min-h-initial',
  maxWidth = 'max-w-9xl',
  authGuard = false,
}) => {
  const navigate = useNavigate();

  const isLoading = useSelector((state: RootState) => state.app.isLoading);
  const user = useSelector((state: RootState) => state.app.user);

  const [sidebarOpen, setSidebarOpen] = useState(false);

  useEffect(() => {
    if (!isLoading && !user && authGuard) {
      navigate('/');
    }
  }, [isLoading, user]);

  return (
    <div className='flex h-screen overflow-hidden w-full'>
      {/* Sidebar */}
      <Sidebar sidebarOpen={sidebarOpen} setSidebarOpen={setSidebarOpen} />

      {/* Content area */}
      <div
        id='main-layout-area'
        className={classNames(
          'relative flex flex-col flex-1 overflow-y-auto overflow-x-hidden',
          { [`bg-${bg}`]: bg }
        )}
      >
        {/*  Site header */}
        <Header sidebarOpen={sidebarOpen} setSidebarOpen={setSidebarOpen} />

        <main
          className={classNames('w-full mx-auto', {
            [minHeight]: minHeight,
            [maxWidth]: maxWidth,
            [padding]: padding,
          })}
        >
          {/* Content */}
          {children}
        </main>
      </div>
    </div>
  );
};
export default React.memo(Dashboard);
