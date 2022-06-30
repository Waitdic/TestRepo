import React from 'react';

type Props = {};

const PaginationClassic: React.FC<Props> = ({}) => {
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
              className='btn bg-white border-slate-200 text-slate-300 cursor-not-allowed'
              disabled
            >
              &lt;- Previous
            </button>
          </li>
          <li className='ml-3 first:ml-0'>
            <button className='btn bg-white border-slate-200 hover:border-slate-300 text-primary'>
              Next -&gt;
            </button>
          </li>
        </ul>
      </nav>
      <div className='text-sm text-slate-500 text-center sm:text-left'>
        Showing <span className='font-medium text-slate-600'>1</span> to{' '}
        <span className='font-medium text-slate-600'>10</span> of{' '}
        <span className='font-medium text-slate-600'>467</span> results
      </div>
    </div>
  );
};

export default React.memo(PaginationClassic);
