import { Dispatch, SetStateAction, FC, memo } from 'react';
import { Link, useLocation } from 'react-router-dom';
import { useSelector } from 'react-redux';
import { Disclosure } from '@headlessui/react';
import { MenuIcon, XIcon } from '@heroicons/react/outline';
import classnames from 'classnames';
//
import { userNavigation } from '@/temp';
//
import ModuleSelector from '../ModuleSelector';
import TenantSelector from '../TenantSelector';
import UserDropdown from '../UserDropdown';
import { RootState } from '@/store';

type Props = {
  title: string;
  showSidebar: boolean;
  setShowSidebar: Dispatch<SetStateAction<boolean>>;
  handleChangeModule: (moduleId: string, uri: string) => void;
};

const Navigation: FC<Props> = ({ title, handleChangeModule }) => {
  const { pathname } = useLocation();
  const modules = useSelector((state: RootState) => state.app.modules);
  const user = useSelector((state: RootState) => state.app.user);
  const signOut = useSelector((state: RootState) => state.app.signOut);

  const userName = user?.fullName;
  const activeModule = modules.filter((module) => module.isActive)[0];
  const currentConsoles = activeModule?.consoles;

  return (
    <Disclosure as='nav' className='bg-white dark:bg-gray-800'>
      {({ open }) => (
        <>
          <div
            className='navbar sticky w-full top-0 z-10 h-16 shadow bg-white dark:bg-gray-800 text-gray-700 dark:text-gray-100 items-center flex'
            data-testid='masthead'
          >
            <div className='max-w-screen px-3 sm:px-6 lg:max-w-7xl lg:px-8 w-full items-center flex lg:ml-auto xl:mx-auto'>
              {/* Title */}
              <h1 className='navbar__title text-md md:text-xl mr-auto'>
                {title}
              </h1>
              {/* Small device sidebar open button */}
              <Disclosure.Button
                className='lg:hidden inline-flex items-center justify-center p-2 rounded-md text-gray-700 dark:text-gray-400 hover:text-white hover:bg-gray-700 focus:outline-none focus:ring-2 focus:ring-inset focus:ring-white'
                data-testid='sidebar__openBtn'
              >
                <span className='sr-only'>Open main menu</span>
                {open ? (
                  <XIcon className='block h-6 w-6' aria-hidden='true' />
                ) : (
                  <MenuIcon className='block h-6 w-6' aria-hidden='true' />
                )}
              </Disclosure.Button>
              {/* Desktop Navigation */}
              <div
                className='hidden lg:block ml-auto pl-4 flex justify-between'
                data-testid='masthead__nav--desktop'
              >
                <div className='ml-4 flex items-center lg:ml-6'>
                  {/* Module Selector */}
                  <ModuleSelector handleChangeModule={handleChangeModule} />
                  {/* Tenant Selector */}
                  <TenantSelector />
                  {/* User dropdown */}
                  <UserDropdown />
                </div>
              </div>
            </div>
          </div>

          <Disclosure.Panel className='lg:hidden'>
            <div className='px-2 pt-2 pb-3 space-y-1 sm:px-3'>
              <div className='flex mb-2'>
                {/* Module Selector */}
                <ModuleSelector
                  handleChangeModule={handleChangeModule}
                  position='left'
                />
                {/* Tenant Selector */}
                <TenantSelector position='left' />
              </div>
              {currentConsoles &&
                currentConsoles.map(({ name, uri }) => (
                  <Link key={name} to={uri}>
                    <span
                      className={classnames(
                        pathname.includes(name.toLowerCase()) &&
                          pathname !== '/'
                          ? 'sidebar__link--active bg-primary dark:bg-gray-900 dark:md:bg-primary text-white'
                          : 'sidebar__link text-gray-700 dark:text-gray-400 hover:text-white hover:bg-gray-700 md:text-gray-600 md:hover:bg-gray-50 md:hover:text-gray-900',
                        'group flex items-center px-2 py-2 mb-2 text-base md:text-sm font-medium rounded-md'
                      )}
                    >
                      {name}
                    </span>
                  </Link>
                ))}
            </div>
            <div className='pt-4 pb-3 border-t border-gray-400 dark:border-gray-700'>
              <div className='flex items-center px-5 sm:px-6'>
                <div className='text-base font-medium text-gray-700 dark:text-white'>
                  {userName}
                </div>
              </div>
              <div className='mt-3 px-2 space-y-1 sm:px-3'>
                {userNavigation.map((item) => (
                  <Link
                    key={item.name}
                    to={item.href}
                    className='block px-3 py-2 rounded-md text-base font-medium text-gray-700 dark:text-gray-400 dark:hover:text-white hover:bg-gray-700'
                  >
                    {item.name}
                  </Link>
                ))}
                <span
                  className='block px-3 py-2 rounded-md text-base font-medium text-gray-700 dark:text-gray-400 dark:hover:text-white hover:bg-gray-700 cursor-pointer'
                  onClick={signOut}
                >
                  Sign Out
                </span>
              </div>
            </div>
          </Disclosure.Panel>
        </>
      )}
    </Disclosure>
  );
};

export default memo(Navigation);
