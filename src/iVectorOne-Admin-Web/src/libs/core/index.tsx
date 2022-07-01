import { memo, FC } from 'react';
//
import MainLayout from '@/layouts/Main';
import { WelcomeBanner } from '@/components';

type Props = {
  error: string | null;
};

export const Dashboard: FC<Props> = memo(({ error }) => {
  return (
    <MainLayout>
      {error && (
        <div className='mb-12 flex flex-col justify-center items-center'>
          <h1 className='text-4xl font-semibold text-red-500 mb-2'>
            Incomplete Setup
          </h1>
          <p className='text-lg text-center'>{error}</p>
        </div>
      )}
      <WelcomeBanner />
    </MainLayout>
  );
});
