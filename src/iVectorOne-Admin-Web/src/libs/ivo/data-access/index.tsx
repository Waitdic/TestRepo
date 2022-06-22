import { useState, useEffect, useCallback } from 'react';
import axios, { Method } from 'axios';
import { get } from 'lodash';
import { useSelector } from 'react-redux';
//
import { Subscription, Provider, ProviderConfiguration, User } from '@/types';
import { RootState } from '@/store';
import ApiCall from '@/axios';

export function useIvoFetching() {
  const user = useSelector((state: RootState) => state.app.user);

  const [subscriptions, setSubscriptions] = useState<Subscription[]>([]);
  const [providers, setProviders] = useState<Provider[]>([]);
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  const fetch = useCallback(async (user: User) => {
    try {
      const activeTenant = user?.tenants.find((tenant) => tenant.isActive);
      const tenantKey: any = activeTenant?.tenantKey;
      const subsRes = await ApiCall.get(`/tenants/subscriptions`, {
        headers: {
          Accept: 'application/json',
          Tenantkey: tenantKey,
        },
      });
      const subscriptions: Subscription[] = get(
        subsRes,
        'data.subscriptions',
        []
      );
      setSubscriptions(subscriptions);
      setProviders([]);
      setError(null);
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
    if (!!user?.tenants?.length) {
      fetch(user);
    }
  }, [fetch, user]);

  return { subscriptions, providers, isLoading, error };
}

export function useProviderInfo() {
  const [configurations, setConfigurations] = useState<ProviderConfiguration[]>(
    []
  );
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  const fetch = useCallback(async () => {
    setIsLoading(true);

    try {
      const res = await axios.request({
        method: 'GET' as Method,
        url: 'http://localhost:3001/ivo/providers/info',
      });

      const providersRes = get(res, 'data.data.configuration', []);
      setConfigurations(providersRes);
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
    fetch();
  }, [fetch]);

  return { configurations, error, isLoading };
}
