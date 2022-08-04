import classNames from 'classnames';
import { FC, memo } from 'react';

type Props = {
  title: string;
  description?: string;
  color?: string;
};

const SectionTitle: FC<Props> = ({
  title,
  description = null,
  color = 'text-dark',
}) => {
  return (
    <>
      <h2 className={classNames('text-2xl font-bold mb-3', color)}>{title}</h2>
      {description && (
        <p className='mt-2 max-w-4xl text-sm text-gray-500'>{description}</p>
      )}
    </>
  );
};

export default memo(SectionTitle);
