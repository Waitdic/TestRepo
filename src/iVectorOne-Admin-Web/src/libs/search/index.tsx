import React, { useMemo, useState } from 'react';
import { useSelector } from 'react-redux';
//
import { RootState } from '@/store';
import type { SearchDetails, SupplierSearchResults } from '@/types';
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

  const [searchResults, setSearchResults] = useState<SupplierSearchResults[]>(
    []
  );
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

  const tableBody = useMemo(() => {
    let rows: { id: number; name: string; items: string[] }[] = [];
    searchResults.forEach((result, idx) => {
      let items: any[] = [];
      Object.entries(result).forEach(([key, value], idx) => {
        items.push(value);
      });
      rows.push({
        id: idx,
        name: Object.keys(result)[idx],
        items,
      });
    });
    return rows;
  }, [searchResults]);

  return (
    <Main title='Search Tester'>
      <div className='bg-white shadow-lg rounded-sm mb-8'>
        <div className='flex flex-col gap-2 min-h-[720px]'>
          <SearchFilters
            searchDetails={searchDetails}
            setSearchDetails={setSearchDetails}
            setSearchResults={setSearchResults}
          />
          {isLoading && (
            <div className='text-center text-lg pb-4 px-4'>
              <p className='animate-pulse'>Searching...</p>
            </div>
          )}
          {searchDetails.isActive && !isLoading && (
            <div className='p-4 w-full'>
              <TableList
                headerList={tableHeaderList}
                bodyList={tableBody}
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
