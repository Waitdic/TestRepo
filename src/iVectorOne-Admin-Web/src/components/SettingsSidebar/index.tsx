import React, { useMemo } from 'react';
import { NavLink, useLocation } from 'react-router-dom';

type Props = {};

const SettingsSidebar: React.FC<Props> = () => {
  const location = useLocation();
  const { pathname } = location;

  const sidebarGroups = useMemo(
    () => [
      {
        title: 'Business settings',
        items: [
          {
            title: 'My account',
            to: '/settings/my-account',
            icon: (
              <svg
                className={`w-4 h-4 shrink-0 fill-current text-slate-400 mr-2 ${
                  pathname.includes('/settings/account') && 'text-indigo-400'
                }`}
                viewBox='0 0 16 16'
              >
                <path d='M12.311 9.527c-1.161-.393-1.85-.825-2.143-1.175A3.991 3.991 0 0012 5V4c0-2.206-1.794-4-4-4S4 1.794 4 4v1c0 1.406.732 2.639 1.832 3.352-.292.35-.981.782-2.142 1.175A3.942 3.942 0 001 13.26V16h14v-2.74c0-1.69-1.081-3.19-2.689-3.733zM6 4c0-1.103.897-2 2-2s2 .897 2 2v1c0 1.103-.897 2-2 2s-2-.897-2-2V4zm7 10H3v-.74c0-.831.534-1.569 1.33-1.838 1.845-.624 3-1.436 3.452-2.422h.436c.452.986 1.607 1.798 3.453 2.422A1.943 1.943 0 0113 13.26V14z' />
              </svg>
            ),
          },
        ],
      },
      {
        title: 'Experience',
        items: [
          {
            title: 'Give Feedback',
            to: '/settings/feedback',
            icon: (
              <svg
                className={`w-4 h-4 shrink-0 fill-current text-slate-400 mr-2 ${
                  pathname.includes('/settings/feedback') && 'text-indigo-400'
                }`}
                viewBox='0 0 16 16'
              >
                <path d='M7.001 3h2v4h-2V3zm1 7a1 1 0 110-2 1 1 0 010 2zM15 16a1 1 0 01-.6-.2L10.667 13H1a1 1 0 01-1-1V1a1 1 0 011-1h14a1 1 0 011 1v14a1 1 0 01-1 1zM2 11h9a1 1 0 01.6.2L14 13V2H2v9z' />
              </svg>
            ),
          },
        ],
      },
    ],
    [pathname]
  );

  return (
    <div className='flex flex-nowrap overflow-x-scroll no-scrollbar md:block md:overflow-auto px-3 py-6 border-b md:border-b-0 md:border-r border-slate-200 min-w-60 md:space-y-3'>
      {sidebarGroups.map(({ title, items }, i) => (
        <div key={i}>
          <div className='text-xs font-semibold text-slate-400 uppercase mb-3'>
            {title}
          </div>
          <ul className='flex flex-nowrap md:block mr-3 md:mr-0'>
            {items.map(({ title, to, icon }, j) => (
              <li key={j} className='mr-0.5 md:mr-0 md:mb-0.5'>
                <NavLink
                  end
                  to={to}
                  className={`flex items-center px-2.5 py-2 rounded whitespace-nowrap ${
                    pathname.includes(to) && 'bg-indigo-50'
                  }`}
                >
                  {icon}
                  <span
                    className={`text-sm font-medium ${
                      pathname.includes(to)
                        ? 'text-indigo-500'
                        : 'hover:text-dark'
                    }`}
                  >
                    {title}
                  </span>
                </NavLink>
              </li>
            ))}
          </ul>
        </div>
      ))}
    </div>
  );
};

export default React.memo(SettingsSidebar);
