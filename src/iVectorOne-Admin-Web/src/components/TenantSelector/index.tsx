import { Fragment, FC, memo } from 'react';
import { useNavigate } from 'react-router-dom';
import { Menu, Transition } from '@headlessui/react';
import classNames from 'classnames';
import { useDispatch, useSelector } from 'react-redux';
//
import { Tenant } from '@/types';
import { RootState } from '@/store';
import getStaticSVGIcon from '@/utils/getStaticSVGIcon';

type Props = {
  sidebarExpanded?: boolean;
};

const TenantSelector: FC<Props> = () => {
  const navigate = useNavigate();
  const user = useSelector((state: RootState) => state.app.user);
  const activeTenant = user?.tenants?.find((tenant) => tenant.isSelected);

  const dispatch = useDispatch();

  const handleChangeTenant = (tenantId: number) => {
    if (!user) return;

    const updatedTenants: Tenant[] = user?.tenants.map((tenant) => ({
      ...tenant,
      isSelected: tenant.tenantId === tenantId,
    }));
    dispatch.app.updateUser({
      ...user,
      tenants: updatedTenants,
    });
    dispatch.app.resetModuleList();
    navigate('/');
  };

  if (user?.tenants?.length === 0) {
    return null;
  }

  return (
    <Menu as='div' className='relative'>
      <div>
        <Menu.Button
          className={classNames(
            'group flex items-center max-w-xs text-sm focus:outline-none'
          )}
          title='Change Tenant'
        >
          <div className='flex gap-1'>
            <p className='ml-2 text-sm'>{activeTenant?.name}</p>
            {getStaticSVGIcon('chevronDown', 'w-5 h-5')}
          </div>
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
          className={classNames(
            'origin-top-right top-full right-0 absolute mt-2 w-48 rounded-md shadow-lg py-1 ring-1 ring-black ring-opacity-5 focus:outline-none bg-white z-50'
          )}
        >
          {user?.tenants &&
            user?.tenants.map(({ tenantId, name, isSelected }) => (
              <Menu.Item key={tenantId}>
                {({ active }) => (
                  <span
                    className={classNames(
                      active ? 'bg-gray-100' : '',
                      isSelected ? 'bg-gray-100' : '',
                      'block px-4 py-2 text-sm textDark cursor-pointer'
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
