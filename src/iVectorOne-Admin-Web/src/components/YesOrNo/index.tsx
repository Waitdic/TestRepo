import { Switch } from '@headlessui/react';
import classnames from 'classnames';
import React from 'react';

type Props = {
  isActive: boolean;
};

const YesOrNo: React.FC<Props> = ({ isActive }) => {
  return (
    <Switch
      checked={isActive}
      onChange={() => void 0}
      className={classnames(
        isActive ? 'bg-primary' : 'bg-gray-200',
        'pointer-events-none relative inline-flex flex-shrink-0 h-6 w-11 border-2 border-transparent rounded-full cursor-pointer transition-colors ease-in-out duration-200 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500'
      )}
    >
      <span
        aria-hidden='true'
        className={classnames(
          isActive ? 'translate-x-5' : 'translate-x-0',
          'pointer-events-none inline-block h-5 w-5 rounded-full bg-white shadow transform ring-0 transition ease-in-out duration-200'
        )}
      />
    </Switch>
  );
};

export default React.memo(YesOrNo);
