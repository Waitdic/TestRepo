import { FC, memo } from 'react';
import { Link, useLocation } from 'react-router-dom';
import { useSelector } from 'react-redux';
import classnames from 'classnames';
//
import { RootState } from '@/store';

const Sidebar: FC = () => {
  const { pathname } = useLocation();
  const modules = useSelector((state: RootState) => state.app.modules);

  const activeModule = modules.filter((module) => module.isActive)[0];
  const currentConsoles = activeModule?.consoles;

  return (
    <div
      className='sidebar hidden lg:block lg:col-span-2 lg:sticky lg:top-16 lg:inset-y-0'
      data-testid='sidebar--desktop'
    >
      {/* Sidebar component, swap this element with another sidebar if you like */}
      <div className='flex flex-col flex-grow h-full'>
        <div className='flex-1 flex flex-col bg-transparent overflow-y-auto'>
          <nav className='pb-4 space-y-1'>
            {currentConsoles &&
              currentConsoles.map(({ name, uri }) => (
                <Link key={name} to={uri}>
                  <span
                    className={classnames(
                      pathname.includes(name.toLowerCase()) && pathname !== '/'
                        ? 'sidebar__link--active bg-primary hover:bg-primaryHover text-white'
                        : 'sidebar__link text-gray-600 hover:bg-gray-50 hover:text-gray-900',
                      'group flex items-center px-3 py-2 mb-2 text-sm font-medium rounded-md'
                    )}
                  >
                    {name}
                  </span>
                </Link>
              ))}
          </nav>
          <p className='px-2 py-4 mt-auto'>Version 1.0</p>
        </div>
      </div>
    </div>
  );
};

export default memo(Sidebar);
