import { memo, FC } from 'react';
//
import MainLayout from '@/layouts/Main';

export const IvoView: FC = memo(() => {
  return (
    <MainLayout>
      <div className='py-4'>
        <div className='flex flex-col'>{/* Module Content */}</div>
      </div>
    </MainLayout>
  );
});
