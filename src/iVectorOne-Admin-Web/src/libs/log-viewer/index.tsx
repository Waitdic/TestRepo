import React, { useMemo, useState } from 'react';
import { useSelector } from 'react-redux';
//
import type { LogEntries } from '@/types';
import { RootState } from '@/store';
import Main from '@/layouts/Main';
import { LogFilters, TableList } from '@/components';

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
];

const LogViewer: React.FC = () => {
  const isLoading = useSelector((state: RootState) => state.app.isLoading);

  const [results, setResults] = useState<LogEntries[]>([]);

  const tableBody = useMemo(() => {
    let rows: { id: number; name: string; items: string[] }[] = [];
    results.forEach((result, idx) => {
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
  }, [results]);

  return (
    <Main title='Log Viewer'>
      <div className='bg-white shadow-lg rounded-sm mb-8'>
        <div className='flex flex-col gap-2 min-h-[720px]'>
          <LogFilters setResults={setResults} />
          {isLoading && (
            <div className='text-center text-sm pb-4 px-4'>
              <p className='animate-pulse'>Searching...</p>
            </div>
          )}
          {!isLoading && (
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

export default React.memo(LogViewer);