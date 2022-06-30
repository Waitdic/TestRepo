import { useState, useEffect, useCallback } from 'react';
import { get } from 'lodash';
import { useSelector } from 'react-redux';
//
import { RootState } from '@/store';
import ApiCall from '@/axios';
import { User, Tenant, Module } from '@/types';

export function useCoreFetching() {
  const { username } = useSelector((state: RootState) => state.app.awsAmplify);

  const [user, setUser] = useState<User>(null);
  const [moduleList, setModuleList] = useState<Module[]>([]);
  const [tenantList, setTenantList] = useState<Tenant[]>([]);
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  const fetch = useCallback(async (userKey: string) => {
    try {
      const userRes = await ApiCall.get(`/users/${userKey}`);
      const userData = get(userRes, 'data', null);
      const user: User = {
        fullName: userData?.fullName,
        tenants: userData?.tenants,
      };
      setUser(user);

      if (user?.tenants.length > 0) {
        setTenantList(user?.tenants);
        setError(null);
      } else {
        setTenantList([]);
        setError('Contact support to complete the setup of your account');
      }

      setIsLoading(false);
    } catch (error) {
      if (typeof error === 'string') {
        console.error(error.toUpperCase());
        setError(error.toUpperCase());
      } else if (error instanceof Error) {
        console.error(error.message);
        setError(error.message);
      }
      setIsLoading(false);
    }
  }, []);

  useEffect(() => {
    if (username) {
      fetch(username);
    }
  }, [fetch, username]);

  return { user, moduleList, tenantList, isLoading, error };
}
