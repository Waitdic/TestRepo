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
          <h1 className='text-4xl font-semibold mb-2'>Welcome to iVectorOne</h1>
          <p className='text-lg text-center'>
            Our team are just getting your account setup, please check back
            later. <br /> If you are still seeing this message after a few hours
            please contact our support team.
          </p>
        </div>
      ) : (
        <WelcomeBanner />
      )}
    </MainLayout>
  );
};

export default React.memo(Dashboard);
