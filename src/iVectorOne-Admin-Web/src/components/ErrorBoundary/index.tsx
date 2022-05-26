import { FC, memo } from 'react';

type Props = {
  title?: string;
};

const ErrorBoundary: FC<Props> = ({ title = 'Unexpected Error' }) => {
  return (
    <div
      className='fixed top-1/2 left-1/2 text-center'
      style={{ transform: 'translate(-50%, -50%)' }}
    >
      <h3 className='mt-2 text-sm font-medium text-gray-900'>{title}</h3>
      <p className='mt-1 text-sm text-gray-500'>
        We ran into an unexpected error completing your request.
      </p>
      <p className='mt-1 text-sm text-gray-500'>Please try again.</p>
      <p className='mt-1 text-sm text-gray-500'>
        If the problem continues contact our support desk.
      </p>
    </div>
  );
};

export default memo(ErrorBoundary);
