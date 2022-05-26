import { FC, memo } from 'react';
import classnames from 'classnames';

type Props = {
  className?: string | null;
};

const Spinner: FC<Props> = ({ className = null }) => (
  <div className={classnames('lds-ring', { [className as string]: className })}>
    <div></div>
    <div></div>
    <div></div>
    <div></div>
  </div>
);

export default memo(Spinner);
