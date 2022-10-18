import { useState, useEffect, useCallback } from 'react';
import { get } from 'lodash';
import { useDispatch, useSelector } from 'react-redux';
//
import { RootState } from '@/store';
import ApiCall from '@/axios';
import type { User, Tenant, ApiError } from '@/types';
import handleApiError from '@/utils/handleApiError';

//* User, tenant data fetch
export function useCoreFetching() {
  const dispatch = useDispatch();
  const { username } = useSelector((state: RootState) => state.app.awsAmplify);
  const [error, setError] = useState<string | null>(null);
  const fetch = useCallback(async (userKey: string) => {
    dispatch.app.setIsLoading(true);
    try {
      const userRes = await ApiCall.get(`/users/${userKey}`);
      const userData = get(userRes, 'data', null);
      if (!userData) {
        throw new Error('User not found');
      }
      const user: User = {
        userId: userData.userId,
        fullName: userData.fullName,
        tenants: userData.tenants.map((tenant: Tenant, idx: number) => ({
          ...tenant,
          isSelected: idx === 0,
        })),
        authorisations: userData.authorisations,
        success: userData.success,
      };
      dispatch.app.updateUser(user);

      const isValidUser =
        (user.tenants.length > 0 || userData.authorisations.length > 0) &&
        userData !== null;
      if (isValidUser) {
        setError(null);
        dispatch.app.setIncompleteSetup(false);
      } else {
        setError('Contact support to complete the setup of your account.');
        dispatch.app.setIncompleteSetup(true);
      }
      dispatch.app.setIsLoading(false);
    } catch (err) {
      dispatch.app.setIncompleteSetup(true);
      dispatch.app.setIsLoading(false);
      const errorMessage = handleApiError(err as ApiError);
      setError(errorMessage);
    }
  }, []);

  useEffect(() => {
    if (username) {
      fetch(username);
    }
  }, [fetch, username]);

  return { error };
}

//* Refetch user data
export async function refetchUserData(
  userKey: string,
  onInit: () => void,
  onSuccess: (user: User) => void,
  onFailed: (err: string) => void
) {
  onInit();
  try {
    const userRes = await ApiCall.get(`/users/${userKey}`);
    const userData = get(userRes, 'data', null);
    if (!userData) {
      throw new Error('User not found');
    }
    const user: User = {
      userId: userData.userId,
      fullName: userData.fullName,
      tenants: userData.tenants.map((tenant: Tenant, idx: number) => ({
        ...tenant,
        isSelected: idx === 0,
      })),
      authorisations: userData.authorisations,
      success: userData.success,
    };
    onSuccess(user);
  } catch (err: any) {
    const errorMessage = handleApiError(err as ApiError);
    onFailed?.(errorMessage);
  }
}
