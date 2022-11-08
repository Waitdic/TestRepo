import React, { useMemo, useState } from 'react';
import { useSelector } from 'react-redux';
import { orderBy as _orderBy } from 'lodash';
//
import { RootState } from '@/store';
import type { SearchDetails, SupplierSearchResults } from '@/types';
import Main from '@/layouts/Main';
import { SearchFilters, TableList } from '@/components';

const tableHeaderList = [
  {
    name: 'Supplier',
    original: 'supplier',
    align: 'left',
  },
  {
    name: 'Room Code',
    original: 'roomCode',
    align: 'left',
  },
  {
    name: 'Room Type',
    original: 'roomType',
    align: 'left',
  },
  {
    name: 'Meal Basis',
    original: 'mealBasis',
    align: 'left',
  },
  {
    name: 'Currency',
    original: 'currency',
    align: 'left',
  },
  {
    name: 'Cost',
    original: 'cost',
    align: 'left',
  },
  {
    name: 'Non-Ref',
    original: 'nonRef',
    align: 'left',
  },
];

const Search: React.FC = () => {
  const isLoading = useSelector((state: RootState) => state.app.isLoading);

  const [orderBy, setOrderBy] = useState<{
    by: string | null;
    order: 'asc' | 'desc';
  }>({
    by: null,
    order: 'asc',
  });
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
    let rows: {
      id: number;
      name: string;
      items: { name: string; value: any }[];
    }[] = [];
    if (orderBy.by === null) {
      searchResults.forEach((result, idx) => {
        let items: { name: string; value: any }[] = [];
        Object.entries(result).forEach(([key, value]) => {
          items.push({ name: key, value });
        });
        const name = Object.keys(result)[0];
        rows.push({
          id: idx,
          name,
          items,
        });
      });
    } else {
      let orderByKey = orderBy.by;
      if (orderByKey === 'nonRef') {
        orderByKey = 'nonRefundable';
      } else if (orderByKey === 'cost') {
        orderByKey = 'totalCost';
      }

      const sortedResults = _orderBy(
        searchResults,
        [orderByKey],
        orderBy.order
      );

      sortedResults.forEach((result, idx) => {
        let items: { name: string; value: any }[] = [];
        Object.entries(result).forEach(([key, value]) => {
          items.push({ name: key, value });
        });
        const name = Object.keys(result)[0];
        rows.push({
          id: idx,
          name,
          items,
        });
      });
    }

    return rows;
  }, [searchResults, orderBy]);

  const handleOrderChange = (orderBy: string, order: 'asc' | 'desc') => {
    setOrderBy({ by: orderBy, order });
  };

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
                onOrderChange={handleOrderChange}
                orderBy={orderBy}
              />
            </div>
          )}
        </div>
      </div>
    </Main>
  );
};

export default React.memo(Search);
