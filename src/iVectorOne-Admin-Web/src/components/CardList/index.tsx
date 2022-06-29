import { FC, memo } from 'react';
import { Link } from 'react-router-dom';
import classNames from 'classnames';
//
import { DropdownEditMenu, EmptyState, Spinner } from '@/components';

type Props = {
  bodyList: {
    id: number | string;
    name: string;
    isActive?: boolean;
    actions?: {
      name: string;
      href: string;
    }[];
  }[];
  emptyState: {
    title: string;
    description: string;
    href: string;
    buttonText: string;
  };
  isLoading?: boolean;
};

const CardList: FC<Props> = ({ bodyList, emptyState, isLoading = false }) => {
  if (isLoading) {
    return (
      <div className='p-4 text-center'>
        <Spinner />
      </div>
    );
  }

  return (
    <>
      {bodyList.length > 0 ? (
        <div className='grid grid-cols-12'>
          {bodyList.map(({ id, name, isActive = undefined, actions }) => (
            <div
              key={id}
              className='col-span-full sm:col-span-6 xl:col-span-4 bg-white shadow-lg rounded-sm border border-slate-200'
            >
              <div className='flex flex-col h-full'>
                {/* Card top */}
                <div className='grow p-5'>
                  <div className='flex justify-between items-start'>
                    {/* Name */}
                    <header>
                      <h3 className='text-2xl'>{name}</h3>
                    </header>
                    {/* Menu button */}
                    <DropdownEditMenu
                      align='right'
                      className='relative inline-flex shrink-0'
                    >
                      <li>
                        <Link
                          className='font-medium text-sm text-slate-600 hover:text-slate-800 flex py-1 px-3'
                          to='#0'
                        >
                          Option 1
                        </Link>
                      </li>
                      <li>
                        <Link
                          className='font-medium text-sm text-slate-600 hover:text-slate-800 flex py-1 px-3'
                          to='#0'
                        >
                          Option 2
                        </Link>
                      </li>
                      <li>
                        <Link
                          className='font-medium text-sm text-rose-500 hover:text-rose-600 flex py-1 px-3'
                          to='#0'
                        >
                          Remove
                        </Link>
                      </li>
                    </DropdownEditMenu>
                  </div>
                </div>
                {/* Card footer */}
                <div className='border-t border-slate-200'>
                  <div className='flex divide-x divide-slate-200r'>
                    <div className='flex-1 flex justify-center items-center'>
                      <span
                        className={classNames('py-1 px-6 rounded-2xl', {
                          'bg-green-200 text-green-500': isActive,
                          'bg-gray-200 text-gray-500': !isActive,
                        })}
                      >
                        {isActive ? 'Active' : 'Inactive'}
                      </span>
                    </div>
                    <Link
                      className='block flex-1 text-center text-sm text-slate-600 hover:text-slate-800 font-medium px-3 py-4 group'
                      to={`/subscriptions/edit/${id}`}
                    >
                      <div className='flex items-center justify-center'>
                        <svg
                          className='w-4 h-4 fill-current text-slate-400 group-hover:text-slate-500 shrink-0 mr-2'
                          viewBox='0 0 16 16'
                        >
                          <path d='M11.7.3c-.4-.4-1-.4-1.4 0l-10 10c-.2.2-.3.4-.3.7v4c0 .6.4 1 1 1h4c.3 0 .5-.1.7-.3l10-10c.4-.4.4-1 0-1.4l-4-4zM4.6 14H2v-2.6l6-6L10.6 8l-6 6zM12 6.6L9.4 4 11 2.4 13.6 5 12 6.6z' />
                        </svg>
                        <span>Edit</span>
                      </div>
                    </Link>
                  </div>
                </div>
              </div>
            </div>
          ))}
        </div>
      ) : (
        <EmptyState {...emptyState} />
      )}
    </>
  );
};

export default memo(CardList);
