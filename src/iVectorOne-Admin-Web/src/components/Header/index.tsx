import React from 'react';
import { useSelector } from 'react-redux';
import { Auth } from 'aws-amplify';
//
import { RootState } from '@/store';
import { DropdownMenu, TenantSelector } from '@/components';

type Props = {
  sidebarOpen: boolean;
  setSidebarOpen: React.Dispatch<React.SetStateAction<boolean>>;
};

const Header: React.FC<Props> = ({ sidebarOpen, setSidebarOpen }) => {
  const user = useSelector((state: RootState) => state.app.user);

  return (
    <header className='sticky top-0 bg-white border-b border-slate-200 z-30'>
      <div className='px-4 sm:px-6 lg:px-8'>
        <div className='flex items-center justify-between h-16 -mb-px'>
          {/* Header: Left side */}
          <div className='flex'>
            {/* Hamburger button */}
            <button
              className='text-dark lg:hidden'
              aria-controls='sidebar'
              aria-expanded={sidebarOpen}
              onClick={() => setSidebarOpen(!sidebarOpen)}
            >
              <span className='sr-only'>Open sidebar</span>
              <svg
                className='w-6 h-6 fill-current'
                viewBox='0 0 24 24'
                xmlns='http://www.w3.org/2000/svg'
              >
                <rect x='4' y='5' width='16' height='2' />
                <rect x='4' y='11' width='16' height='2' />
                <rect x='4' y='17' width='16' height='2' />
              </svg>
            </button>
          </div>

          {/* Header: Right side */}
          <div className='flex items-center space-x-3'>
            {!!user?.tenants?.length && <TenantSelector />}
            <DropdownMenu
              dropdownNavigation={[
                {
                  name: 'Logout',
                  action: Auth.signOut,
                },
              ]}
            >
              {user?.fullName}
            </DropdownMenu>
          </div>
        </div>
      </div>
    </header>
  );
};

export default React.memo(Header);
