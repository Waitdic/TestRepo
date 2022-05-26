import { FC, memo } from 'react';
import { Link } from 'react-router-dom';
import classnames from 'classnames';
//
import { EmptyState, Spinner } from '@/components';

type Props = {
  headerList: {
    name: string;
    align: string;
  }[];
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

const TableList: FC<Props> = ({
  headerList,
  bodyList,
  emptyState,
  isLoading = false,
}) => {
  if (isLoading) {
    return (
      <div className='p-4 text-center'>
        <Spinner />
      </div>
    );
  }

  return (
    // eslint-disable-next-line react/jsx-no-useless-fragment
    <>
      {bodyList.length > 0 ? (
        <div className='align-middle inline-block w-full shadow'>
          <div className='overflow-x-auto overflow-y-hidden sm:rounded-lg'>
            <table className='divide-y divide-gray-200 w-full'>
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
                {bodyList.map(({ id, name, isActive = undefined, actions }) => (
                  <tr key={id}>
                    <td className='px-6 py-4 whitespace-nowrap'>
                      <div className='flex flex-col justify-center'>
                        <div className='text-sm font-medium text-gray-700'>
                          {name}
                        </div>
                        {typeof isActive !== 'undefined' && (
                          <div className='text-sm font-medium text-gray-500'>
                            {isActive ? 'Active' : 'Inactive'}
                          </div>
                        )}
                      </div>
                    </td>
                    <td className='px-6 py-4 text-right whitespace-nowrap text-sm'>
                      {actions &&
                        actions.length > 0 &&
                        actions.map(({ name, href }) => (
                          <Link
                            to={href}
                            className='text-primary hover:text-primaryHover'
                            key={href}
                          >
                            {name}
                          </Link>
                        ))}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      ) : (
        <EmptyState {...emptyState} />
      )}
    </>
  );
};

export default memo(TableList);
