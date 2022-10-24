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
    arrivalDate: new Date(new Date().setDate(new Date().getDate() + 1)),
    account: '',
    duration: 1,
    adults: 2,
    children: 0,
    childrenAges: [0],
    infants: 0,
  });

  const handleChangeDetails = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setSearchDetails((prev) => ({ ...prev, [name]: value }));
  };

  const handleChangeArrivalDate = (date: Date[] | Date) => {
    setSearchDetails((prev) => ({ ...prev, arrivalDate: date as Date }));
  };

  const handleQuestsChange = (name: string, value: number) => {
    if (name === 'children') {
      const childrenCount = Number(value);
      if (searchDetails.childrenAges.length !== childrenCount) {
        setSearchDetails((prev) => ({
          ...prev,
          childrenAges: Array.from({ length: childrenCount }, (_) => {
            return 1;
          }),
        }));
      }
    }
    setSearchDetails((prev) => ({ ...prev, [name]: value }));
  };

  const handleChangeChildrenAges = (index: number, count: number) => {
    const childrenAges = [...searchDetails.childrenAges];
    childrenAges[index] = count;
    setSearchDetails((prev) => ({ ...prev, childrenAges }));
  };

  return (
    <Main title='Search'>
      <div className='bg-white shadow-lg rounded-sm mb-8'>
        <div className='flex flex-col gap-2 min-h-[720px]'>
          <div className='flex flex-nowrap no-scrollbar md:block px-3 py-6 border-b md:border-b-0 md:border-r border-slate-200 min-w-[380px] md:space-y-3'>
            <div>
              <div className='text-xs font-semibold text-slate-400 uppercase mb-3'>
                Search Details
              </div>
              <div className='grid grid-cols-6 gap-3'>
                <div className='col-span-1'>
                  <Select
                    id='account'
                    name='account'
                    labelText='Account'
                    options={[]}
                    onUncontrolledChange={(optionId) =>
                      handleQuestsChange('account', optionId)
                    }
                  />
                </div>
                <div className='col-span-3'>
                  <UncontrolledTextField
                    name='property'
                    label='Property'
                    placeholder='Please input a few letters of the hotel you require'
                    onChange={handleChangeDetails}
                    value={searchDetails.property}
                  />
                </div>
                <div className='col-span-2'></div>
                <div className='col-span-1'>
                  <Datepicker
                    label='Arrival Date'
                    onChange={handleChangeArrivalDate}
                  />
                </div>
                <div className='col-span-1'>
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
                    onUncontrolledChange={(optionId) =>
                      handleQuestsChange('duration', optionId)
                    }
                  />
                </div>
                <div className='grid grid-cols-3 gap-2 col-span-3'>
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
                    onUncontrolledChange={(optionId) =>
                      handleQuestsChange('adults', optionId)
                    }
                  />
                  <Select
                    id='children'
                    name='children'
                    labelText='Children'
                    options={Array.from({ length: 5 }, (_, i) => ({
                      id: i,
                      name: `${i}`,
                    }))}
                    defaultValue={{
                      id: 0,
                      name: '0',
                    }}
                    onUncontrolledChange={(optionId) =>
                      handleQuestsChange('children', optionId)
                    }
                  />
                  <Select
                    id='infants'
                    name='infants'
                    labelText='Infants'
                    options={Array.from({ length: 4 }, (_, i) => ({
                      id: i,
                      name: `${i}`,
                    }))}
                    defaultValue={{
                      id: 0,
                      name: '0',
                    }}
                    onUncontrolledChange={(optionId) =>
                      handleQuestsChange('infants', optionId)
                    }
                  />
                </div>
                {searchDetails.children > 0 && (
                  <div className='grid grid-cols-4 gap-2 col-span-2'>
                    <label className='block col-span-full text-sm font-medium text-dark'>
                      Children Ages
                    </label>
                    {Array.from({ length: searchDetails.children }, (_, i) => (
                      <div key={i} className='col-span-1'>
                        <Select
                          id={`childrenAges-${i + 1}`}
                          name={`childrenAges-${i + 1}`}
                          options={Array.from({ length: 16 }, (_, i) => ({
                            id: i + 1,
                            name: `${i + 1}`,
                          }))}
                          onUncontrolledChange={(optionId) =>
                            handleChangeChildrenAges(i, optionId)
                          }
                        />
                      </div>
                    ))}
                  </div>
                )}
              </div>
            </div>
          </div>
          <div className='p-4 w-full'>
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
