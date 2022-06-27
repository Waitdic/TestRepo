import React, { useState, useEffect, useRef } from 'react';
import { Link, NavLink, useLocation } from 'react-router-dom';

import { SidebarLinkGroup, TenantSelector } from '@/components';
import { useSelector } from 'react-redux';
import { RootState } from '@/store';
import classNames from 'classnames';
import getStaticConsoleIcon from '@/utils/getStaticConsoleIcon';

type Props = {
  sidebarOpen: boolean;
  setSidebarOpen: React.Dispatch<React.SetStateAction<boolean>>;
};

const Sidebar: React.FC<Props> = ({ sidebarOpen, setSidebarOpen }) => {
  const { pathname } = useLocation();
  const tenants = useSelector((state: RootState) => state.app.user?.tenants);
  const modules = useSelector((state: RootState) => state.app.modules);

  const activeModule = modules.filter((module) => module.isActive)[0];
  const currentConsoles = activeModule?.consoles;

  const trigger = useRef<any>(null);
  const sidebar = useRef<any>(null);

  const storedSidebarExpanded = localStorage.getItem('sidebar-expanded');
  const [sidebarExpanded, setSidebarExpanded] = useState<boolean>(
    storedSidebarExpanded === null ? false : storedSidebarExpanded === 'true'
  );

  const renderConsoles = () => {
    if (!tenants?.length) return null;
    return currentConsoles?.map(({ name, uri }) => (
      <Link
        key={name}
        to={uri}
        className={classNames(
          pathname.includes(name.toLowerCase()) && pathname !== '/'
            ? 'text-primary'
            : 'text-gray-600 hover:text-gray-900',
          'group flex items-center text-sm font-medium rounded-md mb-3'
        )}
      >
        <>
          <span
            className={classNames('group-hover:text-primary', {
              'mr-2': sidebarExpanded,
            })}
          >
            {getStaticConsoleIcon(
              name.toLowerCase(),
              pathname.includes(name.toLowerCase()) && pathname !== '/'
            )}
          </span>
          {sidebarExpanded && (
            <span className='group-hover:text-primary'>{name}</span>
          )}
        </>
      </Link>
    ));
  };

  // close on click outside
  useEffect(() => {
    const clickHandler = ({ target }: any) => {
      if (!sidebar.current || !trigger.current) return;
      if (
        !sidebarOpen ||
        sidebar.current.contains(target) ||
        trigger.current.contains(target)
      )
        return;
      setSidebarOpen(false);
    };
    document.addEventListener('click', clickHandler);
    return () => document.removeEventListener('click', clickHandler);
  });

  // close if the esc key is pressed
  useEffect(() => {
    const keyHandler = ({ keyCode }: { keyCode: number }) => {
      if (!sidebarOpen || keyCode !== 27) return;
      setSidebarOpen(false);
    };
    document.addEventListener('keydown', keyHandler);
    return () => document.removeEventListener('keydown', keyHandler);
  });

  useEffect(() => {
    localStorage.setItem('sidebar-expanded', sidebarExpanded.toString());
    if (sidebarExpanded) {
      document.querySelector('body')?.classList.add('sidebar-expanded');
    } else {
      document.querySelector('body')?.classList.remove('sidebar-expanded');
    }
  }, [sidebarExpanded]);

  return (
    <div>
      {/* Sidebar backdrop (mobile only) */}
      <div
        className={`fixed inset-0 bg-slate-900 bg-opacity-30 z-40 lg:hidden lg:z-auto transition-opacity duration-200 ${
          sidebarOpen ? 'opacity-100' : 'opacity-0 pointer-events-none'
        }`}
        aria-hidden='true'
      ></div>

      {/* Sidebar */}
      <div
        id='sidebar'
        ref={sidebar}
        className={`border-gray-200 border-r flex flex-col absolute z-40 left-0 top-0 lg:static lg:left-auto lg:top-auto lg:translate-x-0 transform h-screen overflow-y-scroll lg:overflow-y-auto no-scrollbar w-64 lg:w-20 lg:sidebar-expanded:!w-64 2xl:!w-64 shrink-0 bg-white p-4 transition-all duration-200 ease-in-out ${
          sidebarOpen ? 'translate-x-0' : '-translate-x-64'
        }`}
      >
        {/* Sidebar header */}
        <div className='flex justify-between mb-10 pr-3 sm:px-2'>
          {/* Close button */}
          <button
            ref={trigger}
            className='lg:hidden text-slate-500 hover:text-slate-400'
            onClick={() => setSidebarOpen(!sidebarOpen)}
            aria-controls='sidebar'
            aria-expanded={sidebarOpen}
          >
            <span className='sr-only'>Close sidebar</span>
            <svg
              className='w-6 h-6 fill-current'
              viewBox='0 0 24 24'
              xmlns='http://www.w3.org/2000/svg'
            >
              <path d='M10.7 18.7l1.4-1.4L7.8 13H20v-2H7.8l4.3-4.3-1.4-1.4L4 12z' />
            </svg>
          </button>
          {/* Logo */}
          <NavLink end to='/' className='block'>
            <img src='/iVectorOne_Logo-768x207.png' />
          </NavLink>
        </div>

        {/* Links */}
        <div className='space-y-8 flex flex-col h-full'>
          {/* Pages group */}
          <div
            className={classNames({
              'flex flex-col items-center': !sidebarExpanded,
            })}
          >
            {renderConsoles()}
          </div>
          {/* More group */}
          <div className='mt-5'>
            <TenantSelector sidebarExpanded={sidebarExpanded} />
          </div>
        </div>

        {/* Expand / collapse button */}
        <div className='pt-3 hidden lg:inline-flex 2xl:hidden justify-end mt-auto'>
          <div className='py-2'>
            <button onClick={() => setSidebarExpanded(!sidebarExpanded)}>
              <span className='sr-only'>Expand / collapse sidebar</span>
              <svg
                className='w-6 h-6 fill-current sidebar-expanded:rotate-180'
                viewBox='0 0 24 24'
              >
                <path
                  className='text-slate-400'
                  d='M19.586 11l-5-5L16 4.586 23.414 12 16 19.414 14.586 18l5-5H7v-2z'
                />
                <path className='text-slate-600' d='M3 23H1V1h2z' />
              </svg>
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default React.memo(Sidebar);
