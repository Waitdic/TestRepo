import { useState, useEffect, useCallback } from 'react';
import axios, { Method } from 'axios';
import { get } from 'lodash';
//
import { Subscription, Provider, ProviderConfiguration } from '@/types';

export function useIvoFetching() {
  const [subscriptions, setSubscriptions] = useState<Subscription[]>([]);
  const [providers, setProviders] = useState<Provider[]>([]);
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  const fetch = useCallback(async () => {
    try {
      const subscriptionRes = axios.request({
        method: 'GET' as Method,
        url: 'http://localhost:3001/ivo/subscriptions.list',
      });
      const providerRes = await axios.request({
        method: 'GET' as Method,
        url: 'http://localhost:3001/ivo/providers.list',
      });

      const IvoData = await Promise.all([subscriptionRes, providerRes]);

      const subscriptionsData: Subscription[] = get(
        IvoData[0],
        'data.data.subscriptions',
        []
      );
      setSubscriptions(subscriptionsData);
      const providersData: Provider[] = get(
        IvoData[1],
        'data.data.subscriptions',
        []
      );
      setProviders(providersData);

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

  return { configurations, error, isLoading };
}
