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
            <div className='fixed inset-0 bg-white' />
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
              <Dialog.Overlay className='relative flex-1 flex flex-col max-w-xs w-full bg-white border-gray-200 border-r'>
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
                      className='ml-1 flex items-center justify-center h-10 w-10 rounded-full focus:outline-none focus:ring-2 focus:ring-inset focus:ring-slate-900'
                      onClick={() => setShowSidebar(false)}
                    >
                      <span className='sr-only'>Close sidebar</span>
                      <XIcon
                        className='h-6 w-6 text-slate-900'
                        aria-hidden='true'
                      />
                    </button>
                  </div>
                </Transition.Child>
                <div className='flex-1 h-0 pt-5 pb-4 overflow-y-auto overflow-x-hidden'>
                  <div className='flex-shrink-0 flex items-center px-4'>
                    <img
                      className='h-12 w-auto'
                      src='/iVectorOne_Logo-768x207.png'
                      alt='Workflow'
                    />
                  </div>
                  <nav className='mt-5 px-4 space-y-1'>
                    {currentConsoles?.map(({ name, uri }) => (
                      <Link
                        key={name}
                        to={uri}
                        className={classNames(
                          pathname.includes(name.toLowerCase()) &&
                            pathname !== '/'
                            ? 'bg-gray-100 text-gray-900 px-2'
                            : 'text-gray-600 hover:text-gray-900',
                          'group flex items-center py-2 text-sm font-medium rounded-md'
                        )}
                      >
                        {name}
                      </Link>
                    ))}
                  </nav>
                </div>
                <div className='flex-shrink-0 flex border-t border-gray-200 py-3 px-2'>
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
        <div className='flex-1 flex flex-col min-h-0 border-r border-gray-200 bg-white'>
          <div className='flex-1 flex flex-col pt-5 pb-4 overflow-y-auto overflow-x-hidden'>
            <div className='flex items-center flex-shrink-0 px-4'>
              <img
                className='h-16 w-auto'
                src='/iVectorOne_Logo-768x207.png'
                alt='Workflow'
              />
            </div>
            <nav className='mt-5 flex-1 px-4 bg-white space-y-1'>
              {currentConsoles?.map(({ name, uri }) => (
                <Link
                  key={name}
                  to={uri}
                  className={classNames(
                    pathname.includes(name.toLowerCase()) && pathname !== '/'
                      ? 'bg-gray-100 text-gray-900 px-2'
                      : 'text-gray-600 hover:text-gray-900',
                    'group flex items-center py-2 text-sm font-medium rounded-md'
                  )}
                >
                  {name}
                </Link>
              ))}
            </nav>
          </div>
          <div className='flex-shrink-0 flex border-t border-gray-200 py-3 px-2'>
            {/* Tenant Selector */}
            <TenantSelector />
          </div>
        </div>
      </div>
    </>
  );
};

export default memo(Sidebar);
