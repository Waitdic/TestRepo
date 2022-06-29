import React from 'react';
//
import Main from '@/layouts/Main';
import { AccountPanel, SettingsSidebar } from '@/components';

const MyAccount: React.FC = () => {
  return (
    <Main>
      <>
        {/* Page header */}
        <div className='mb-8'>
          {/* Title */}
          <h1 className='text-2xl md:text-3xl text-slate-800 font-bold'>
            My Account
          </h1>
        </div>

        {/* Content */}
        <div className='bg-white shadow-lg rounded-sm mb-8'>
          <div className='flex flex-col md:flex-row md:-mr-px'>
            <SettingsSidebar />
            <AccountPanel />
          </div>
        </div>
      </>
    </Main>
  );
};

export default React.memo(MyAccount);
