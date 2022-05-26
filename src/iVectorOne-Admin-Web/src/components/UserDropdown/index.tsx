import { Fragment, FC, memo } from 'react';
import { Link } from 'react-router-dom';
import { useSelector } from 'react-redux';
import { Menu, Transition } from '@headlessui/react';
import classnames from 'classnames';
//
import { userNavigation } from '@/temp';
//
import { SkeletonText } from '@/components';
import { RootState } from '@/store';

type Props = {
  position?: 'left' | 'right';
};

const UserDropdown: FC<Props> = ({ position = 'right' }) => {
  const user = useSelector((state: RootState) => state.app.user);
  const signOut = useSelector((state: RootState) => state.app.signOut);

  const userName = user?.fullName;

  return (
    <Menu as='div' className='ml-2 md:ml-3 relative'>
      <div>
        <Menu.Button className='max-w-xs flex items-center text-sm px-4 py-2 shadow-sm text-gray-700 hover:text-white dark:text-white font-medium rounded-md hover:bg-primary focus:outline-none focus:ring-2 focus:ring-indigo-500'>
          <span className='navbar__hover block flex items-center'>
            {userName ? userName : <SkeletonText width='w-20' height='h-2' />}
          </span>
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
            'origin-top-right right-0 absolute mt-2 w-48 rounded-md shadow-lg py-1 ring-1 ring-black ring-opacity-5 focus:outline-none bg-white z-50',
            {
              'right-0': position === 'right',
              'left-0': position === 'left',
            }
          )}
        >
          {userNavigation.map(({ name, href }) => (
            <Menu.Item key={name}>
              {({ active }) => (
                <Link
                  to={href}
                  className={classnames(
                    active ? 'bg-gray-100' : '',
                    'block px-4 py-2 text-sm text-gray-700'
                  )}
                >
                  {name}
                </Link>
              )}
            </Menu.Item>
          ))}
          <Menu.Item>
            <span
              className='block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100 cursor-pointer'
              onClick={signOut}
            >
              Sign Out
            </span>
          </Menu.Item>
        </Menu.Items>
      </Transition>
    </Menu>
  );
};

export default memo(UserDropdown);
