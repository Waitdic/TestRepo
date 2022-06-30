import React from 'react';
//
import Main from '@/layouts/Main';
import { PaginationClassic } from '@/components';

type Props = {};

const ChangeLog: React.FC<Props> = ({}) => {
  return (
    <Main>
      <>
        {/* Page header */}
        <div className='sm:flex sm:justify-between sm:items-center mb-8'>
          {/* Left: Title */}
          <div className='mb-4 sm:mb-0'>
            <h1 className='text-2xl md:text-3xl text-slate-800 font-bold'>
              Changelog
            </h1>
          </div>
        </div>

        <div className='border-t border-slate-200'>
          <div className='max-w-3xl m-auto mt-8'>
            {/* Filters */}
            <div className='xl:pl-32 xl:-translate-x-16 mb-2'>
              <ul className='flex flex-wrap -m-1'>
                <li className='m-1'>
                  <button className='inline-flex items-center justify-center text-sm font-medium leading-5 rounded-full px-3 py-1 border border-transparent shadow-sm bg-primary text-white duration-150 ease-in-out'>
                    View All
                  </button>
                </li>
                <li className='m-1'>
                  <button className='inline-flex items-center justify-center text-sm font-medium leading-5 rounded-full px-3 py-1 border border-slate-200 hover:border-slate-300 shadow-sm bg-white text-slate-500 duration-150 ease-in-out'>
                    Announcements
                  </button>
                </li>
                <li className='m-1'>
                  <button className='inline-flex items-center justify-center text-sm font-medium leading-5 rounded-full px-3 py-1 border border-slate-200 hover:border-slate-300 shadow-sm bg-white text-slate-500 duration-150 ease-in-out'>
                    Bug Fix
                  </button>
                </li>
                <li className='m-1'>
                  <button className='inline-flex items-center justify-center text-sm font-medium leading-5 rounded-full px-3 py-1 border border-slate-200 hover:border-slate-300 shadow-sm bg-white text-slate-500 duration-150 ease-in-out'>
                    Product
                  </button>
                </li>
              </ul>
            </div>

            {/* Posts */}
            <div className='xl:-translate-x-16'>
              {/* Post */}
              <article className='pt-6'>
                <div className='xl:flex'>
                  <div className='w-32 shrink-0'>
                    <div className='text-xs font-semibold uppercase text-slate-400 xl:leading-8'>
                      8 July, 2021
                    </div>
                  </div>
                  <div className='grow pb-6 border-b border-slate-200'>
                    <header>
                      <h2 className='text-2xl text-slate-800 font-bold mb-3'>
                        Released version 2.0
                      </h2>
                      <div className='flex flex-nowrap items-center space-x-2 mb-4'>
                        <div className='flex items-center'>
                          <a className='block mr-2 shrink-0' href='#0'>
                            <img
                              className='rounded-full border-2 border-white box-content'
                              src='/images/user-avatar-32.png'
                              width='32'
                              height='32'
                              alt='User 04'
                            />
                          </a>
                          <a
                            className='block text-sm font-semibold text-slate-800'
                            href='#0'
                          >
                            Simona Lürwer
                          </a>
                        </div>
                        <div className='text-slate-400'>·</div>
                        <div>
                          <div className='text-xs inline-flex font-medium bg-emerald-100 text-emerald-600 rounded-full text-center px-2.5 py-1'>
                            Product
                          </div>
                        </div>
                      </div>
                    </header>
                    <div className='space-y-3'>
                      <p>
                        Lorem ipsum dolor sit amet, consectetur adipiscing elit,
                        sed do eiusmod tempor incididunt ut labore et dolore
                        magna aliqua. Ut enim ad minim veniam, quis nostrud
                        exercitation ullamco laboris nisi ut aliquip ex ea
                        commodo consequat.
                      </p>
                      <p>
                        Duis aute irure dolor in reprehenderit in voluptate
                        velit esse cillum dolore eu fugiat nulla pariatur
                        excepteur sint occaecat cupidatat non proident.
                      </p>
                    </div>
                  </div>
                </div>
              </article>
              {/* Post */}
              <article className='pt-6'>
                <div className='xl:flex'>
                  <div className='w-32 shrink-0'>
                    <div className='text-xs font-semibold uppercase text-slate-400 xl:leading-8'>
                      6 July, 2021
                    </div>
                  </div>
                  <div className='grow pb-6 border-b border-slate-200'>
                    <header>
                      <h2 className='text-2xl text-slate-800 font-bold mb-3'>
                        Feature Name is now public 🎉
                      </h2>
                      <div className='flex flex-nowrap items-center space-x-2 mb-4'>
                        <div className='flex items-center'>
                          <a className='block mr-2 shrink-0' href='#0'>
                            <img
                              className='rounded-full border-2 border-white box-content'
                              src='/images/user-avatar-32.png'
                              width='32'
                              height='32'
                              alt='User 04'
                            />
                          </a>
                          <a
                            className='block text-sm font-semibold text-slate-800'
                            href='#0'
                          >
                            Danielle Cohen
                          </a>
                        </div>
                        <div className='text-slate-400'>·</div>
                        <div>
                          <div className='text-xs inline-flex font-medium bg-amber-100 text-amber-600 rounded-full text-center px-2.5 py-1'>
                            Announcement
                          </div>
                        </div>
                      </div>
                    </header>
                    <div className='space-y-3'>
                      <p>
                        Lorem ipsum dolor sit amet, consectetur adipiscing elit,
                        sed do eiusmod tempor incididunt ut labore et dolore
                        magna aliqua. Ut enim ad minim veniam, quis nostrud
                        exercitation ullamco laboris nisi ut aliquip ex ea
                        commodo consequat.
                      </p>
                      <p>
                        Duis aute irure dolor in reprehenderit in voluptate
                        velit esse cillum dolore eu fugiat nulla pariatur
                        excepteur sint occaecat cupidatat non proident.
                      </p>
                    </div>
                  </div>
                </div>
              </article>
              {/* Post */}
              <article className='pt-6'>
                <div className='xl:flex'>
                  <div className='w-32 shrink-0'>
                    <div className='text-xs font-semibold uppercase text-slate-400 xl:leading-8'>
                      4 July, 2021
                    </div>
                  </div>
                  <div className='grow pb-6 border-b border-slate-200'>
                    <header>
                      <h2 className='text-2xl text-slate-800 font-bold mb-3'>
                        Bugs fixed, issues, and more
                      </h2>
                      <div className='flex flex-nowrap items-center space-x-2 mb-4'>
                        <div className='flex items-center'>
                          <a className='block mr-2 shrink-0' href='#0'>
                            <img
                              className='rounded-full border-2 border-white box-content'
                              src='/images/user-avatar-32.png'
                              width='32'
                              height='32'
                              alt='User 04'
                            />
                          </a>
                          <a
                            className='block text-sm font-semibold text-slate-800'
                            href='#0'
                          >
                            Patrick Kumar
                          </a>
                        </div>
                        <div className='text-slate-400'>·</div>
                        <div>
                          <div className='text-xs inline-flex font-medium bg-rose-100 text-rose-600 rounded-full text-center px-2.5 py-1'>
                            Bug Fix
                          </div>
                        </div>
                      </div>
                    </header>
                    <div className='space-y-3'>
                      <p>
                        Lorem ipsum dolor sit amet, consectetur adipiscing elit,
                        sed do eiusmod tempor incididunt ut labore et dolore
                        magna aliqua. Ut enim ad minim veniam, quis nostrud
                        exercitation ullamco laboris nisi ut aliquip ex ea
                        commodo consequat.
                      </p>
                      <p>
                        Duis aute irure dolor in reprehenderit in voluptate
                        velit esse cillum dolore eu fugiat nulla pariatur
                        excepteur sint occaecat cupidatat non proident.
                      </p>
                      <ul className='list-disc list-inside space-y-1'>
                        <li>E-commerce: Better lorem ipsum generator.</li>
                        <li>Booking: Lorem ipsum post generator.</li>
                        <li>Retail: Better lorem ipsum generator.</li>
                        <li>Services: Better lorem ipsum generator.</li>
                      </ul>
                    </div>
                  </div>
                </div>
              </article>
            </div>

            {/* Pagination */}
            <div className='xl:pl-32 xl:-translate-x-16 mt-6'>
              <PaginationClassic />
            </div>
          </div>
        </div>
      </>
    </Main>
  );
};

export default ChangeLog;
