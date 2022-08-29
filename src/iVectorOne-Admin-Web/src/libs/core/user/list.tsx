import React, { useCallback, useEffect, useMemo, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
//
import { RootState } from '@/store';
import MainLayout from '@/layouts/Main';
import { NotificationStatus } from '@/constants';
import { CardList, Notification, RoleGuard } from '@/components';
import { getUsers } from '../data-access/user';
import type { NotificationState, UserResponse } from '@/types';

type Props = {};

const tableEmptyState = {
  title: 'No Users',
  description: ['Get started by creating a new user.'],
  href: '/users/create',
  buttonText: 'New User',
};

const UserList: React.FC<Props> = () => {
  const dispatch = useDispatch();
  const user = useSelector((state: RootState) => state.app.user);
  const userKey = useSelector(
    (state: RootState) => state.app.awsAmplify.username
  );
  const isLoading = useSelector((state: RootState) => state.app.isLoading);

  const [notification, setNotification] = useState<NotificationState>();
  const [showNotification, setShowNotification] = useState(false);
  const [users, setUsers] = useState<UserResponse[]>([]);

  const activeTenant = useMemo(
    () => user?.tenants.find((tenant: any) => tenant.isSelected),
    [user?.tenants]
  );
  const isValidUser = useMemo(
    () => !!userKey && !!activeTenant,
    [userKey, activeTenant]
  );
  const tableList = useMemo(
    () =>
      users?.map(({ userId, userName, key }) => ({
        id: userId,
        name: userName,
        isActive: true,
        actions: [{ name: 'View', href: `/users/${key}` }],
      })),
    [users]
  );

  const fetchUsers = useCallback(async () => {
    if (!isValidUser) return;
    await getUsers(
      activeTenant?.tenantKey as string,
      userKey as string,
      () => {
        dispatch.app.setIsLoading(true);
      },
      (fetchedUsers) => {
        dispatch.app.setIsLoading(false);
        setUsers(fetchedUsers);
      },
      () => {
        dispatch.app.setIsLoading(false);
        setNotification({
          status: NotificationStatus.ERROR,
          message: 'Error fetching users',
        });
      }
    );
  }, [isValidUser]);

  useEffect(() => {
    fetchUsers();
  }, [fetchUsers]);

  return (
    <>
      <RoleGuard withRedirect>
        <MainLayout title='Users' addNew addNewHref='/users/create'>
          <CardList
            bodyList={tableList}
            isLoading={isLoading}
            emptyState={tableEmptyState}
          />
        </MainLayout>
      </RoleGuard>

      {showNotification && (
        <Notification
          description={notification?.message as string}
          show={showNotification}
          setShow={setShowNotification}
          status={notification?.status as NotificationStatus}
        />
      )}
    </>
  );
};

export default React.memo(UserList);
