import React, {
  Fragment,
  useCallback,
  useEffect,
  useMemo,
  useState,
} from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { sortBy } from 'lodash';
import { BsFilterRight } from 'react-icons/bs';
import { AiOutlineClose } from 'react-icons/ai';
import { Transition } from '@headlessui/react';
//
import type { Account } from '@/types';
import { getAccounts } from '@/libs/core/data-access/account';
import { RootState } from '@/store';
import { NotificationStatus } from '@/constants';
import {
  Button,
  Datepicker,
  Select,
  UncontrolledTextField,
} from '@/components';
import classNames from 'classnames';

type Props = {
  filters: any;
  setFilters: React.Dispatch<React.SetStateAction<any>>;
  setResults: React.Dispatch<React.SetStateAction<any[]>>;
};

const LogFilters: React.FC<Props> = ({ filters, setFilters, setResults }) => {
  const dispatch = useDispatch();

  const user = useSelector((state: RootState) => state.app.user);
  const userKey = useSelector(
    (state: RootState) => state.app.awsAmplify.username
  );
  const isLoading = useSelector((state: RootState) => state.app.isLoading);

  const [accounts, setAccounts] = useState<Account[]>([]);
  const [searchQuery, setSearchQuery] = useState('');
  const [showFilters, setShowFilters] = useState(false);

  const activeTenant = useMemo(
    () => user?.tenants?.find((t) => t.isSelected),
    [user]
  );

  const handleChangeLogDateRange = useCallback(
    (date: Date[] | Date) => {
      //! TODO: fix any
      setFilters((prevFilters: any) => ({
        ...prevFilters,
        logDateRange: date,
      }));
    },
    [setFilters]
  );

  const handleOnFilterChange = (name: string, optionId: number) => {
    setFilters((prevFilters: any) => ({
      ...prevFilters,
      [name]: optionId,
    }));
  };

  const handleOnSearchQueryChange = (
    e: React.ChangeEvent<HTMLInputElement>
  ) => {
    const { value } = e.target;
    setSearchQuery(value);
  };

  const handleOnSearch = () => {
    console.log(filters);
  };

  const fetchAccounts = useCallback(async () => {
    if (!activeTenant || !userKey) return;
    await getAccounts(
      {
        id: activeTenant?.tenantId,
        key: activeTenant?.tenantKey,
      },
      userKey,
      () => {
        dispatch.app.setIsLoading(true);
      },
      (accounts) => {
        dispatch.app.setIsLoading(false);
        setAccounts(accounts);
        //! TODO: fix any
        setFilters((prev: any) => ({
          ...prev,
          accountId: accounts[0].accountId,
        }));
      },
      (error, instance) => {
        dispatch.app.setIsLoading(false);
        console.error(error, instance);
      }
    );
  }, [userKey, activeTenant]);

  useEffect(() => {
    fetchAccounts();

    return () => {
      setAccounts([]);
    };
  }, [fetchAccounts]);

  return (
    <div className='no-scrollbar relative px-3 py-6 border-b md:border-b-0 md:border-r border-slate-200 min-w-[380px] md:space-y-3'>
      <button
        className='absolute top-2 right-2 hover:scale-105 transition-transform duration-300'
        onClick={() => setShowFilters((prev) => !prev)}
        title='Toggle filters'
      >
        {showFilters ? (
          <AiOutlineClose className='w-6 h-6' />
        ) : (
          <BsFilterRight className='w-8 h-8' />
        )}
      </button>

      {showFilters && (
        <>
          <div>
            <div className='text-xs font-semibold text-slate-400 uppercase mb-3'>
              Log Filters
            </div>
          </div>
          <div className='grid md:grid-cols-2 lg:grid-cols-4 gap-3 pb-5'>
            <div>
              <Select
                id='account'
                name='account'
                labelText='Account'
                options={sortBy?.(accounts, [
                  function (o) {
                    return o?.userName?.toLowerCase?.();
                  },
                ])?.map((a) => ({
                  id: a.accountId,
                  name: a.userName,
                }))}
                onUncontrolledChange={(optionId) =>
                  handleOnFilterChange('account', optionId)
                }
              />
            </div>
            <div>
              <Datepicker
                mode='range'
                label='Log Date Range'
                onChange={handleChangeLogDateRange}
              />
            </div>
            <div>
              <Select
                id='supplier'
                name='supplier'
                labelText='Supplier'
                options={[
                  {
                    id: 0,
                    name: 'All',
                  },
                ]}
                onUncontrolledChange={(optionId) =>
                  handleOnFilterChange('supplier', optionId)
                }
              />
            </div>
            <div>
              <Select
                id='system'
                name='system'
                labelText='System'
                options={[
                  {
                    id: 0,
                    name: 'All',
                  },
                ]}
                onUncontrolledChange={(optionId) =>
                  handleOnFilterChange('system', optionId)
                }
              />
            </div>
            <div>
              <Select
                id='type'
                name='type'
                labelText='Type'
                options={[
                  {
                    id: 0,
                    name: 'All',
                  },
                ]}
                onUncontrolledChange={(optionId) =>
                  handleOnFilterChange('type', optionId)
                }
              />
            </div>
            <div>
              <Select
                id='responseSuccess'
                name='responseSuccess'
                labelText='Response Success'
                options={[
                  {
                    id: 0,
                    name: 'All',
                  },
                ]}
                onUncontrolledChange={(optionId) =>
                  handleOnFilterChange('responseSuccess', optionId)
                }
              />
            </div>
            <div className='col-span-full'>
              <Button text='Refresh' />
            </div>
          </div>
        </>
      )}
      <div className='grid lg:grid-cols-4'>
        <h3 className='col-span-full text-3xl font-semibold'>Booking Search</h3>
        <div className='col-span-full text-sm mb-2'>
          <p>
            Please input a booking reference, supplier booking reference or lead
            quest name
          </p>
        </div>
        <div className='col-span-2'>
          <UncontrolledTextField
            name='searchQuery'
            onChange={handleOnSearchQueryChange}
          />
        </div>
        <div className='col-span-full'>
          <Button text='Search' onClick={handleOnSearch} className='mt-5' />
        </div>
      </div>
    </div>
  );
};

export default React.memo(LogFilters);
