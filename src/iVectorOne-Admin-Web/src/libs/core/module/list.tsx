import { memo, useState, FC, useEffect } from 'react';
//
import { Module } from '@/types';
import { NotificationStatus } from '@/constants';
//
import MainLayout from '@/layouts/Main';
import {
  ErrorBoundary,
  TableList,
  Button,
  SearchField,
  Notification,
} from '@/components';

type Props = {
  fetchedModuleList: {
    moduleList: Module[];
    error: string | null;
    isLoading: boolean;
  };
};

export const ModuleList: FC<Props> = memo(({ fetchedModuleList }) => {
  const [filteredModuleList, setFilteredModuleList] = useState<Module[]>(
    fetchedModuleList.moduleList
  );
  const [showError, setShowError] = useState<boolean>(false);

  const tableHeaderList = [
    { name: 'Name', align: 'left' },
    { name: 'Actions', align: 'right' },
  ];

  const tableBodyList = filteredModuleList.map(({ moduleId, name }) => ({
    id: moduleId,
    name,
    actions: [{ name: 'Edit', href: `/module/edit/${moduleId}` }],
  }));
  const tableEmptyState = {
    title: 'No modules',
    description: 'Get started by creating a new module.',
    href: '/module/create',
    buttonText: 'New Module',
  };

  useEffect(() => {
    if (fetchedModuleList.error) {
      setShowError(true);
    } else {
      setShowError(false);
    }
    setFilteredModuleList(fetchedModuleList.moduleList);
  }, [fetchedModuleList]);

  return (
    <>
      <MainLayout
      // title='Module List'
      >
        <div className='flex flex-col'>
          {/* Modules */}

          {fetchedModuleList.error ? (
            <ErrorBoundary />
          ) : (
            <>
              <div className='flex align-start justify-end mb-6'>
                <div className='flex'>
                  <SearchField
                    list={fetchedModuleList.moduleList}
                    setList={setFilteredModuleList}
                  />
                  <Button
                    text='New'
                    isLink
                    href='/module/create'
                    className='ml-3'
                  />
                </div>
              </div>
              <TableList
                headerList={tableHeaderList}
                bodyList={tableBodyList}
                isLoading={fetchedModuleList.isLoading}
                emptyState={tableEmptyState}
              />
            </>
          )}
        </div>
      </MainLayout>

      {showError && (
        <Notification
          title='Error'
          description={fetchedModuleList.error as string}
          show={showError}
          setShow={setShowError}
          status={NotificationStatus.ERROR}
          autoHide={false}
        />
      )}
    </>
  );
});
