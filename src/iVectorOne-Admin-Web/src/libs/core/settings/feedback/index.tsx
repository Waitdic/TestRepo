import React from 'react';
//
import Main from '@/layouts/Main';
import { FeedbackPanel, SettingsSidebar } from '@/components';

const Feedback: React.FC = () => {
  return (
    <Main authGuard>
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
            <FeedbackPanel />
          </div>
        </div>
      </>
    </Main>
  );
};

export default React.memo(Feedback);
