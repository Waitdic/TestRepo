import React, { useState } from 'react';
//
import { Sidebar, Header, DropdownFilter, Datepicker } from '@/components';

type Props = {
  children: React.ReactNode;
};

const Dashboard: React.FC<Props> = ({ children }) => {
  const [sidebarOpen, setSidebarOpen] = useState(false);

  return (
    <div className='flex h-screen overflow-hidden'>
      {/* Sidebar */}
      <Sidebar sidebarOpen={sidebarOpen} setSidebarOpen={setSidebarOpen} />

      {/* Content area */}
      <div className='relative flex flex-col flex-1 overflow-y-auto overflow-x-hidden'>
        {/*  Site header */}
        <Header sidebarOpen={sidebarOpen} setSidebarOpen={setSidebarOpen} />

        <main className='px-4 sm:px-6 lg:px-8 py-8 w-full max-w-9xl mx-auto bg-slate-100 h-full'>
          {/* Content */}
          {children}
        </main>
      </div>
    </div>
  );
};

export default React.memo(Dashboard);
