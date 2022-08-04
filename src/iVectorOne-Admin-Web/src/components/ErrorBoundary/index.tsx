import { FC, memo } from 'react';

type Props = {
  title?: string;
};

const ErrorBoundary: FC<Props> = ({ title = 'Unexpected Error' }) => {
  return (
    <div className='max-w-2xl m-auto mt-16'>
      <div className='text-center px-4'>
        <div className='inline-flex items-center justify-center w-16 h-16 rounded-full bg-gradient-to-t from-slate-200 to-slate-100 mb-4'>
          <svg className='w-5 h-6 fill-current' viewBox='0 0 20 24'>
            <path
              className='text-slate-500'
              d='M10 10.562l9-5-8.514-4.73a1 1 0 00-.972 0L1 5.562l9 5z'
            />
            <path
              className='text-slate-300'
              d='M9 12.294l-9-5v10.412a1 1 0 00.514.874L9 23.294v-11z'
            />
            <path
              className='text-slate-400'
              d='M11 12.294v11l8.486-4.714a1 1 0 00.514-.874V7.295l-9 4.999z'
            />
          </svg>
        </div>
        <h2 className='text-2xl textDark font-bold mb-2'>{title}</h2>
        <div className='mb-6'>
          <p>We ran into an unexpected error completing your request.</p>
          <p>Please try again.</p>
          <p>If the problem continues contact our support desk.</p>
        </div>
      </div>
    </div>
  );
};

export default memo(ErrorBoundary);
