import React, { useCallback, useEffect, useMemo, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { sortBy } from 'lodash';
//
import type {
  SearchDetails,
  Account,
  SearchRequestData,
  SupplierSearchResults,
} from '@/types';
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

type Props = {
  searchDetails: SearchDetails;
  setSearchDetails: React.Dispatch<React.SetStateAction<SearchDetails>>;
  setSearchResults: React.Dispatch<
    React.SetStateAction<SupplierSearchResults[]>
  >;
};

const SearchFilters: React.FC<Props> = ({
  searchDetails,
  setSearchDetails,
  setSearchResults,
}) => {
  const dispatch = useDispatch();

  const user = useSelector((state: RootState) => state.app.user);
  const userKey = useSelector(
    (state: RootState) => state.app.awsAmplify.username
  );
  const isLoading = useSelector((state: RootState) => state.app.isLoading);

  const [accounts, setAccounts] = useState<Account[]>([]);
  const [searchQuery, setSearchQuery] = useState('');

  const activeTenant = useMemo(
    () => user?.tenants?.find((t) => t.isSelected),
    [user]
  );

  const onSearch = useCallback(
    async (value: string) => {
      if (isLoading) return;
      await getPropertiesById({
        userKey: userKey as string,
        tenant: {
          id: activeTenant?.tenantId as number,
          key: activeTenant?.tenantKey as string,
        },
        accountId: searchDetails.accountId,
        query: value,
        onInit: () => {
          dispatch.app.setIsLoading(true);
          setSearchDetails((prev) => ({ ...prev, properties: [] }));
        },
        onSuccess: (properties) => {
          dispatch.app.setIsLoading(false);
          setSearchDetails((prev) => ({ ...prev, properties }));
        },
        onFailed: (_err, instance, title) => {
          dispatch.app.setIsLoading(false);
          dispatch.app.setNotification({
            status: NotificationStatus.ERROR,
            message: title,
            instance,
          });
        },
      });
    },
    [activeTenant, userKey, searchDetails, isLoading]
  );

  const handleChangeDetails = useCallback(
    (e: React.ChangeEvent<HTMLInputElement>) => {
      if (isLoading) return;
      const { value } = e.target;
      setSearchDetails((prev) => ({
        ...prev,
        property: {
          propertyId: 0,
          name: value,
        },
      }));
    },
    [isLoading]
  );

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
    if (name === 'account') {
      setSearchDetails({
        accountId: value,
        properties: [],
        property: {
          propertyId: 0,
          name: '',
        },
        arrivalDate: new Date(new Date().setDate(new Date().getDate() + 1)),
        duration: 7,
        adults: 2,
        children: 0,
        childrenAges: [],
        infants: 0,
        isActive: false,
      });
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
        setSearchResults([]);
      },
      onSuccess: (data) => {
        setSearchDetails((prev) => ({
          ...prev,
          isActive: true,
        }));
        setSearchResults(data);
        dispatch.app.setIsLoading(false);
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

  const handlePropertySearch = useCallback(async () => {
    if (!activeTenant || !userKey || searchDetails.property.name === '') return;
    const { name } = searchDetails.property;
    if (name.length >= 4 && !!searchDetails.accountId) {
      await onSearch(name);
      setSearchQuery(searchDetails.property.name);
    }
  }, [activeTenant, userKey, searchDetails, onSearch]);

  useEffect(() => {
    if (searchDetails.property.name === '') return;
    const timedSearch = setInterval(() => {
      if (
        searchDetails.property.name.length >= 4 &&
        searchDetails.property.name !== searchQuery &&
        !!searchDetails.accountId
      ) {
        handlePropertySearch();
      }
    }, 2000);

    return () => {
      timedSearch && clearInterval(timedSearch);
    };
  }, [searchDetails, searchQuery]);

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
              options={sortBy?.(accounts, [
                function (o) {
                  return o?.userName?.toLowerCase?.();
                },
              ])?.map((a) => ({
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
          <div className='grid grid-cols-12 gap-2 col-span-full'>
            <div className='col-span-2'>
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
            </div>
            <div className='col-span-2'>
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
            </div>
            {searchDetails.children > 0 && (
              <div className='grid grid-cols-4 items-end gap-2 col-span-4'>
                {Array.from({ length: searchDetails.children }, (_, i) => (
                  <div className='relative flex flex-1 flex-col'>
                    {i === 0 && (
                      <label className='block absolute bottom-full left-0 col-span-full min-w-[120px] text-sm font-medium text-dark'>
                        Children Ages
                      </label>
                    )}
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
                  </div>
                ))}
              </div>
            )}
            <div className='col-span-2'>
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
            <div
              className={`col-span-${
                searchDetails.childrenAges.length > 0 ? 2 : 6
              } flex items-end justify-end`}
            >
              {!isLoading && (
                <Button text='Search' onClick={handleSearchSubmit} />
              )}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default React.memo(SearchFilters);
