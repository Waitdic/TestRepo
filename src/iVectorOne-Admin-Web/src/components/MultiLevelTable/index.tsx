import classNames from 'classnames';
import { capitalize } from 'lodash';
import React, { useMemo } from 'react';
import { v4 as uuid } from 'uuid';
//
import type { MultiLevelTableData } from '@/types';

type Props = {
  data: MultiLevelTableData[] | null;
};

const headerColors = [
  'bg-blue-500',
  'bg-green-500',
  'bg-yellow-500',
  'bg-red-500',
  'bg-indigo-500',
  'bg-pink-500',
  'bg-purple-500',
  'bg-gray-500',
  'bg-orange-500',
];

const MultiLevelTable: React.FC<Props> = ({ data }) => {
  const headers = useMemo(() => {
    if (!data?.length) return [];
    const headers = Object?.keys(data[0]);
    return headers;
  }, [data]);

  const getHeaderName = (header: string) => {
    if (header.toLowerCase() === 'name') return '';
    if (header.toLowerCase() === 's2b') return 'S2B';
    return capitalize(header);
  };
  const getSubHeaders = (header: string) => {
    if (!data) return [];
    const subHeaders: string[] = [];
    data.forEach((d: any) => {
      if (typeof d[header] === 'object') {
        Object.keys(d[header]).forEach((subHeader) => {
          if (!subHeaders.includes(subHeader)) subHeaders.push(subHeader);
        });
      }
    });
    return subHeaders;
  };

  return (
    <div>
      <div className='flex'>
        {headers?.map((header, idx) => {
          if (header === 'queryDate') return null;
          return (
            <div
              key={idx}
              className={classNames(
                `flex-1 text-center border-t border-b text-white border-slate-300 ${
                  idx !== 0 && headerColors[idx]
                }`,
                {
                  'border-t-0': idx === 0,
                  'border-l': idx !== 0,
                  'border-r': idx === headers.length - 1,
                }
              )}
            >
              <p
                className={classNames(`p-1 border-slate-300`, {
                  'border-b': idx !== 0,
                })}
              >
                {getHeaderName(header)}
              </p>
              <div className='flex bg-gray-500 bg-opacity-50'>
                {getSubHeaders(header)?.map((subHeader) => (
                  <div key={subHeader} className='flex-1 text-sm p-1'>
                    {capitalize(subHeader)}
                  </div>
                ))}
              </div>
            </div>
          );
        })}
      </div>
      {data?.map((row: any, index: number) => {
        const rowId = uuid();
        const isEven = index % 2 === 0;
        const rowClass = classNames(
          'flex border-l border-r border-b border-slate-300',
          {
            'bg-gray-200': isEven,
          }
        );
        const cellClass = classNames(
          'flex-1 px-[2px] border-r border-slate-300'
        );

        return (
          <div key={rowId} className={rowClass}>
            {Object.values(row).map((value, index) => {
              const rowKey = Object.keys(row)[index];
              if (rowKey === 'queryDate') {
                return null;
              }

              const cellId = uuid();
              if (typeof value === 'object' && !!value) {
                return (
                  <div key={cellId} className={`flex ${cellClass}`}>
                    {Object.values(value).map((subValue, index) => (
                      <div
                        key={rowId + cellId + index}
                        className={classNames('text-right flex-1', {
                          'border-r border-slate-300':
                            index !== Object.values(value).length - 1,
                        })}
                      >
                        <p className='p-1'>{subValue}</p>
                      </div>
                    ))}
                  </div>
                );
              }
              return (
                <div
                  key={cellId}
                  className={classNames(cellClass, {
                    'text-right border-r border-slate-300': index !== 0,
                    'bg-yellow-200': index === 0,
                  })}
                >
                  <p className='p-1'>{value}</p>
                </div>
              );
            })}
          </div>
        );
      })}
    </div>
  );
};

export default React.memo(MultiLevelTable);
