import { FC, memo } from 'react';
import { Link } from 'react-router-dom';
import classnames from 'classnames';
import moment from 'moment';
//
import { EmptyState, Spinner, YesOrNo } from '@/components';

type Props = {
  headerList: {
    name: string;
    align: string;
  }[];
  bodyList: {
    id: number | string;
    name: string;
    items?: any[];
    isActive?: boolean;
    actions?: {
      name: string;
      href?: string;
      onClick?: () => void;
    }[];
  }[];
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
};

const TableList: FC<Props> = ({
  headerList,
  bodyList,
  emptyState,
  isLoading = false,
  showOnEmpty = false,
  initText,
}) => {
  const renderCell = (cellData: any) => {
    if (typeof cellData === 'boolean') {
      return (
        <div className='text-center'>
          <YesOrNo isActive={cellData} />
        </div>
      );
    }
    if (typeof cellData === 'string' && cellData.includes('T')) {
      return moment(new Date(cellData)).format('YYYY-MM-DD HH:mm:ss');
    }
    return cellData;
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
      {bodyList.length > 0 || showOnEmpty ? (
        <div className='align-middle inline-block w-full shadow'>
          <div className='overflow-x-auto overflow-y-hidden sm:rounded-lg'>
            <table className='table-auto min-w-[1100px] divide-y divide-gray-200 w-full'>
              <thead className='bg-gray-50'>
                <tr>
                  {headerList.length > 0 &&
                    headerList.map(({ name, align }) => (
                      <th
                        scope='col'
                        className={classnames(
                          'px-6 py-3 text-xs font-medium text-gray-500 uppercase tracking-wider',
                          {
                            'text-left': align === 'left',
                            'text-right': align === 'right',
                          }
                        )}
                        key={name}
                      >
                        {name}
                      </th>
                    ))}
                </tr>
              </thead>
              <tbody className='bg-white divide-y divide-gray-200'>
                {showOnEmpty && bodyList.length === 0 && (
                  <tr>
                    <td className='px-6 py-4 ' colSpan={7}>
                      <p className='text-sm font-medium text-dark'>
                        {initText}
                      </p>
                    </td>
                  </tr>
                )}
                {bodyList.map(
                  ({ id, name, isActive = undefined, actions, items }, idx) => {
                    const isEven = idx % 2 === 0;
                    const rowClass = classnames(
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
                                className='px-6 py-4 text-sm font-medium text-dark'
                              >
                                {renderCell(item)}
                              </td>
                            ))}
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
                            <td className='px-6 py-4 text-right text-sm'>
                              {actions &&
                                actions.length > 0 &&
                                actions.map(
                                  ({ name: actionName, href, onClick }) => {
                                    if (!!href) {
                                      return (
                                        <Link
                                          to={href}
                                          className='text-primary hover:text-primaryHover'
                                          key={href}
                                        >
                                          {actionName}
                                        </Link>
                                      );
                                    } else {
                                      return (
                                        <button
                                          key={actionName}
                                          className='text-red-400 hover:text-primaryHover'
                                          onClick={() => onClick?.()}
                                        >
                                          {actionName}
                                        </button>
                                      );
                                    }
                                  }
                                )}
                            </td>
                          </>
                        )}
                      </tr>
                    );
                  }
                )}
              </tbody>
            </table>
          </div>
        </div>
      ) : (
        <>{!!emptyState && <EmptyState {...emptyState} />}</>
      )}
    </>
  );
};

export default memo(TableList);
