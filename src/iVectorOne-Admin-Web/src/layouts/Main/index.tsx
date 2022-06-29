import React, { useState } from 'react';
import classNames from 'classnames';
//
import { Sidebar, Header } from '@/components';

type Props = {
  children: React.ReactNode;
  bg?: string;
  padding?: string;
};

const Dashboard: React.FC<Props> = ({
  children,
  bg = 'bg-slate-100',
  padding = 'px-4 sm:px-6 lg:px-8 py-8',
}) => {
  const [sidebarOpen, setSidebarOpen] = useState(false);

  return (
    <div className='flex h-screen overflow-hidden'>
      {/* Sidebar */}
      <Sidebar sidebarOpen={sidebarOpen} setSidebarOpen={setSidebarOpen} />

      {/* Content area */}
      <div className='relative flex flex-col flex-1 overflow-y-auto overflow-x-hidden'>
        {/*  Site header */}
        <Header sidebarOpen={sidebarOpen} setSidebarOpen={setSidebarOpen} />

        <main
          className={classNames('w-full max-w-9xl mx-auto h-full', {
            [bg]: bg,
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
