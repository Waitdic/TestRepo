import classNames from 'classnames';
import React from 'react';

type Props = {
  postPerPage: number;
  resultCount: number;
  currentPageCount: number;
  setCurrentPageCount: React.Dispatch<React.SetStateAction<number>>;
};

const PaginationClassic: React.FC<Props> = ({
  currentPageCount,
  postPerPage,
  resultCount,
  setCurrentPageCount,
}) => {
  const handleIncrementPageCount = () => {
    if (currentPageCount === Math.ceil(resultCount / postPerPage) - 1) return;
    setCurrentPageCount(currentPageCount + 1);
  };

  const handleDecrementPageCount = () => {
    setCurrentPageCount(currentPageCount - 1);
  };

  return (
    <div className='flex flex-col sm:flex-row sm:items-center sm:justify-between'>
      <nav
        className='mb-4 sm:mb-0 sm:order-1'
        role='navigation'
        aria-label='Navigation'
      >
        <ul className='flex justify-center'>
          <li className='ml-3 first:ml-0'>
            <button
              className={classNames('btn bg-white border-slate-200', {
                'text-slate-300 cursor-not-allowed': currentPageCount === 0,
                'hover:border-slate-300 text-primary': currentPageCount > 0,
              })}
              disabled={currentPageCount === 0}
              onClick={handleDecrementPageCount}
            >
              &lt;- Previous
            </button>
          </li>
          <li className='ml-3 first:ml-0'>
            <button
              className={classNames('btn bg-white border-slate-200', {
                'text-slate-300 cursor-not-allowed':
                  currentPageCount === Math.ceil(resultCount / postPerPage) - 1,
                'hover:border-slate-300 text-primary':
                  currentPageCount < Math.ceil(resultCount / postPerPage) - 1,
              })}
              onClick={handleIncrementPageCount}
            >
              Next -&gt;
            </button>
          </li>
        </ul>
      </nav>
      <div className='text-sm text-slate-500 text-center sm:text-left'>
        Showing{' '}
        <span className='font-medium textDark'>
          {currentPageCount * postPerPage === 0
            ? 1
            : currentPageCount * postPerPage}
        </span>{' '}
        to{' '}
        <span className='font-medium textDark'>
          {resultCount < currentPageCount * postPerPage + postPerPage
            ? resultCount
            : currentPageCount * postPerPage + postPerPage}
        </span>{' '}
        of <span className='font-medium textDark'>{resultCount}</span> results
      </div>
    </div>
  );
};

export default React.memo(PaginationClassic);
