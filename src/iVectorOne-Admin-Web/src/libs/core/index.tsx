import React from 'react';
//
import MainLayout from '@/layouts/Main';
import { WelcomeBanner } from '@/components';

type Props = {
  error: string | null;
};

const Dashboard: React.FC<Props> = ({ error }) => {
  return (
    <MainLayout>
      {error ? (
        <div className='mb-12 flex flex-col justify-center items-center'>
          <h1 className='text-4xl font-semibold mb-2'>Incomplete Setup</h1>
          <p className='text-lg text-center'>{error}</p>
        </div>
      ) : (
        <WelcomeBanner />
      )}
    </MainLayout>
  );
};

export default React.memo(Dashboard);
