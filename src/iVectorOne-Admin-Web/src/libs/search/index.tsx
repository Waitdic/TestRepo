import { EmptyState, UncontrolledTextField } from '@/components';
import Main from '@/layouts/Main';
import React, { useState } from 'react';

const tableEmptyState = {
  title: 'No Search Results',
  description: ['No results found for your search.'],
};

const Search: React.FC = () => {
  const [searchDetails, setSearchDetails] = useState({
    property: '',
  });

  const handleChangeDetails = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setSearchDetails((prev) => ({ ...prev, [name]: value }));
  };

  return (
    <Main title='Search'>
      <div className='bg-white shadow-lg rounded-sm mb-8'>
        <div className='flex flex-col md:flex-row md:-mr-px gap-6'>
          <div className='flex flex-nowrap overflow-x-scroll no-scrollbar md:block md:overflow-auto px-3 py-6 border-b md:border-b-0 md:border-r border-slate-200 min-w-[380px] md:space-y-3'>
            <div>
              <div className='text-xs font-semibold text-slate-400 uppercase mb-3'>
                Search Details
              </div>
              <div>
                <UncontrolledTextField
                  name='property'
                  label='Property'
                  placeholder='Please input a few letters of the hotel you require'
                  onChange={handleChangeDetails}
                  value={searchDetails.property}
                />
              </div>
            </div>
          </div>
          <div className='pl-6 md:pl-0 py-6 pr-6 w-full'>
            <EmptyState {...tableEmptyState} />
          </div>
        </div>
      </div>
    </Main>
  );
};

export default React.memo(Search);
