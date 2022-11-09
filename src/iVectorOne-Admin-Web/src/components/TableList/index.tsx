import { FC, memo, useMemo, useState } from 'react';
import { Link } from 'react-router-dom';
import classNames from 'classnames';
import moment from 'moment';
//
import { Button, EmptyState, Spinner, YesOrNo } from '@/components';

export interface TableHeaderList {
  name: string;
  align: string;
  original?: string;
}
export interface TableBodyListItems {
  name: string;
  value: any;
  align?: 'right' | 'center' | 'left';
}
export interface TableListBody {
  id: number | string;
  name: string;
  items?: TableBodyListItems[];
  isActive?: boolean;
  actions?: {
    name: string;
    href?: string;
    onClick?: () => void;
  }[];
}

type BodyListPager = {
  total: number;
  current: number;
  remaining: number;
  amount: number;
  multiplier: number;
  isEnd: boolean;
};

type Props = {
  headerList: TableHeaderList[];
  bodyList: TableListBody[];
  showOnEmpty?: boolean;
  initText?: string;
  emptyState?: {
    title: string;
    description: string[];
    buttonText?: string;
    href?: string;
    onClick?: () => void;
  };
  isLoading?: boolean;
  onOrderChange?: (orderBy: string, order: 'asc' | 'desc') => void;
  orderBy?: {
    by: string | null;
    order: 'asc' | 'desc';
    only?: string[];
  };
  loadMore?: {
    amount: number;
    onClick?: (pager?: BodyListPager) => void;
  };
};

