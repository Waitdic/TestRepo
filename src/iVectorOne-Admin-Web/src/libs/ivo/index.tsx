import { memo, FC, Dispatch, SetStateAction } from 'react';
//
import MainLayout from '@/layouts/Main';
import { Notification } from '@/components';
import { NotificationStatus } from '@/constants';

type Props = {
  error: string | null;
  setError: Dispatch<SetStateAction<string | null>>;
};

export const IvoView: FC<Props> = memo(({ error, setError }) => {
  const setShow = () => {
    setError(null);
  };

  return (
    <>
      <MainLayout>
        <div className='py-4'>
          <div className='flex flex-col'>{/* Module Content */}</div>
        </div>
      </MainLayout>

      {!!error && (
        <Notification
          title='Uncompleted user'
          description={error}
          show={!!error}
          setShow={setShow}
          status={NotificationStatus.ERROR}
          autoHide={false}
        />
      )}
    </>
  );
});
