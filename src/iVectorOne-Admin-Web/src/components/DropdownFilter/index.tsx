import React, { useState, useRef, useEffect } from 'react';
//
import Transition from '@/utils/Transition';
import { DropdownFilterProps } from '@/types';

type Props = {
  allItems: any[];
  items: any[];
  setFilteredItems: React.Dispatch<React.SetStateAction<any[]>>;
  align: 'left' | 'right';
  filters: DropdownFilterProps[];
  setFilters: React.Dispatch<React.SetStateAction<DropdownFilterProps[]>>;
  title: string;
};

const DropdownFilter: React.FC<Props> = ({
  align,
  filters,
  setFilteredItems,
  setFilters,
  title,
  items,
  allItems,
}) => {
  const [dropdownOpen, setDropdownOpen] = useState(false);

  const trigger = useRef<any>(null);
  const dropdown = useRef<any>(null);

  // close on click outside
  useEffect(() => {
    const clickHandler = ({ target }: any) => {
      if (!dropdown.current || !trigger.current) return;
      if (
        !dropdownOpen ||
        dropdown.current.contains(target) ||
        trigger.current.contains(target)
      )
        return;
      setDropdownOpen(false);
    };
    document.addEventListener('click', clickHandler);
    return () => document.removeEventListener('click', clickHandler);
  });

  // close if the esc key is pressed
  useEffect(() => {
    const keyHandler = ({ keyCode }: { keyCode: number }) => {
      if (!dropdownOpen || keyCode !== 27) return;
      setDropdownOpen(false);
    };
    document.addEventListener('keydown', keyHandler);
    return () => document.removeEventListener('keydown', keyHandler);
  });

  const handleFilterChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const newFilters = filters.map((filter) => {
      if (filter.name === e.currentTarget.name) {
        return { ...filter, value: !filter.value };
      }
      return filter;
    });
    setFilters(newFilters);
  };

  const handleClearFilter = () => {
    const newFilters = filters.map((filter) => ({
      ...filter,
      value: false,
    }));
    setFilters(newFilters);
    setFilteredItems(allItems);
    setDropdownOpen(false);
  };

  const handleApplyFilter = () => {
    filters.forEach((filter) => {
      if (filter.name === 'Active' && filter.value) {
        setFilteredItems(
          allItems.filter((item) => {
            return item.isActive === filter.value;
          })
        );
      } else {
        setFilteredItems(allItems);
      }
    });
    setDropdownOpen(false);
  };

  return (
    <div className='relative inline-flex'>
      <button
        ref={trigger}
        className='btn bg-white border border-gray-300 text-gray-700 hover:bg-gray-50 focus:ring-blue-500'
        aria-haspopup='true'
        onClick={() => setDropdownOpen(!dropdownOpen)}
        aria-expanded={dropdownOpen}
      >
        <span className='sr-only'>{title}</span>
        <wbr />
        <svg className='w-4 h-4 fill-current' viewBox='0 0 16 16'>
          <path d='M9 15H7a1 1 0 010-2h2a1 1 0 010 2zM11 11H5a1 1 0 010-2h6a1 1 0 010 2zM13 7H3a1 1 0 010-2h10a1 1 0 010 2zM15 3H1a1 1 0 010-2h14a1 1 0 010 2z' />
        </svg>
      </button>
      <Transition
        show={dropdownOpen}
        tag='div'
        className={`origin-top-left z-10 absolute top-full min-w-56 bg-white border border-slate-200 pt-1.5 rounded shadow-lg overflow-hidden mt-1 ${
          align === 'right' ? 'right-0' : 'left-0'
        }`}
        enter='transition ease-out duration-200 transform'
        enterStart='opacity-0 -translate-y-2'
        enterEnd='opacity-100 translate-y-0'
        leave='transition ease-out duration-200'
        leaveStart='opacity-100'
        leaveEnd='opacity-0'
      >
        <div ref={dropdown}>
          <div className='text-xs font-semibold text-slate-400 uppercase pt-1.5 pb-2 px-4'>
            {title}
          </div>
          <ul className='mb-4'>
            {filters?.map(({ name, value }) => (
              <li key={name} className='py-1 px-3'>
                <label className='flex items-center'>
                  <input
                    name={name}
                    type='checkbox'
                    className='form-checkbox'
                    checked={value}
                    onChange={(e) => handleFilterChange(e)}
                  />
                  <span className='text-sm font-medium ml-2'>{name}</span>
                </label>
              </li>
            ))}
          </ul>
          <div className='py-2 px-3 border-t border-slate-200 bg-slate-50'>
            <ul className='flex items-center justify-between'>
              <li>
                <button
                  className='btn-xs bg-white border-slate-200 hover:border-slate-300 text-slate-500 hover:text-slate-600'
                  onClick={handleClearFilter}
                >
                  Clear
                </button>
              </li>
              <li>
                <button
                  className='btn-xs bg-indigo-500 hover:bg-indigo-600 text-white'
                  onClick={handleApplyFilter}
                  onBlur={handleApplyFilter}
                >
                  Apply
                </button>
              </li>
            </ul>
          </div>
        </div>
      </Transition>
    </div>
  );
};

export default React.memo(DropdownFilter);