const TableList: FC<Props> = ({
  headerList,
  bodyList,
  emptyState,
  isLoading = false,
  showOnEmpty = false,
  initText,
  onOrderChange,
  orderBy,
  loadMore,
}) => {
  const [loadMoreMultiplier, setLoadMoreMultiplier] = useState(1);

  const slicedBodyList = useMemo(() => {
    if (!!loadMore) {
      return bodyList.slice(0, loadMoreMultiplier * loadMore.amount);
    }
    return bodyList;
  }, [bodyList, loadMoreMultiplier]);

  const bodyListPager = useMemo(() => {
    if (!!loadMore) {
      const total = bodyList.length;
      const current = slicedBodyList.length;
      const remaining = total - current;
      const amount = loadMore.amount;
      const multiplier = loadMoreMultiplier;
      const isEnd = remaining <= 0;

      const pager: BodyListPager = {
        total,
        current,
        remaining,
        amount,
        multiplier,
        isEnd,
      };

      loadMore?.onClick?.(pager);

      return pager;
    }
  }, [bodyList, loadMoreMultiplier]);

  const handleLoadMore = () => {
    if (!loadMore) return;
    setLoadMoreMultiplier((prev) => prev + 1);
  };

  const renderCell = (cellData: any, name: string) => {
    if (typeof cellData === 'boolean') {
      return (
        <div className='text-center'>
          <YesOrNo isActive={cellData} />
        </div>
      );
    }
    if (
      typeof cellData === 'string' &&
      (name?.includes?.('date') || name?.includes?.('timestamp'))
    ) {
      const dateStr = moment(new Date(cellData)).format('YYYY-MM-DD HH:mm:ss');
      return dateStr !== 'Invalid date' ? dateStr : cellData;
    }
    return cellData;
  };

  const handleOnOrderChange = (name: string, originalName?: string) => {
    if (!!orderBy?.only && !orderBy.only.includes(name)) {
      return;
    }
    onOrderChange?.(
      originalName || name,
      orderBy?.order === 'asc' &&
        (orderBy?.by === originalName || orderBy?.by === name)
        ? 'desc'
        : 'asc'
    );
  };

  const renderActions = (
    actions:
      | {
          name: string;
          href?: string;
          onClick?: () => void;
        }[]
      | undefined
  ) => {
    if (!actions || actions?.length === 0) return null;

    const actionClasses = classNames('text-primary hover:text-primaryHover', {
      'ml-2': actions?.length > 1,
    });

    return (
      <td className='px-6 py-4 text-right text-sm'>
        {actions.map(({ name: actionName, href, onClick }) => {
          if (!!href) {
            return (
              <Link to={href} className={actionClasses} key={href}>
                {actionName}
              </Link>
            );
          } else {
            return (
              <button
                key={actionName}
                className={actionClasses}
                onClick={() => onClick?.()}
              >
                {actionName}
              </button>
            );
          }
        })}
      </td>
    );
  };

  if (isLoading) {
    return (
      <div className='p-4 text-center'>
        <Spinner />
      </div>
    );
  }

  return (
    <>
      {slicedBodyList.length > 0 || showOnEmpty ? (
        <div className='align-middle inline-block w-full shadow'>
          <div className='overflow-x-auto overflow-y-hidden sm:rounded-lg'>
            <table className='table-auto min-w-[1100px] divide-y divide-gray-200 w-full'>
              <thead className='bg-primary'>
                <tr>
                  {headerList.length > 0 &&
                    headerList.map(({ name, align, original }, idx) => (
                      <th
                        scope='col'
                        className={classNames(
                          'px-6 py-3 text-xs font-medium text-white uppercase tracking-wider',
                          {
                            'text-left': align === 'left',
                            'text-right': align === 'right',
                            'cursor-pointer':
                              (!!onOrderChange && !orderBy?.only) ||
                              (!!orderBy?.only && orderBy.only.includes(name)),
                          }
                        )}
                        key={name}
                        onClick={() => handleOnOrderChange(name, original)}
                      >
                        <p className='relative'>
                          {name}
                          <span className='absolute top-0'>
                            {((!!onOrderChange &&
                              orderBy?.by === (original || name)) ||
                              (idx === 0 && orderBy?.by === null)) && (
                              <svg
                                className={classNames('w-4 h-4 ml-1', {
                                  'transform rotate-180':
                                    orderBy?.order === 'asc',
                                })}
                                fill='none'
                                stroke='currentColor'
                                viewBox='0 0 24 24'
                                xmlns='http://www.w3.org/2000/svg'
                              >
                                <path
                                  strokeLinecap='round'
                                  strokeLinejoin='round'
                                  strokeWidth={2}
                                  d='M5 15l7-7 7 7'
                                />
                              </svg>
                            )}
                          </span>
                        </p>
                      </th>
                    ))}
                </tr>
              </thead>
              <tbody className='bg-white divide-y divide-gray-200'>
                {showOnEmpty && slicedBodyList.length === 0 && (
                  <tr>
                    <td className='px-6 py-4 ' colSpan={7}>
                      <p className='text-sm font-medium text-dark'>
                        {initText}
                      </p>
                    </td>
                  </tr>
                )}
                {slicedBodyList.map(
                  ({ id, name, isActive = undefined, actions, items }, idx) => {
                    const isEven = idx % 2 === 0;
                    const rowClass = classNames(
                      'bg-white',
                      { 'bg-gray-50': isEven },
                      { 'bg-green-50': isActive }
                    );
                    return (
                      <tr key={id} className={rowClass}>
                        {!!items?.length ? (
                          <>
                            {items.map((item, idx) => (
                              <td
                                key={idx}
                                className={classNames(
                                  'px-6 py-4 text-sm font-medium text-dark',
                                  {
                                    'text-right': item.align === 'right',
                                    'text-center': item.align === 'center',
                                  }
                                )}
                              >
                                {renderCell(item.value, item.name)}
                              </td>
                            ))}
                            {renderActions(actions)}
                          </>
                        ) : (
                          <>
                            <td className='px-6 py-4'>
                              <div className='flex flex-col justify-center'>
                                <div className='text-sm font-medium text-dark'>
                                  <p>{name}</p>
                                </div>
                                {typeof isActive !== 'undefined' && (
                                  <div className='text-sm font-medium text-gray-500'>
                                    {isActive ? 'Active' : 'Inactive'}
                                  </div>
                                )}
                              </div>
                            </td>
                            {renderActions(actions)}
                          </>
                        )}
                      </tr>
                    );
                  }
                )}
              </tbody>
            </table>
            {!!bodyListPager && bodyListPager?.total > 0 && (
              <div
                className={classNames('flex items-center my-5 px-4', {
                  'justify-between': !bodyListPager.isEnd,
                  'justify-end': bodyListPager.isEnd,
                })}
              >
                {!bodyListPager.isEnd && (
                  <Button text='Load More' onClick={handleLoadMore} />
                )}
                <p className='text-xs text-gray-400'>
                  {bodyListPager.current} / {bodyListPager.total}
                </p>
              </div>
            )}
          </div>
        </div>
      ) : (
        <>{!!emptyState && <EmptyState {...emptyState} />}</>
      )}
    </>
  );
};

export default memo(TableList);
