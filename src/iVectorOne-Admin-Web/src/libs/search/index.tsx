import React, { useState } from 'react';
import { useSelector } from 'react-redux';
//
import { RootState } from '@/store';
import type { SearchDetails } from '@/types';
import Main from '@/layouts/Main';
import { SearchFilters, TableList } from '@/components';

const tableHeaderList = [
  {
    name: 'Supplier',
    align: 'left',
  },
  {
    name: 'Room Code',
    align: 'left',
  },
  {
    name: 'Room Type',
    align: 'left',
  },
  {
    name: 'Meal Basis',
    align: 'left',
  },
  {
    name: 'Currency',
    align: 'left',
  },
  {
    name: 'Cost',
    align: 'left',
  },
  {
    name: 'Non-Ref',
    align: 'left',
  },
];

const Search: React.FC = () => {
  const isLoading = useSelector((state: RootState) => state.app.isLoading);

  const [searchDetails, setSearchDetails] = useState<SearchDetails>({
    properties: [],
    property: {
      propertyId: 0,
      name: '',
    },
    arrivalDate: new Date(new Date().setDate(new Date().getDate() + 1)),
    accountId: -1,
    duration: 7,
    adults: 2,
    children: 0,
    childrenAges: [],
    infants: 0,
    isActive: false,
  });

  return (
    <Main title='Search Tester'>
      <div className='bg-white shadow-lg rounded-sm mb-8'>
        <div className='flex flex-col gap-2 min-h-[720px]'>
          <SearchFilters
            searchDetails={searchDetails}
            setSearchDetails={setSearchDetails}
          />
          {isLoading && (
            <div className='text-center text-sm pb-4 px-4'>
              <p className='animate-pulse'>Searching...</p>
            </div>
          )}
          {searchDetails.isActive && (
            <div className='p-4 w-full'>
              <TableList
                headerList={tableHeaderList}
                bodyList={[]}
                showOnEmpty
                initText='Please input some search details and perform a search'
              />
            </div>
          )}
        </div>
      </div>
    </Main>
  );
};

export default React.memo(Search);
