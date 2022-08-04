import React from 'react';
//
import Main from '@/layouts/Main';
import { AccountPanel, SettingsSidebar } from '@/components';

const Contact: React.FC = () => {
  return (
    <Main title='Contact'>
      <div className='bg-white shadow-lg rounded-sm mb-8'>
        <div className='flex flex-col md:flex-row md:-mr-px'>
          <SettingsSidebar />
          <AccountPanel />
        </div>
      </div>
    </Main>
  );
};

export default React.memo(Contact);
