import { memo, Fragment, FC } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { LightBulbIcon, MoonIcon } from '@heroicons/react/outline';
import { Menu, Transition } from '@headlessui/react';
import { BsPalette } from 'react-icons/bs';
import classnames from 'classnames';
//
import { RootState } from '@/store';

//? Temporary mock pallette for testing
export const themeColorPalettes = [{ name: 'Custom Palette', id: 'custom' }];

const ToggleThemeColor: FC = () => {
  const theme = useSelector((state: RootState) => state.app.theme);
  const dispatch = useDispatch();

  const handleChangeThemeColor = (variant: string) => {
    dispatch.app.updateThemeColor(variant);
  };

  return (
    <>
      <div className='ml-2 flex items-center'>
        {theme !== 'light' && (
          <LightBulbIcon
            className='navbar__hover w-8 h-6 cursor-pointer hover:text-primary'
            onClick={() => handleChangeThemeColor('light')}
          />
        )}
        {theme !== 'dark' && (
          <MoonIcon
            className='navbar__hover w-8 h-6 cursor-pointer hover:text-primary'
            onClick={() => handleChangeThemeColor('dark')}
          />
        )}
      </div>
      {/* Color Palettes dropdown */}
      <Menu as='div' className='ml-2 relative'>
        <div>
          <Menu.Button className='max-w-xs flex items-center text-sm focus:outline-none'>
            <BsPalette className='navbar__hover h-6 w-6 hover:text-primary' />
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
          <Menu.Items className='origin-top-right absolute right-0 mt-2 w-48 rounded-md shadow-lg py-1 ring-1 ring-black ring-opacity-5 focus:outline-none bg-white'>
            {themeColorPalettes.map((palette) => (
              <Menu.Item key={palette.name}>
                {({ active }) => (
                  <span
                    className={classnames(
                      active ? 'bg-gray-100' : '',
                      'block px-4 py-2 text-sm textDark cursor-pointer'
                    )}
                    onClick={() => handleChangeThemeColor(palette.id)}
                  >
                    {palette.name}
                  </span>
                )}
              </Menu.Item>
            ))}
          </Menu.Items>
        </Transition>
      </Menu>
    </>
  );
};

export default memo(ToggleThemeColor);
