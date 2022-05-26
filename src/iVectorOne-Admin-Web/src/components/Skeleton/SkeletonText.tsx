import { FC, memo } from 'react';
import classnames from 'classnames';

type SkeletonTextProps = {
  width: string;
  height: string;
};

const SkeletonText: FC<SkeletonTextProps> = ({ width, height }) => (
  <span
    className={classnames(
      'block bg-gray-700 dark:bg-gray-100 opacity-0 animate-pulse rounded-full',
      {
        [width]: width,
        [height]: height,
      }
    )}
  />
);

export default memo(SkeletonText);
