import classNames from 'classnames';
import { capitalize } from 'lodash';
import React, { useMemo } from 'react';

type Props = {
  data: any;
};

const MultiLevelTable: React.FC<Props> = ({ data }) => {
  console.log(data);

  const headers = useMemo(() => {
    if (!data) return [];

    const headers = Object.keys(data[0]);
    return headers;
  }, [data]);

  const getHeaderName = (header: string) => {
    if (header.toLowerCase() === 'name') return '';
    if (header.toLowerCase() === 's2b') return 'S2B';
    return capitalize(header);
  };
  const getSubHeaders = (header: string) => {
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

  const renderRows = () => {
    if (!data) return null;

    return data.map((row: any, index: number) => {
      const isEven = index % 2 === 0;
      const rowClass = classNames('flex', {
        'bg-gray-200': isEven,
      });
      const cellClass = classNames('flex-1 p-1');

      return (
        <div key={index} className={rowClass}>
          {Object.values(row).map((value, index) => {
            if (typeof value === 'object' && !!value) {
              return (
                <div key={index} className={`flex ${cellClass}`}>
                  {Object.values(value).map((subValue, index) => (
                    <div key={index} className={`text-center flex-1`}>
                      {subValue}
                    </div>
                  ))}
                </div>
              );
            }
            return (
              <div
                className={classNames(cellClass, {
                  'text-center': index !== 0,
                  'border-r-0': index === 0,
                })}
              >
                {value}
              </div>
            );
          })}
        </div>
      );
    });
  };

  return (
    <div>
      <div className='flex'>
        {headers.map((header) => (
          <div
            className={classNames(
              'flex-1 text-center border-t border-r border-b border-slate-300',
              {
                'border-t-0 border-b-0': header === 'name',
              }
            )}
          >
            <p className='p-1'>{getHeaderName(header)}</p>
            <div className='flex'>
              {getSubHeaders(header)?.map((subHeader) => (
                <div
                  key={subHeader}
                  className='flex-1 border-t border-slate-300 p-1'
                >
                  {capitalize(subHeader)}
                </div>
              ))}
            </div>
          </div>
        ))}
      </div>
      {renderRows()}
    </div>
  );
};

export default React.memo(MultiLevelTable);
