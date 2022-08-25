import React, { useState, useEffect, useRef } from 'react';
import { NavLink, useLocation } from 'react-router-dom';
//
import { RoleGuard, SidebarLinkGroup } from '@/components';
import { useSelector } from 'react-redux';
import { RootState } from '@/store';

type Props = {
  sidebarOpen: boolean;
  setSidebarOpen: React.Dispatch<React.SetStateAction<boolean>>;
};

const Sidebar: React.FC<Props> = ({ sidebarOpen, setSidebarOpen }) => {
  const { pathname } = useLocation();

  const isIncompleteSetup = useSelector(
    (state: RootState) => state.app.incompleteSetup
  );

  const trigger = useRef<any>(null);
  const sidebar = useRef<any>(null);

  const [sidebarExpanded, setSidebarExpanded] = useState<boolean>(true);

  // close on click outside
  useEffect(() => {
    const clickHandler = ({ target }: any) => {
      if (!sidebar.current) return;
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
        className={`border-gray-200 border-r flex flex-col absolute z-40 left-0 top-0 lg:static lg:left-auto lg:top-auto lg:translate-x-0 transform h-screen overflow-y-scroll lg:overflow-y-auto no-scrollbar w-64 lg:w-20 lg:sidebar-expanded:!w-64 2xl:!w-64 shrink-0 bg-slate-800 p-4 transition-all duration-200 ease-in-out ${
          sidebarOpen ? 'translate-x-0' : '-translate-x-64'
        }`}
      >
        {/* Sidebar header */}
        <div className='flex justify-between mb-10 pr-3 sm:px-2'>
          {/* Close button */}
          <button
            ref={trigger}
            className='lg:hidden text-white hover:text-primary'
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
            <img src='/logo.png' />
          </NavLink>
        </div>
        {/* Links */}
        <div className='space-y-8 flex flex-col h-full'>
          {/* Pages group */}
          <div>
            <ul className='mt-3'>
              {!isIncompleteSetup && (
                <>
                  {/* Dashboard */}
                  <SidebarLinkGroup
                    activecondition={
                      pathname === '/' || pathname.includes('dashboard')
                    }
                    to='/'
                    title='Dashboard'
                    sidebarExpanded={sidebarExpanded}
                    setSidebarExpanded={setSidebarExpanded}
                  />
                  {/* Accounts */}
                  <SidebarLinkGroup
                    activecondition={
                      pathname === '/accounts' || pathname.includes('accounts')
                    }
                    to='/accounts'
                    title='Accounts'
                    sidebarExpanded={sidebarExpanded}
                    setSidebarExpanded={setSidebarExpanded}
                  />
                  {/* Suppliers */}
                  <SidebarLinkGroup
                    activecondition={
                      pathname === '/suppliers' ||
                      pathname.includes('suppliers')
                    }
                    to='/suppliers'
                    title='Suppliers'
                    sidebarExpanded={sidebarExpanded}
                    setSidebarExpanded={setSidebarExpanded}
                  />
                </>
              )}
              <RoleGuard>
                {/* Tenants */}
                <SidebarLinkGroup
                  activecondition={
                    pathname === '/tenants' || pathname.includes('tenants')
                  }
                  to='/tenants'
                  title='Tenants'
                  sidebarExpanded={sidebarExpanded}
                  setSidebarExpanded={setSidebarExpanded}
                />
                {/* Users */}
                <SidebarLinkGroup
                  activecondition={
                    pathname === '/users' || pathname.includes('users')
                  }
                  to='/users'
                  title='Users'
                  sidebarExpanded={sidebarExpanded}
                  setSidebarExpanded={setSidebarExpanded}
                />
              </RoleGuard>
              {/* Settings */}
              {/* <SidebarLinkGroup
                  activecondition={
                    pathname === '/settings' || pathname.includes('settings')
                  }
                  to='/'
                  title='Settings'
                  sidebarExpanded={sidebarExpanded}
                  setSidebarExpanded={setSidebarExpanded}
                  links={[
                    {
                      title: 'Contact',
                      to: '/settings/contact',
                    },
                    {
                      title: 'Feedback',
                      to: '/settings/feedback',
                    },
                  ]}
                /> */}
              {/* Support */}
              <SidebarLinkGroup
                activecondition={
                  pathname === '/support' || pathname.includes('support')
                }
                to='/support'
                title='Support'
                sidebarExpanded={sidebarExpanded}
                setSidebarExpanded={setSidebarExpanded}
                links={[
                  {
                    title: 'Knowledge base',
                    to: '/support/knowledge-base',
                  },
                  {
                    title: 'Change log',
                    to: '/support/change-log',
                  },
                  {
                    title: 'Road map',
                    to: '/support/road-map',
                  },
                ]}
              />
            </ul>
          </div>
        </div>
      </div>
    </div>
  );
};

export default React.memo(Sidebar);
