import React, { useMemo, useState } from 'react';
import classNames from 'classnames';
//
import { ChangeLogFilterTypes } from '@/constants';
import Main from '@/layouts/Main';
import { PaginationClassic } from '@/components';
import ChangeLogData from '../../../../static-json/change-log.json';

type Props = {};

const POST_PER_PAGE = 3;
const INITIAL_FILTERS = [
  {
    name: ChangeLogFilterTypes.ALL,
    isActive: true,
  },
  {
    name: ChangeLogFilterTypes.ANNOUNCEMENT,
    isActive: false,
  },
  {
    name: ChangeLogFilterTypes.BUG_FIX,
    isActive: false,
  },
  {
    name: ChangeLogFilterTypes.PRODUCT,
    isActive: false,
  },
];

const ChangeLog: React.FC<Props> = () => {
  const changeLogData = ChangeLogData.data;

  const [filteredChangeLogData, setFilteredChangeLogData] =
    useState(changeLogData);
  const [filters, setFilters] = useState(INITIAL_FILTERS);
  const [currentPageCount, setCurrentPageCount] = useState(0);

  const handleChangeFilter = (name: string) => {
    const newFilters = filters.map((filter) => ({
      ...filter,
      isActive: filter.name === name,
    }));
    setFilters(newFilters);
    if (name === ChangeLogFilterTypes.ALL) {
      setFilteredChangeLogData(changeLogData);
    } else {
      setFilteredChangeLogData(
        changeLogData.filter((log) => log.category === name)
      );
    }
  };

  const slicedChangeLogData = useMemo(() => {
    const start = currentPageCount * POST_PER_PAGE;
    const end = start + POST_PER_PAGE;
    return filteredChangeLogData.slice(start, end);
  }, [currentPageCount, filteredChangeLogData]);

  return (
    <Main title='Change Log'>
      <div className='border-t border-slate-200'>
        <div className='max-w-3xl m-auto mt-8'>
          {/* Filters */}
          <div className='xl:pl-32 xl:-translate-x-16 mb-2'>
            <ul className='flex flex-wrap -m-1'>
              {filters.map((filter, index) => (
                <li
                  key={index}
                  className='m-1'
                  onClick={() => handleChangeFilter(filter.name)}
                >
                  <button
                    className={classNames(
                      'inline-flex items-center justify-center text-sm font-medium leading-5 rounded-full px-3 py-1 border shadow-sm duration-150 ease-in-out',
                      {
                        'border-slate-200 hover:border-slate-300 bg-white text-slate-500':
                          !filter.isActive,
                        'border-transparent bg-primary text-white':
                          filter.isActive,
                      }
                    )}
                  >
                    {filter.name}
                  </button>
                </li>
              ))}
            </ul>
          </div>
          {/* Posts */}
          <div className='xl:-translate-x-16'>
            {slicedChangeLogData.map((item, index) => (
              <article key={index} className='pt-6'>
                <div className='xl:flex'>
                  <div className='w-32 shrink-0'>
                    <div className='text-xs font-semibold uppercase text-slate-400 xl:leading-8'>
                      {item.date}
                    </div>
                  </div>
                  <div className='grow pb-6 border-b border-slate-200'>
                    <header>
                      <h2 className='text-2xl text-slate-800 font-bold mb-3'>
                        {item.title}
                      </h2>
                      <div className='flex flex-nowrap items-center space-x-2 mb-4'>
                        <div className='flex items-center'>
                          <a
                            className='block text-sm font-semibold text-slate-800'
                            href='#0'
                          >
                            {item.author}
                          </a>
                        </div>
                        <div className='text-slate-400'>·</div>
                        <div>
                          <div
                            className={classNames(
                              'text-xs inline-flex font-medium bg-emerald-100 text-emerald-600 rounded-full text-center px-2.5 py-1',
                              {
                                'bg-emerald-100 text-emerald-600':
                                  item.category === 'Product',
                                'bg-amber-100 text-amber-600':
                                  item.category === 'Announcement',
                                'bg-rose-100 text-rose-600':
                                  item.category === 'Bug Fix',
                              }
                            )}
                          >
                            {item.category}
                          </div>
                        </div>
                      </div>
                    </header>
                    <div className='space-y-3 text-slate-800'>
                      {item.description.map((desc, idx) => (
                        <p key={idx}>{desc}</p>
                      ))}
                    </div>
                  </div>
                </div>
              </article>
            ))}
          </div>
          {/* Pagination */}
          <div className='xl:pl-32 xl:-translate-x-16 mt-6'>
            <PaginationClassic
              postPerPage={POST_PER_PAGE}
              resultCount={filteredChangeLogData.length}
              currentPageCount={currentPageCount}
              setCurrentPageCount={setCurrentPageCount}
            />
          </div>
        </div>
      </div>
    </Main>
  );
};

export default ChangeLog;
