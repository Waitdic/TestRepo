import { useState, useEffect, useCallback } from 'react';
import { get } from 'lodash';
import { useDispatch, useSelector } from 'react-redux';
//
import { RootState } from '@/store';
import ApiCall from '@/axios';
import type { User, Tenant } from '@/types';

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
        fullName: userData.fullName,
        tenants: userData.tenants.map((tenant: Tenant, idx: number) => ({
          ...tenant,
          isSelected: idx === 0,
        })),
        authorisations: userData.authorisations,
        success: userData.success,
      };
      dispatch.app.updateUser(user);
      if (user?.tenants.length > 0) {
        setError(null);
      } else {
        setError('Contact support to complete the setup of your account');
      }
      dispatch.app.setIsLoading(false);
    } catch (err) {
      if (typeof err === 'string') {
        console.error(err.toUpperCase());
        setError(err.toUpperCase());
      } else if (err instanceof Error) {
        console.error(err.message);
        setError(err.message);
      }
      dispatch.app.setIsLoading(false);
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
      fullName: userData.fullName,
      tenants: userData.tenants.map((tenant: Tenant, idx: number) => ({
        ...tenant,
        isSelected: idx === 0,
      })),
      authorisations: userData.authorisations,
      success: userData.success,
    };
    onSuccess(user);
  } catch (err) {
    if (typeof err === 'string') {
      console.error(err.toUpperCase());
      onFailed(err.toUpperCase());
    } else if (err instanceof Error) {
      console.error(err.message);
      onFailed(err.message);
    }
  }
}
