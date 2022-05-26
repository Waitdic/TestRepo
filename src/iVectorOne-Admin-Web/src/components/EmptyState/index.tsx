import { FC, memo } from 'react';
import { PlusIcon } from '@heroicons/react/outline';
//
import { Button } from '@/components';

type Props = {
  title: string;
  description: string;
  href: string;
  buttonText: string;
};

const EmptyState: FC<Props> = ({ title, description, href, buttonText }) => {
  return (
    <div
      className='fixed top-1/2 left-1/2 text-center'
      style={{ transform: 'translate(-50%,-50%)' }}
    >
      <h3 className='mt-2 text-sm font-medium text-gray-900'>{title}</h3>
      <p className='mt-1 text-sm text-gray-500'>{description}</p>
      <div className='mt-6'>
        <Button
          isLink
          href={href}
          text={buttonText}
          icon={<PlusIcon className='-ml-1 mr-2 h-5 w-5' aria-hidden='true' />}
        />
      </div>
    </div>
  );
};

export default memo(EmptyState);
