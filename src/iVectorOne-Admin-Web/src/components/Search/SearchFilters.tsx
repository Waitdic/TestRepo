import React, { useCallback, useEffect, useMemo, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
//
import type { SearchDetails, Account, SearchRequestData } from '@/types';
import { getAccounts } from '@/libs/core/data-access/account';
import { RootState } from '@/store';
import { NotificationStatus } from '@/constants';
import {
  Button,
  Datepicker,
  Select,
  UncontrolledTextField,
} from '@/components';
import {
  getPropertiesById,
  searchByProperty,
} from '@/libs/search/data-access/property';
import { debounce } from 'lodash';

type Props = {
  setSearchDetails: React.Dispatch<React.SetStateAction<SearchDetails>>;
  searchDetails: SearchDetails;
};

const SearchFilters: React.FC<Props> = ({
  searchDetails,
  setSearchDetails,
}) => {
  const dispatch = useDispatch();

  const user = useSelector((state: RootState) => state.app.user);
  const userKey = useSelector(
    (state: RootState) => state.app.awsAmplify.username
  );
  const isLoading = useSelector((state: RootState) => state.app.isLoading);

  const [accounts, setAccounts] = useState<Account[]>([]);

  const activeTenant = useMemo(
    () => user?.tenants?.find((t) => t.isSelected),
    [user]
  );

  const handleChangeDetails = async (
    e: React.ChangeEvent<HTMLInputElement>
  ) => {
    if (isLoading) return;
    const { value } = e.target;
    setSearchDetails((prev) => ({
      ...prev,
      property: {
        propertyId: 0,
        name: value,
      },
    }));

    const debounced = debounce(
      async () => {
        await getPropertiesById({
          userKey: userKey as string,
          tenant: {
            id: activeTenant?.tenantId as number,
            key: activeTenant?.tenantKey as string,
          },
          accountId: searchDetails.accountId,
          query: value,
          onInit: () => {
            setSearchDetails((prev) => ({ ...prev, properties: [] }));
            dispatch.app.setIsLoading(true);
          },
          onSuccess: (properties) => {
            dispatch.app.setIsLoading(false);
            setSearchDetails((prev) => ({ ...prev, properties }));
          },
          onFailed: (err, instance) => {
            dispatch.app.setIsLoading(false);
            dispatch.app.setNotification({
              status: NotificationStatus.ERROR,
              message: err,
              instance,
            });
          },
        });
      },
      100,
      { leading: true, trailing: true, maxWait: 1000 }
    );
    if (
      value.length >= 4 &&
      !!searchDetails.accountId &&
      value !== searchDetails.property.name
    ) {
      debounced();
    }
  };

  const handleChangeArrivalDate = (date: Date[] | Date) => {
    if (Array.isArray(date)) {
      setSearchDetails((prev) => ({
        ...prev,
        arrivalDate: date[0],
      }));
    } else {
      setSearchDetails((prev) => ({ ...prev, arrivalDate: date }));
    }
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

  const handleSearchSubmit = useCallback(async () => {
    if (!activeTenant || !userKey || isLoading) return;

    if (searchDetails.property.propertyId === 0) {
      dispatch.app.setNotification({
        message: 'Please select a valid property',
        status: NotificationStatus.ERROR,
      });
      return;
    }

    const requestData: SearchRequestData = {
      ArrivalDate: searchDetails.arrivalDate.toISOString(),
      Duration: searchDetails.duration,
      Properties: [searchDetails.property.propertyId],
      RoomRequests: [
        {
          Adults: searchDetails.adults,
          Children: searchDetails.children,
          Infants: searchDetails.infants,
          ChildAges: searchDetails.childrenAges,
        },
      ],
    };

    await searchByProperty({
      userKey: userKey,
      tenant: {
        id: activeTenant?.tenantId,
        key: activeTenant?.tenantKey,
      },
      accountId: searchDetails.accountId,
      requestData,
      onInit: () => {
        dispatch.app.setIsLoading(true);
      },
      onSuccess: (data) => {
        dispatch.app.setIsLoading(false);
        setSearchDetails((prev) => ({
          ...prev,
          isActive: true,
        }));
      },
      onFailed: (message, instance) => {
        dispatch.app.setIsLoading(false);
        dispatch.app.setNotification({
          status: NotificationStatus.ERROR,
          message,
          instance,
        });
      },
    });
  }, [activeTenant, userKey, searchDetails, isLoading]);

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
        setSearchDetails((prev) => ({
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

  const handleSetProperty = (selectedResult: number) => {
    const selectedProperty = searchDetails.properties?.find(
      (p) => p.propertyId === selectedResult
    );
    if (selectedProperty) {
      setSearchDetails((prev) => ({
        ...prev,
        property: selectedProperty,
      }));
    }
  };

  const autoCompleteDetails = useMemo(
    () => ({
      results: searchDetails.properties,
      handler: handleSetProperty,
    }),
    [searchDetails]
  );

  useEffect(() => {
    fetchAccounts();

    return () => {
      setAccounts([]);
    };
  }, [fetchAccounts]);

  return (
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
              options={accounts?.map((a) => ({
                id: a.accountId,
                name: a.userName,
              }))}
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
              value={searchDetails.property.name}
              autoComplete={autoCompleteDetails}
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
                    })).slice(2, 16)}
                    onUncontrolledChange={(optionId) =>
                      handleChangeChildrenAges(i, optionId)
                    }
                  />
                </div>
              ))}
            </div>
          )}
          <div className='row-start-4 col-span-full'>
            {!isLoading && (
              <Button text='Search' onClick={handleSearchSubmit} />
            )}
          </div>
        </div>
      </div>
    </div>
  );
};

export default React.memo(SearchFilters);
