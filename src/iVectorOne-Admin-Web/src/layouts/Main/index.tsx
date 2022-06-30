import React, { useState } from 'react';
import classNames from 'classnames';
//
import { Sidebar, Header } from '@/components';

type Props = {
  children: React.ReactNode;
  bg?: string;
  padding?: string;
  minHeight?: string;
  maxWidth?: string;
};

const Dashboard: React.FC<Props> = ({
  children,
  bg = 'slate-100',
  padding = 'px-4 sm:px-6 lg:px-8 py-8',
  minHeight = 'min-h-initial',
  maxWidth = 'max-w-9xl',
}) => {
  const [sidebarOpen, setSidebarOpen] = useState(false);

  return (
    <div className='flex h-screen overflow-hidden'>
      {/* Sidebar */}
      <Sidebar sidebarOpen={sidebarOpen} setSidebarOpen={setSidebarOpen} />

      {/* Content area */}
      <div
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
