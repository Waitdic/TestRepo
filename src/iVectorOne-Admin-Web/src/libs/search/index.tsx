import {
  Datepicker,
  Select,
  TableList,
  UncontrolledTextField,
} from '@/components';
import Main from '@/layouts/Main';
import React, { useState } from 'react';

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
  const [searchDetails, setSearchDetails] = useState({
    property: '',
    arrivalDate: [
      new Date(),
      new Date(new Date().setDate(new Date().getDate() + 1)),
    ],
  });

  const handleChangeDetails = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setSearchDetails((prev) => ({ ...prev, [name]: value }));
  };

  const handleChangeArrivalDate = (date: Date[]) => {
    setSearchDetails((prev) => ({ ...prev, arrivalDate: date }));
  };

  return (
    <Main title='Search'>
      <div className='bg-white shadow-lg rounded-sm mb-8'>
        <div className='flex flex-col md:flex-row md:-mr-px gap-6 min-h-[720px]'>
          <div className='flex flex-nowrap overflow-x-scroll no-scrollbar md:block md:overflow-auto px-3 py-6 border-b md:border-b-0 md:border-r border-slate-200 min-w-[380px] md:space-y-3'>
            <div>
              <div className='text-xs font-semibold text-slate-400 uppercase mb-3'>
                Search Details
              </div>
              <div className='space-y-3'>
                <UncontrolledTextField
                  name='property'
                  label='Property'
                  placeholder='Please input a few letters of the hotel you require'
                  onChange={handleChangeDetails}
                  value={searchDetails.property}
                />
                <Datepicker
                  label='Arrival Date'
                  onChange={handleChangeArrivalDate}
                />
                <Select
                  id='duration'
                  name='duration'
                  labelText='Duration'
                  options={Array.from({ length: 21 }, (_, i) => ({
                    id: i + 1,
                    name: `${i + 1} night${i + 1 > 1 ? 's' : ''}`,
                  }))}
                  defaultValue={{
                    id: 7,
                    name: '7 nights',
                  }}
                />
                <div className='grid grid-cols-3 gap-2'>
                  <Select
                    id='adults'
                    name='adults'
                    labelText='Adults'
                    options={Array.from({ length: 6 }, (_, i) => ({
                      id: i + 1,
                      name: `${i + 1}`,
                    }))}
                    defaultValue={{
                      id: 2,
                      name: '2',
                    }}
                  />
                  <Select
                    id='children'
                    name='children'
                    labelText='Children'
                    options={Array.from({ length: 4 }, (_, i) => ({
                      id: i,
                      name: `${i}`,
                    }))}
                    defaultValue={{
                      id: 0,
                      name: '0',
                    }}
                  />
                  <Select
                    id='infants'
                    name='infants'
                    labelText='Infants'
                    options={Array.from({ length: 3 }, (_, i) => ({
                      id: i,
                      name: `${i}`,
                    }))}
                    defaultValue={{
                      id: 0,
                      name: '0',
                    }}
                  />
                </div>
              </div>
            </div>
          </div>
          <div className='pl-6 md:pl-0 py-6 pr-6 w-full'>
            <TableList
              headerList={tableHeaderList}
              bodyList={[]}
              showOnEmpty
              initText='Please input some search details and perform a search'
            />
          </div>
        </div>
      </div>
    </Main>
  );
};

export default React.memo(Search);
