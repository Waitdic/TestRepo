import getStaticSVGIcon from '@/utils/getStaticSVGIcon';
import classNames from 'classnames';
import React, { useMemo, useState } from 'react';
import { Link, NavLink } from 'react-router-dom';

type Props = {
  title: string;
  to: string;
  activecondition?: boolean;
  sidebarExpanded: boolean;
  setSidebarExpanded: React.Dispatch<React.SetStateAction<boolean>>;
  links?: {
    title: string;
    to: string;
  }[];
};

const SidebarLinkGroup: React.FC<Props> = ({
  to,
  activecondition = false,
  sidebarExpanded,
  setSidebarExpanded,
  title,
  links = null,
}) => {
  const [open, setOpen] = useState(activecondition);

  const iconName = useMemo(() => {
    if (to === '/') return 'dashboard';
    return to.toLowerCase().replace('/', '').replace(/ /g, '-');
  }, [to]);

  const handleClick = () => {
    setOpen(!open);
  };

  return (
    <li
      className={classNames('px-3 py-2 rounded-sm mb-0.5 last:mb-0', {
        // activecondition || open ? 'bg-primary' : 'bg-white'
      })}
    >
      <>
        <Link
          to={to}
          className={classNames('group block truncate text-white', {
            // 'text-white': activecondition || open,
          })}
          onClick={(e) => {
            if (!links) return;
            e.preventDefault();
            sidebarExpanded ? handleClick() : setSidebarExpanded(true);
          }}
        >
          <div className='flex items-center justify-between'>
            <div className='flex items-center'>
              {getStaticSVGIcon(
                iconName,
                `duration-200 fill-white group-hover:fill-primary shrink-0 h-6 w-6`
              )}
              <span className='text-sm group-hover:text-primary font-medium ml-3 lg:opacity-0 lg:sidebar-expanded:opacity-100 2xl:opacity-100 duration-200'>
                {title}
              </span>
            </div>
            {links && (
              <div className='flex shrink-0 ml-2'>
                <svg
                  className={`w-3 h-3 shrink-0 ml-1 fill-current text-white ${
                    open && 'transform rotate-180'
                  }`}
                  viewBox='0 0 12 12'
                >
                  <path d='M5.9 11.4L.5 6l1.4-1.4 4 4 4-4L11.3 6z' />
                </svg>
              </div>
            )}
          </div>
        </Link>
        {links &&
          links.map(({ to: subLinkTo, title: subLinkTitle }) => (
            <div
              key={subLinkTitle}
              className='lg:hidden lg:sidebar-expanded:block 2xl:block'
            >
              <ul className={`pl-9 mt-1 ${!open && 'hidden'}`}>
                <li className='mb-1 last:mb-0'>
                  <NavLink
                    end
                    to={subLinkTo}
                    className={({ isActive: _isActive }) =>
                      'block text-white hover:text-primary transition duration-150 truncate '
                    }
                  >
                    <span className='text-sm font-medium lg:opacity-0 lg:sidebar-expanded:opacity-100 2xl:opacity-100 duration-200'>
                      {subLinkTitle}
                    </span>
                  </NavLink>
                </li>
              </ul>
            </div>
          ))}
      </>
    </li>
  );
};

export default React.memo(SidebarLinkGroup);
