import { FC, memo, useMemo } from 'react';
import { Link } from 'react-router-dom';
import classNames from 'classnames';
//
import { EmptyState, Spinner } from '@/components';

type Props = {
  bodyList: {
    id: number | string;
    name?: string;
    isActive?: boolean;
    actions?: {
      name: string;
      href: string;
    }[];
  }[];
  emptyState: {
    title: string;
    description: string[];
    href: string;
    buttonText: string;
  };
  isLoading?: boolean;
  statusIsPlaceholder?: boolean;
  cardClassNames?: string;
};

const CardList: FC<Props> = ({
  bodyList,
  emptyState,
  isLoading = false,
  statusIsPlaceholder = false,
  cardClassNames = 'col-span-full md:col-span-6 xl:col-span-4',
}) => {
  const renderStatus = (isActive: boolean) =>
    useMemo(() => (isActive ? 'Active' : 'Inactive'), [isActive]);

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
        <div className='grid grid-cols-12 gap-6'>
          {bodyList.map(({ id, name, isActive = undefined, actions }) => (
            <div
              key={id}
              className={classNames(
                'bg-white shadow-lg rounded-sm border border-slate-200',
                {
                  [cardClassNames]: cardClassNames,
                }
              )}
            >
              <div className='flex flex-col h-full'>
                {/* Card top */}
                <div className='grow p-5'>
                  <div className='flex justify-between items-start'>
                    {/* Name */}
                    <header>
                      <h3 className='text-2xl'>{name}</h3>
                    </header>
                  </div>
                </div>
                {/* Card footer */}
                <div className='border-t border-slate-200'>
                  <div className='flex divide-x divide-slate-200r'>
                    {statusIsPlaceholder ? (
                      <span className='block flex-1 text-center text-sm text-slate-600 hover:text-slate-800 font-medium px-3 py-4 group'>
                        {/* placeholder */}
                      </span>
                    ) : (
                      <div className='flex-1 flex justify-center items-center'>
                        <span
                          className={classNames('py-1 px-6 rounded-2xl', {
                            'bg-green-200 text-green-500': isActive,
                            'bg-gray-200 text-gray-500': !isActive,
                          })}
                        >
                          {renderStatus(!!isActive)}
                        </span>
                      </div>
                    )}
                    <Link
                      className='block flex-1 text-center text-sm text-slate-600 hover:text-slate-800 font-medium px-3 py-4 group'
                      to={actions?.[0]?.href || ''}
                    >
                      <div className='flex items-center justify-center'>
                        <span>{actions?.[0]?.name}</span>
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
