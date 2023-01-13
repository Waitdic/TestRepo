import React, { useCallback, useMemo, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { orderBy as _orderBy } from 'lodash';
import { AiOutlineClose } from 'react-icons/ai';
//
import type { LogEntries } from '@/types';
import type { TableListBody } from 'src/components/TableList';
import { RootState } from '@/store';
import Main from '@/layouts/Main';
import { LogFilters, Modal, TableList } from '@/components';
import { getLogViewerPayloadPopup } from './data-access';
import getLogDetailsPayloadPopupModel from '@/utils/getLogDetailsPayloadPopupModel';
import { syntaxHighlight } from '@/utils/syntaxHighlight';

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
  {
    name: 'Actions',
    align: 'right',
  },
];

type TableOrderBy = {
  by: string | null;
  order: 'asc' | 'desc';
  only: string[];
};

type PayloadPopup = {
  isOpen: boolean;
  variant: 'request' | 'response';
  details: any;
};

const LogViewer: React.FC = () => {
  const dispatch = useDispatch();

  const isLoading = useSelector((state: RootState) => state.app.isLoading);
  const user = useSelector((state: RootState) => state.app.user);
  const userKey = useSelector(
    (state: RootState) => state.app.awsAmplify.username
  );

  const [results, setResults] = useState<LogEntries[]>([]);
  const [orderBy, setOrderBy] = useState<TableOrderBy>({
    by: 'Date and Time',
    order: 'asc',
    only: ['Date and Time'],
  });
  const [accountId, setAccountId] = useState(-1);
  const [payloadPopup, setPayloadPopup] = useState<PayloadPopup>({
    isOpen: false,
    variant: 'request',
    details: {},
  });

  const activeTenant = useMemo(() => {
    return user?.tenants.find((tenant) => tenant.isSelected);
  }, [user]);

  const handleFetchPayloadPopup = useCallback(
    async (rowId: number | undefined, variant: 'request' | 'response') => {
      if (isLoading || !activeTenant || !userKey || !rowId) return;

      setPayloadPopup((prev) => ({
        ...prev,
        isOpen: true,
        variant,
        details: {}
      }));

      await getLogViewerPayloadPopup({
        tenant: {
          id: activeTenant.tenantId,
          key: activeTenant.tenantKey,
        },
        userKey,
        accountId,
        logId: rowId,
        onInit: () => {
          dispatch.app.setIsLoading(true);
        },
        onSuccess: (logDetails) => {
          dispatch.app.setIsLoading(false);
          var model = getLogDetailsPayloadPopupModel(logDetails[0]); 
          setPayloadPopup((prev) => ({
            ...prev,
            details: model,
          }));
        },
        onFailed: (message, instance, title) => {
          dispatch.app.setIsLoading(false);
          dispatch.app.setNotification({
            message,
            instance,
            title,
          });
        },
      });
    },
    [isLoading, activeTenant, userKey, accountId]
  );

  const tableBody = useMemo(() => {
    let rows: TableListBody[] = [];
    let orderedResults = results;
    if (orderBy.by === 'Date and Time') {
      orderedResults = _orderBy(results, 'timestamp', [orderBy.order]);
    }

    orderedResults.forEach((result, idx) => {
      let items: any[] = [];
      Object.entries(result).forEach(([key, value]) => {
        if (key === 'id') return;
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
        actions: [
          {
            name: 'Req',
            onClick: () => handleFetchPayloadPopup(result.id, 'request'),
          },
          {
            name: 'Res',
            onClick: () => handleFetchPayloadPopup(result.id, 'response'),
          },
        ],
      });
    });
    return rows;
  }, [results, orderBy]);

  const handleTogglePayloadPopup = () =>
    setPayloadPopup({
      ...payloadPopup,
      isOpen: !payloadPopup.isOpen,
    });

  return (
    <>
      <Main title='Log Viewer'>
        <div className='bg-white shadow-lg rounded-sm mb-8'>
          <div className='flex flex-col gap-2 min-h-[720px]'>
            <LogFilters setResults={setResults} setAccountId={setAccountId} />
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
                  minWidth='1100px'
                />
              </div>
            )}
          </div>
        </div>
      </Main>

      {payloadPopup.isOpen && (
        <Modal transparent flex onKeyDown={handleTogglePayloadPopup}>
          <div className='relative bg-white rounded shadow-lg overflow-auto max-w-3xl w-full max-h-full min-w-[280px]'>
            <div className='absolute top-0 right-0'>
              <button
                className='cursor-pointer p-1'
                onClick={handleTogglePayloadPopup}
              >
                <AiOutlineClose className='w-4 h-4' />
              </button>
            </div>
            <div className='px-5 py-3 border-b border-slate-200'>
              <div className=''>
                <div className='font-semibold text-slate-800'>
                  {payloadPopup.variant === 'request' 
                    ? 'Request '
                    : 'Response'}                  
                </div>
                <div className='text-sm text-dark-heading'>
                  <p>{payloadPopup.details.urlPath}</p>
                </div>
              </div>
              {payloadPopup.variant === 'request' && String(payloadPopup.details.urlParams).length > 2 && 
              (
                <div>
                  <div className='flex justify-between items-center'>
                    <div className='font-semibold text-slate-800'>
                      Url Params
                    </div>
                  </div>
                
                  <div className='text-xs lg:text-sm overflow-auto max-h-[90vh] mt-2'>
                    <pre
                      dangerouslySetInnerHTML={{
                        __html: syntaxHighlight(payloadPopup.details.urlParams),
                      }}
                    />
                  </div>
                </div>
              )}
              {(isLoading || Object.keys(payloadPopup.details).length === 0) && (
                <div className='text-center text-sm pb-4 px-4'>
                  <p className='animate-pulse'>Loading...</p>
                </div>
              )}
              <div className='flex justify-between items-center'>
                <div className='font-semibold text-slate-800'>
                  Content
                </div>
              </div>
              <div className='text-xs lg:text-sm overflow-auto max-h-[90vh] mt-2'>
                <pre
                  dangerouslySetInnerHTML={{
                    __html: syntaxHighlight(
                      payloadPopup.variant === 'request' 
                      ? payloadPopup.details.requestBody
                      : payloadPopup.details.responseBody
                    ),
                  }}
                />
              </div>
            </div>
          </div>
        </Modal>
      )}
    </>
  );
};

export default React.memo(LogViewer);
