import { memo, FC, Dispatch, SetStateAction } from 'react';
//
import MainLayout from '@/layouts/Main';
import { Notification } from '@/components';
import { NotificationStatus } from '@/constants';

type Props = {
  error: string | null;
  setError: Dispatch<SetStateAction<string | null>>;
};

export const IvoView: FC<Props> = memo(({ error }) => {
  return (
    <MainLayout>
      <div className='min-h-screen flex flex-col'>
        {error && (
          <div className='min-h-[50vh] h-full flex flex-col justify-center items-center'>
            <h1 className='text-4xl font-semibold text-red-500 mb-2'>
              Uncompleted user
            </h1>
            <p className='text-lg text-center'>{error}</p>
          </div>
        )}
      </div>
    </MainLayout>
  );
});
