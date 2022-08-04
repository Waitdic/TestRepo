import { Dispatch, SetStateAction, FC, memo } from 'react';
import { MenuIcon } from '@heroicons/react/outline';

type Props = {
  setShowSidebar: Dispatch<SetStateAction<boolean>>;
};

const Navigation: FC<Props> = ({ setShowSidebar }) => {
  return (
    <div className='sticky top-0 z-10 md:hidden pl-1 pt-1 sm:pl-3 sm:pt-3 bg-white'>
      <button
        type='button'
        className='-ml-0.5 -mt-0.5 h-12 w-12 inline-flex items-center justify-center rounded-md textDark focus:outline-none focus:ring-2 focus:ring-inset focus:ring-indigo-500'
        onClick={() => setShowSidebar(true)}
      >
        <span className='sr-only'>Open sidebar</span>
        <MenuIcon className='h-6 w-6' aria-hidden='true' />
      </button>
    </div>
  );
};

export default memo(Navigation);
