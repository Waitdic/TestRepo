import { memo, FC } from 'react';
//
import MainLayout from '@/layouts/Main';

export const CoreView: FC = memo(() => {
  return (
    <MainLayout>
      <div className='flex flex-col'>{/* Inner Content Placeholder */}</div>
    </MainLayout>
  );
});
