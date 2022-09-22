import { FC, memo, useMemo } from 'react';
import { Link } from 'react-router-dom';
import classNames from 'classnames';
//
import { DropdownEditMenu, EmptyState, Spinner } from '@/components';

type Props = {
  bodyList: {
    id: number | string;
    name?: string;
    isActive?: boolean;
    actions?: {
      name: string;
      href?: string;
      onClick?: () => void;
      onToggle?: (id: number, isActive: boolean) => Promise<void>;
    }[];
  }[];
  emptyState: {
    title: string;
    description: string[];
    href: string;
    buttonText: string;
  };
  isLoading?: boolean;
  cardClassNames?: string;
};

const CardList: FC<Props> = ({
  bodyList,
  emptyState,
  isLoading = false,
  cardClassNames = 'col-span-full md:col-span-6 xl:col-span-4',
}) => {
  if (isLoading) {
    return (
      <div className='p-4 text-center'>
        <Spinner />
      </div>
    );
  }

  return (
    <>
      {bodyList?.length > 0 ? (
        <div className='grid grid-cols-12 gap-6'>
          {bodyList?.map(({ id, name, isActive = undefined, actions }) => (
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
                  <div className='flex flex-wrap justify-between items-start'>
                    {/* Name */}
                    <header>
                      <h3 className='text-xl xl:text-lg 2xl:text-2xl'>
                        {name}
                      </h3>
                    </header>
                    <DropdownEditMenu align='right' className='relative'>
                      {actions?.map(
                        ({ name: actionName, href, onClick, onToggle }) => {
                          if (!!href) {
                            return (
                              <li key={actionName}>
                                <Link
                                  className='font-medium text-sm text-dark hover:opacity-80 flex py-1 px-3'
                                  to={href}
                                >
                                  <div className='flex items-center justify-center'>
                                    <span>{actionName}</span>
                                  </div>
                                </Link>
                              </li>
                            );
                          } else if (!!onClick || !!onToggle) {
                            return (
                              <li
                                key={actionName}
                                className='font-medium text-sm text-dark hover:opacity-80 flex py-1 px-3 cursor-pointer'
                                onClick={() => {
                                  if (!!onClick) {
                                    onClick();
                                  } else if (!!onToggle) {
                                    onToggle(Number(id), isActive as boolean);
                                  }
                                }}
                              >
                                {actionName}
                              </li>
                            );
                          }
                        }
                      )}
                    </DropdownEditMenu>
                    <div className='flex basis-full mt-14 justify-end'>
                      <span
                        className={classNames('py-1 px-4 rounded-2xl', {
                          'bg-green-200 text-green-500': !!isActive,
                          'bg-red-200 text-red-500': isActive === false,
                        })}
                      >
                        {isActive === false ? 'Inactive' : 'Active'}
                      </span>
                    </div>
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
