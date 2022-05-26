import { FC, memo } from 'react';

type Props = {
  title: string;
  description?: string;
};

const SectionTitle: FC<Props> = ({ title, description = null }) => {
  return (
    <>
      <h3 className='text-lg leading-6 font-medium text-gray-900'>{title}</h3>
      {description && (
        <p className='mt-2 max-w-4xl text-sm text-gray-500'>{description}</p>
      )}
    </>
  );
};

export default memo(SectionTitle);
