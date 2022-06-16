import { Dispatch, FC, Fragment, memo, SetStateAction } from 'react';
import { Link, useLocation } from 'react-router-dom';
import { useSelector } from 'react-redux';
import classNames from 'classnames';
import { Dialog, Transition } from '@headlessui/react';
import { XIcon } from '@heroicons/react/outline';
//
import { RootState } from '@/store';
import TenantSelector from '../TenantSelector';

type Props = {
  showSidebar: boolean;
  setShowSidebar: Dispatch<SetStateAction<boolean>>;
};

const Sidebar: FC<Props> = ({ showSidebar, setShowSidebar }) => {
  const { pathname } = useLocation();
  const modules = useSelector((state: RootState) => state.app.modules);
  const user = useSelector((state: RootState) => state.app.user);

  const activeModule = modules.filter((module) => module.isActive)[0];
  const currentConsoles = activeModule?.consoles;

  return (
    <>
      <Transition.Root show={showSidebar} as={Fragment}>
        <Dialog
          as='div'
          className='relative z-40 md:hidden'
          onClose={setShowSidebar}
        >
          <Transition.Child
            as={Fragment}
            enter='transition-opacity ease-linear duration-300'
            enterFrom='opacity-0'
            enterTo='opacity-100'
            leave='transition-opacity ease-linear duration-300'
            leaveFrom='opacity-100'
            leaveTo='opacity-0'
          >
            <div className='fixed inset-0 bg-gray-600 bg-opacity-75' />
          </Transition.Child>

          <div className='fixed inset-0 flex z-40'>
            <Transition.Child
              as={Fragment}
              enter='transition ease-in-out duration-300 transform'
              enterFrom='-translate-x-full'
              enterTo='translate-x-0'
              leave='transition ease-in-out duration-300 transform'
              leaveFrom='translate-x-0'
              leaveTo='-translate-x-full'
            >
              <Dialog.Overlay className='relative flex-1 flex flex-col max-w-xs w-full bg-white dark:bg-gray-800'>
                <Transition.Child
                  as={Fragment}
                  enter='ease-in-out duration-300'
                  enterFrom='opacity-0'
                  enterTo='opacity-100'
                  leave='ease-in-out duration-300'
                  leaveFrom='opacity-100'
                  leaveTo='opacity-0'
                >
                  <div className='absolute top-0 right-0 -mr-12 pt-2'>
                    <button
                      type='button'
                      className='ml-1 flex items-center justify-center h-10 w-10 rounded-full focus:outline-none focus:ring-2 focus:ring-inset focus:ring-white'
                      onClick={() => setShowSidebar(false)}
                    >
                      <span className='sr-only'>Close sidebar</span>
                      <XIcon
                        className='h-6 w-6 text-white'
                        aria-hidden='true'
                      />
                    </button>
                  </div>
                </Transition.Child>
                <div className='flex-1 h-0 pt-5 pb-4 overflow-y-auto'>
                  <div className='flex-shrink-0 flex items-center px-4'>
                    <img
                      className='h-8 w-auto'
                      src='https://tailwindui.com/img/logos/workflow-logo-indigo-600-mark-gray-800-text.svg'
                      alt='Workflow'
                    />
                  </div>
                  <nav className='mt-5 px-2 space-y-1'>
                    {currentConsoles?.map(({ name, uri }) => (
                      <Link
                        key={name}
                        to={uri}
                        className={classNames(
                          pathname.includes(name.toLowerCase()) &&
                            pathname !== '/'
                            ? 'bg-gray-100 text-gray-900 dark:text-gray-100 dark:bg-gray-700'
                            : 'text-gray-600 hover:bg-gray-50 dark:text-gray-400 hover:text-gray-900 hover:dark:bg-gray-700 dark:hover:text-gray-300',
                          'group flex items-center px-2 py-2 text-sm font-medium rounded-md'
                        )}
                      >
                        {name}
                      </Link>
                    ))}
                  </nav>
                </div>
                <div className='flex-shrink-0 flex border-t border-gray-200 dark:border-gray-500 p-4'>
                  <a href='#' className='flex-shrink-0 group block'>
                    <div className='flex items-center'>
                      <div>
                        <img
                          className='inline-block h-10 w-10 rounded-full'
                          src='https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=facearea&facepad=2&w=256&h=256&q=80'
                          alt=''
                        />
                      </div>
                      <div className='ml-3'>
                        <p className='text-base font-medium text-gray-700 group-hover:text-gray-900 dark:text-gray-200 dark:group-hover:text-gray-100'>
                          {user?.fullName}
                        </p>
                        <p className='text-sm font-medium text-gray-500 group-hover:text-gray-700 dark:text-gray-400 dark:group-hover:text-gray-100'>
                          View profile
                        </p>
                      </div>
                    </div>
                  </a>
                </div>
                <div>
                  {/* Tenant Selector */}
                  <TenantSelector />
                </div>
              </Dialog.Overlay>
            </Transition.Child>
            <div className='flex-shrink-0 w-14'>
              {/* Force sidebar to shrink to fit close icon */}
            </div>
          </div>
        </Dialog>
      </Transition.Root>

      {/* Static sidebar for desktop */}
      <div className='hidden md:flex md:w-64 md:flex-col md:fixed md:inset-y-0'>
        {/* Sidebar component, swap this element with another sidebar if you like */}
        <div className='flex-1 flex flex-col min-h-0 border-r border-gray-200 bg-white dark:bg-gray-800'>
          <div className='flex-1 flex flex-col pt-5 pb-4 overflow-y-auto'>
            <div className='flex items-center flex-shrink-0 px-4'>
              <img
                className='h-8 w-auto'
                src='https://tailwindui.com/img/logos/workflow-logo-indigo-600-mark-gray-800-text.svg'
                alt='Workflow'
              />
            </div>
            <nav className='mt-5 flex-1 px-2 bg-white dark:bg-gray-800 space-y-1'>
              {currentConsoles?.map(({ name, uri }) => (
                <Link
                  key={name}
                  to={uri}
                  className={classNames(
                    pathname.includes(name.toLowerCase()) && pathname !== '/'
                      ? 'bg-gray-100 text-gray-900 dark:text-gray-100 dark:bg-gray-700'
                      : 'text-gray-600 hover:bg-gray-50 dark:text-gray-400 hover:text-gray-900 hover:dark:bg-gray-700 dark:hover:text-gray-300',
                    'group flex items-center px-2 py-2 text-sm font-medium rounded-md'
                  )}
                >
                  {name}
                </Link>
              ))}
            </nav>
          </div>
          <div className='flex-shrink-0 flex border-t border-gray-200 dark:border-gray-500 p-4'>
            <a href='#' className='flex-shrink-0 w-full group block'>
              <div className='flex items-center'>
                <div>
                  <img
                    className='inline-block h-9 w-9 rounded-full'
                    src='https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=facearea&facepad=2&w=256&h=256&q=80'
                    alt=''
                  />
                </div>
                <div className='ml-3'>
                  <p className='text-sm font-medium text-gray-700 group-hover:text-gray-900 dark:text-gray-200 dark:group-hover:text-gray-100'>
                    {user?.fullName}
                  </p>
                  <p className='text-xs font-medium text-gray-500 group-hover:text-gray-700 dark:text-gray-400 dark:group-hover:text-gray-100'>
                    View profile
                  </p>
                </div>
              </div>
            </a>
          </div>
          <div>
            {/* Tenant Selector */}
            <TenantSelector />
          </div>
        </div>
      </div>
    </>
  );
};

export default memo(Sidebar);
