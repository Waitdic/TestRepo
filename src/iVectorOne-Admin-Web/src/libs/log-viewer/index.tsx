import React, { useMemo, useState } from 'react';
import { useSelector } from 'react-redux';
import { orderBy as _orderBy } from 'lodash';
//
import type { LogEntries } from '@/types';
import type { TableBodyListItems } from 'src/components/TableList';
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
    name: 'Success',
    align: 'left',
  },
  {
    name: 'Resp Time',
    align: 'right',
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
  const [orderBy, setOrderBy] = useState<{
    by: string | null;
    order: 'asc' | 'desc';
    only: string[];
  }>({
    by: 'Date and Time',
    order: 'asc',
    only: ['Date and Time'],
  });

  const tableBody = useMemo(() => {
    let rows: {
      id: number;
      name: string;
      items: TableBodyListItems[];
    }[] = [];
    let orderedResults = results;
    if (orderBy.by === 'Date and Time') {
      orderedResults = _orderBy(results, 'timestamp', [orderBy.order]);
    }
    orderedResults.forEach((result, idx) => {
      let items: any[] = [];
      Object.entries(result).forEach(([key, value]) => {
        if (key === 'responseTime') {
          items.push({ name: key, value: value, align: 'right' });
        } else {
          items.push({ name: key, value: value });
        }
      });
      rows.push({
        id: idx,
        name: Object.keys(result)[idx],
        items,
      });
    });
    return rows;
  }, [results, orderBy]);

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
                orderBy={orderBy as any}
                onOrderChange={(by, order) =>
                  setOrderBy((prev) => ({ ...prev, by, order }))
                }
                loadMore={{
                  amount: 50,
                }}
              />
            </div>
          )}
        </div>
      </div>
    </Main>
  );
};

export default React.memo(LogViewer);
