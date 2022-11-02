import React, { useMemo, useState } from 'react';
import { useSelector } from 'react-redux';
//
import { RootState } from '@/store';
import type { SearchDetails, SupplierSearchResults } from '@/types';
import Main from '@/layouts/Main';
import { LogFilters, SearchFilters, TableList } from '@/components';
import ApiCall from '@/axios';

const tableHeaderList = [
  {
    name: 'Date and Time',
    align: 'left',
  },
  {
    name: 'Supplier',
    align: 'left',
  },
  {
    name: 'Type',
    align: 'left',
  },
  {
    name: 'Success',
    align: 'left',
  },
  {
    name: 'Resp Time',
    align: 'left',
  },
  {
    name: 'Supplier Ref',
    align: 'left',
  },
  {
    name: 'Lead Guest',
    align: 'left',
  },
  {
    name: 'Req',
    align: 'left',
  },
  {
    name: 'Resp',
    align: 'left',
  },
];

const LogViewer: React.FC = () => {
  const isLoading = useSelector((state: RootState) => state.app.isLoading);

  const tableBody = useMemo(() => {
    let rows: { id: number; name: string; items: string[] }[] = [];
    [].forEach((result, idx) => {
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
  }, []);

  return (
    <Main title='Log Viewer'>
      <div className='bg-white shadow-lg rounded-sm mb-8'>
        <div className='flex flex-col gap-2 min-h-[720px]'>
          <LogFilters
            filters={{}}
            setFilters={function (value: any): void {
              throw new Error('Function not implemented.');
            }}
            setResults={function (value: React.SetStateAction<any[]>): void {
              throw new Error('Function not implemented.');
            }}
          />
          {isLoading && (
            <div className='text-center text-sm pb-4 px-4'>
              <p className='animate-pulse'>Searching...</p>
            </div>
          )}
          {
            <div className='p-4 w-full'>
              <TableList
                headerList={tableHeaderList}
                bodyList={tableBody}
                showOnEmpty
                initText='Please input some search details and perform a search'
              />
            </div>
          }
        </div>
      </div>
    </Main>
  );
};

export default React.memo(LogViewer);
