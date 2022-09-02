import {
  Fragment,
  FC,
  memo,
  useMemo,
  useState,
  useEffect,
  useRef,
} from 'react';
import { useNavigate } from 'react-router-dom';
import { Menu, Transition } from '@headlessui/react';
import classNames from 'classnames';
import { useDispatch, useSelector } from 'react-redux';
import { sortBy, uniqBy } from 'lodash';
//
import type { Tenant } from '@/types';
import { RootState } from '@/store';
import getStaticSVGIcon from '@/utils/getStaticSVGIcon';
import UncontrolledTextField from '../Fields/UncontrolledTextField';

type Props = {
  sidebarExpanded?: boolean;
};

const TenantSelector: FC<Props> = () => {
  const navigate = useNavigate();
  const user = useSelector((state: RootState) => state.app.user);

  const [filteredTenants, setFilteredTenants] = useState<Tenant[]>([]);

  const dispatch = useDispatch();

  const activeTenant = useMemo(
    () => user?.tenants?.find((tenant) => tenant.isSelected),
    [user]
  );
  const tenantList = useMemo(() => sortBy(user?.tenants, 'name'), [user]);
  const lastUsedTenants = useMemo(() => {
    const list: Tenant[] = JSON.parse(
      localStorage.getItem('lastUsedTenants') || '[]'
    )?.filter((t: Tenant) => t.tenantId !== activeTenant?.tenantId);
    return list;
  }, [activeTenant]);
  const filterableTenants = useMemo(() => {
    if (!activeTenant) return tenantList;
    let list: Tenant[] = [];
    if (filteredTenants.length > 0) {
      list = filteredTenants;
    } else if (lastUsedTenants.length > 0) {
      list = lastUsedTenants;
      if (lastUsedTenants.length < 4) {
        const availableTenants = tenantList.filter(
          (tenant) =>
            !lastUsedTenants.find(
              (t) => Number(t.tenantId) === Number(tenant.tenantId)
            )
        );
        list = list.concat(availableTenants);
      }
    } else {
      list = tenantList;
    }

    return list?.filter((i) => i.tenantId !== Number(activeTenant.tenantId));
  }, [lastUsedTenants, tenantList, filteredTenants, activeTenant]);

  const handleStoreLastUsedTenants = () => {
    if (!activeTenant) return;
    const storedTenants = new Set(lastUsedTenants);
    storedTenants.add({ ...activeTenant, isSelected: false });
    localStorage.setItem(
      'lastUsedTenants',
      JSON.stringify(Array.from(storedTenants))
    );
  };

  const handleChangeTenant = (tenantId: number) => {
    if (!user) return;
    handleStoreLastUsedTenants();
    const updatedTenants: Tenant[] = user?.tenants.map((tenant) => ({
      ...tenant,
      isSelected: tenant.tenantId === tenantId,
    }));
    dispatch.app.updateUser({
      ...user,
      tenants: updatedTenants,
    });
    dispatch.app.resetModuleList();
    setFilteredTenants([]);
    navigate('/');
  };

  const handleTenantSearch = (e: React.ChangeEvent<HTMLInputElement>) => {
    const query = e.target.value;
    if (query.length < 3) {
      setFilteredTenants([]);
      return;
    }

    const updatedTenants = tenantList.filter(({ name }) =>
      name.toLowerCase().includes(query.toLowerCase())
    );
    setFilteredTenants(updatedTenants);
  };

  if (!tenantList) {
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
        <div>
          <Menu.Items
            className={classNames(
              'origin-top-right top-full right-0 absolute mt-2 w-48 rounded-md shadow-lg py-1 ring-1 ring-black ring-opacity-5 focus:outline-none bg-white z-50'
            )}
          >
            {tenantList.length > 4 && (
              <div className='px-2'>
                <UncontrolledTextField
                  name='tenantSearch'
                  onChange={handleTenantSearch}
                />
              </div>
            )}
            <span
              className={classNames(
                'block px-4 py-2 text-sm text-dark cursor-pointer bg-gray-100 mt-1'
              )}
            >
              {activeTenant?.name}
            </span>
            {filterableTenants?.slice(0, 4)?.map(({ tenantId, name }) => (
              <Menu.Item key={tenantId}>
                {() => (
                  <span
                    className={classNames(
                      'block px-4 py-2 text-sm text-dark cursor-pointer'
                    )}
                    onClick={() => handleChangeTenant(tenantId)}
                  >
                    {name}
                  </span>
                )}
              </Menu.Item>
            ))}
          </Menu.Items>
        </div>
      </Transition>
    </Menu>
  );
};

export default memo(TenantSelector);
