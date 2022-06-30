import { FC, memo } from 'react';

type Props = {
  title: string;
  description?: string;
};

const SectionTitle: FC<Props> = ({ title, description = null }) => {
  return (
    <>
      <h2 className='text-2xl text-slate-800 font-bold mb-3'>{title}</h2>
      {description && (
        <p className='mt-2 max-w-4xl text-sm text-gray-500'>{description}</p>
      )}
    </>
  );
};

export default memo(SectionTitle);
