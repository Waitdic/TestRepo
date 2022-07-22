import React from 'react';
//
import Main from '@/layouts/Main';
import { FeedbackPanel, SettingsSidebar } from '@/components';

const Feedback: React.FC = () => {
  return (
    <Main title='My Account'>
      <div className='bg-white shadow-lg rounded-sm mb-8'>
        <div className='flex flex-col md:flex-row md:-mr-px'>
          <SettingsSidebar />
          <FeedbackPanel />
        </div>
      </div>
    </Main>
  );
};

export default React.memo(Feedback);
