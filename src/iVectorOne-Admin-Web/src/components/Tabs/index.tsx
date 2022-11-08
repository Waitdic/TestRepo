import classNames from 'classnames';
import React from 'react';

export type Props = {
  tabs: { name: string; href: string; current: boolean }[];
  onTabChange: (href: string) => void;
};

const Tabs: React.FC<Props> = ({ tabs, onTabChange }) => {
  return (
    <div>
      <div className='hidden sm:block'>
        <div className='border-b border-gray-200'>
          <nav className='-mb-px flex space-x-8' aria-label='Tabs'>
            {tabs.map(({ name, href, current }) => (
              <button
                key={name}
                onClick={() => onTabChange(href)}
                className={classNames(
                  current
                    ? 'border-primary text-primary'
                    : 'border-transparent text-gray-500 hover:text-primary hover:border-primary',
                  'whitespace-nowrap py-4 px-1 border-b-2 font-medium text-sm'
                )}
                aria-current={current ? 'page' : undefined}
              >
                {name}
              </button>
            ))}
          </nav>
        </div>
      </div>
    </div>
  );
};

export default React.memo(Tabs);
