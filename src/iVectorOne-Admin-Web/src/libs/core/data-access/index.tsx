import { useState, useEffect, useCallback } from 'react';
import axios, { Method } from 'axios';
import { get } from 'lodash';
//
import { User, Tenant, Module } from '@/types';

export function useCoreFetching() {
  const [user, setUser] = useState<User>(null);
  const [moduleList, setModuleList] = useState<Module[]>([]);
  const [tenantList, setTenantList] = useState<Tenant[]>([]);
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  const fetch = useCallback(async () => {
    try {
      const userRes = axios.get('http://localhost:3001/users.info');
      const moduleListRes = axios.request({
        method: 'GET' as Method,
        url: 'http://localhost:3001/users.modules.list',
      });
      const tenantListRes = await axios.request({
        method: 'GET' as Method,
        url: 'http://localhost:3001/tenants.list',
      });

      const coreData = await Promise.all([
        userRes,
        moduleListRes,
        tenantListRes,
      ]);

      const userData = get(coreData[0], 'data.data', null);
      const user: User = {
        fullName: userData.fullName,
        tenants: userData.tenants,
      };
      const moduleList: Module[] = get(coreData[1], 'data.data.modules', []);
      setModuleList(moduleList);
      const tenantList: Tenant[] = get(coreData[2], 'data.tenants', []);
      setTenantList(tenantList);

      setUser(user);
      setError(null);
      setIsLoading(false);
    } catch (error) {
      if (typeof error === 'string') {
        console.log(error.toUpperCase());
        setError(error.toUpperCase());
      } else if (error instanceof Error) {
        console.log(error.message);
        setError(error.message);
      }

      setIsLoading(false);
    }
  }, []);

  useEffect(() => {
    fetch();
  }, [fetch]);

  return { user, moduleList, tenantList, isLoading, error };
}
