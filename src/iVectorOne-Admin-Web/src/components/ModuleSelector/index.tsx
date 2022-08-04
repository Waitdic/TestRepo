import { Fragment, FC, memo } from 'react';
import { useSelector } from 'react-redux';
import { Menu, Transition } from '@headlessui/react';
import { ViewGridIcon } from '@heroicons/react/outline';
import classnames from 'classnames';
//
import { RootState } from '@/store';

type Props = {
  handleChangeModule: (moduleId: string, uri: string) => void;
  position?: 'left' | 'right';
};

const ModuleSelector: FC<Props> = ({
  handleChangeModule,
  position = 'right',
}) => {
  const modules = useSelector((state: RootState) => state.app.modules);

  return (
    <Menu as='div' className='relative'>
      <div>
        <Menu.Button
          className='max-w-xs p-1 flex items-center text-sm focus:outline-none'
          title='Change Module'
        >
          <ViewGridIcon className='navbar__hover h-6 w-6 textDark dark:text-gray-400 hover:text-primary dark:hover:text-white' />
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
            'origin-top-right  absolute mt-2 w-48 rounded-md shadow-lg py-1 ring-1 ring-black ring-opacity-5 focus:outline-none bg-white z-50',
            {
              'left-0': position === 'left',
              'right-0': position === 'right',
            }
          )}
        >
          {!!modules &&
            modules.map(({ moduleId, name, uri, isActive }) => (
              <Menu.Item key={moduleId}>
                {({ active }) => (
                  <span
                    className={classnames(
                      active ? 'bg-gray-100' : '',
                      isActive ? 'bg-gray-200' : '',
                      'block px-4 py-2 text-sm textDark cursor-pointer'
                    )}
                    onClick={() => handleChangeModule(moduleId, uri)}
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

export default memo(ModuleSelector);
