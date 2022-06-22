import { Fragment, FC, memo } from 'react';
import { useNavigate } from 'react-router-dom';
import { Menu, Transition } from '@headlessui/react';
import classnames from 'classnames';
import { UsersIcon } from '@heroicons/react/outline';
import { useDispatch, useSelector } from 'react-redux';
//
import { Tenant } from '@/types';
import { RootState } from '@/store';

const TenantSelector: FC = () => {
  const navigate = useNavigate();
  const user = useSelector((state: RootState) => state.app.user);
  const activeTenant = user?.tenants?.find((tenant) => tenant.isActive);

  const dispatch = useDispatch();

  const handleChangeTenant = (tenantId: number) => {
    if (!user) return;

    const updatedTenants: Tenant[] = user?.tenants.map((tenant) => ({
      ...tenant,
      isActive: tenant.tenantId === tenantId ? true : false,
    }));
    dispatch.app.updateUser({
      ...user,
      tenants: updatedTenants,
    });
    dispatch.app.resetModuleList();
    navigate('/');
  };

  return (
    <Menu as='div' className='relative'>
      <div>
        <Menu.Button
          className='group flex items-center max-w-xs p-1 text-sm focus:outline-none'
          title='Change Tenant'
        >
          <>
            <UsersIcon className='navbar__hover h-6 w-6 text-gray-700 group-hover:text-primary' />
            <p className='ml-2 text-gray-700 text-sm font-medium group-hover:text-primary'>
              {activeTenant?.name}
            </p>
          </>
        </Menu.Button>
      </div>
      <Transition
        as={Fragment}
        enter='transition ease-out duration-100'
        enterFrom='transform opacity-0 scale-95'
        enterTo='transform opacity-100 scale-100'
        leave='transition ease-in duration-75'
        leaveFrom='transform opacity-100 scale-100'
        leaveTo='transform opacity-0 scale-95'
      >
        <Menu.Items
          className={classnames(
            'origin-bottom-right bottom-0 left-0 absolute mt-2 w-48 rounded-md shadow-lg py-1 ring-1 ring-black ring-opacity-5 focus:outline-none bg-white z-50'
          )}
        >
          {user?.tenants &&
            user?.tenants.map(({ tenantId, name, isActive }) => (
              <Menu.Item key={tenantId}>
                {({ active }) => (
                  <span
                    className={classnames(
                      active ? 'bg-gray-100' : '',
                      isActive ? 'bg-gray-100' : '',
                      'block px-4 py-2 text-sm text-gray-700 cursor-pointer'
                    )}
                    onClick={() => handleChangeTenant(tenantId)}
                  >
                    {name}
                  </span>
                )}
              </Menu.Item>
            ))}
        </Menu.Items>
      </Transition>
    </Menu>
  );
};

export default memo(TenantSelector);
