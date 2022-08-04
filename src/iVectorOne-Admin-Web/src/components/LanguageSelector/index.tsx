import { FC, Fragment, memo } from 'react';
import { useDispatch } from 'react-redux';
import { Menu, Transition } from '@headlessui/react';
import { MdLanguage } from 'react-icons/md';
import classnames from 'classnames';
//
import { LOCALES } from '@/i18n/index';

const LanguageSelector: FC = () => {
  const dispatch = useDispatch();

  const locales = Object.entries(LOCALES);

  return (
    <Menu as='div' className='ml-3 relative'>
      <div>
        <Menu.Button className='max-w-xs flex items-center text-sm focus:outline-none'>
          <MdLanguage className='navbar__hover h-6 w-6 hover:text-primary' />
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
          {locales.map((locale) => (
            <Menu.Item key={locale[1]}>
              {({ active }) => {
                return (
                  <span
                    className={classnames(
                      active ? 'bg-gray-100' : '',
                      'block flex items-center px-4 py-2 text-sm textDark cursor-pointer'
                    )}
                    onClick={() => dispatch.app.updateLang(locale[1])}
                  >
                    {locale[0]}
                  </span>
                );
              }}
            </Menu.Item>
          ))}
        </Menu.Items>
      </Transition>
    </Menu>
  );
};

export default memo(LanguageSelector);
