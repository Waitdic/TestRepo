import { Fragment, ReactNode, FC, memo } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { Menu, Transition } from '@headlessui/react';
import classnames from 'classnames';
//
import { DropdownNavigationProps } from '@/types';
import { MenuPosition } from '@/constants';
import getStaticSVGIcon from '@/utils/getStaticSVGIcon';
import { Auth } from 'aws-amplify';

type Props = {
  dropdownNavigation: DropdownNavigationProps[];
  children: ReactNode | JSX.Element;
  position?: MenuPosition;
};

const DropdownMenu: FC<Props> = ({
  dropdownNavigation,
  children,
  position = MenuPosition.RIGHT,
}) => {
  const navigate = useNavigate();

  return (
    <Menu as='div' className='ml-3 relative'>
      <div className='flex'>
        <Menu.Button className='max-w-xs flex gap-1 items-center text-sm focus:outline-none'>
          {children}
          {getStaticSVGIcon('chevronDown', 'w-5 h-5')}
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
            'origin-top-right absolute mt-2 w-48 rounded-md shadow-lg py-1 ring-1 ring-black ring-opacity-5 focus:outline-none bg-white z-50',
            {
              'left-0': position === 'left',
              'right-0': position === 'right',
            }
          )}
        >
          {dropdownNavigation &&
            dropdownNavigation.map(({ name, href, action }) => (
              <Menu.Item key={name}>
                {({ active }) => {
                  return (
                    <>
                      {!!href && (
                        <Link
                          className={classnames(
                            active ? 'bg-gray-100' : '',
                            'block px-4 py-2 text-sm text-gray-700 cursor-pointer'
                          )}
                          to={href}
                        >
                          {name}
                        </Link>
                      )}
                      {!!action && (
                        <span
                          className={classnames(
                            active ? 'bg-gray-100' : '',
                            'block px-4 py-2 text-sm text-gray-700 cursor-pointer'
                          )}
                          onClick={() => {
                            Auth.signOut({ global: true });
                            localStorage.clear();
                            navigate('/');
                            window.location.reload();
                          }}
                        >
                          {name}
                        </span>
                      )}
                    </>
                  );
                }}
              </Menu.Item>
            ))}
        </Menu.Items>
      </Transition>
    </Menu>
  );
};

export default memo(DropdownMenu);
