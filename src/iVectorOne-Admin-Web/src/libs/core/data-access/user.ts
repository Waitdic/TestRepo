import { get } from 'lodash';
//
import ApiCall from '@/axios';
import { ApiError, User, UserResponse } from '@/types';
import handleApiError from '@/utils/handleApiError';
import { UserFormFields } from '../user/create';

//* Get all users
export async function getUsers(
  userTenantKey: string,
  userKey: string,
  onInit: () => void,
  onSuccess: (users: UserResponse[]) => void,
  onFailed: (error: string) => void
) {
  onInit();
  try {
    const usersRes = await ApiCall.request({
      method: 'GET',
      url: `/users`,
      headers: {
        Accept: 'application/json',
        Tenantkey: userTenantKey,
        UserKey: userKey,
      },
    });
    const users = get(usersRes, 'data.users', []);
    onSuccess(users);
  } catch (err: any) {
    const errorMessage = handleApiError(err as ApiError);
    onFailed?.(errorMessage);
  }
}

//* Get user by id
export async function getUserInfo(
  userTenantKey: string,
  currentUserKey: string, //? User key of the user who is currently logged in
  userKey: string, //? User key of the user to get info for
  onInit: () => void,
  onSuccess: (user: User) => void,
  onFailed: (error: string) => void
) {
  onInit();
  try {
    const usersRes = await ApiCall.request({
      method: 'GET',
      url: `/users/${userKey}`,
      headers: {
        Accept: 'application/json',
        Tenantkey: userTenantKey,
        UserKey: currentUserKey,
      },
    });
    const user = get(usersRes, 'data', {});
    onSuccess(user);
  } catch (err: any) {
    const errorMessage = handleApiError(err as ApiError);
    onFailed?.(errorMessage);
  }
}

//* Unlink tenant from user
export async function unlinkUserTenant(
  userKey: string,
  tenantId: number,
  userId: number,
  onInit: () => void,
  onSuccess: () => void,
  onFailed: (error: string) => void
) {
  onInit();
  try {
    await ApiCall.request({
      method: 'PUT',
      url: `/tenants/${tenantId}/users/${userId}/unlink`,
      headers: {
        Accept: 'application/json',
        Tenantkey: userKey,
        UserKey: userKey,
      },
    });
    onSuccess();
  } catch (err: any) {
    const errorMessage = handleApiError(err as ApiError);
    onFailed?.(errorMessage);
  }
}

//* Link tenant to user
export async function linkUserTenant(
  userKey: string,
  tenantId: number,
  userId: number,
  onInit: () => void,
  onSuccess: () => void,
  onFailed: (error: string) => void
) {
  onInit();
  try {
    await ApiCall.request({
      method: 'PUT',
      url: `/tenants/${tenantId}/users/${userId}/link`,
      headers: {
        Accept: 'application/json',
        Tenantkey: userKey,
        UserKey: userKey,
      },
      data: {
        RelationShip: 'admin',
      },
    });
    onSuccess();
  } catch (err: any) {
    const errorMessage = handleApiError(err as ApiError);
    onFailed?.(errorMessage);
  }
}

//* Create User
export async function createUser(
  userTenantKey: string,
  userKey: string,
  data: UserFormFields,
  onInit: () => void,
  onSuccess: () => void,
  onFailed: (error: string) => void
) {
  onInit();
  try {
    await ApiCall.request({
      method: 'POST',
      url: `/users`,
      headers: {
        Accept: 'application/json',
        Tenantkey: userTenantKey,
        UserKey: userKey,
      },
      data,
    });
    onSuccess();
  } catch (err: any) {
    const errorMessage = handleApiError(err as ApiError);
    onFailed?.(errorMessage);
  }
}
