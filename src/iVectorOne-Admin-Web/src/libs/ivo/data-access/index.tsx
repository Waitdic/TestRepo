import { useState, useEffect, useCallback } from 'react';
import { get } from 'lodash';
import { useSelector } from 'react-redux';
//
import { Subscription, Provider, User } from '@/types';
import { RootState } from '@/store';
import ApiCall from '@/axios';

export function useIvoFetching() {
  const user = useSelector((state: RootState) => state.app.user);

  const [subscriptions, setSubscriptions] = useState<Subscription[]>([]);
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  const fetch = useCallback(async (user: User) => {
    try {
      const activeTenant = user?.tenants.find((tenant) => tenant.isActive);
      const tenantKey: any = activeTenant?.tenantKey;
      const subsRes = await ApiCall.get(
        `/tenants/${activeTenant?.tenantId}/subscriptions`,
        {
          headers: {
            Accept: 'application/json',
            Tenantkey: tenantKey,
          },
        }
      );
      const subscriptions: Subscription[] = get(
        subsRes,
        'data.subscriptions',
        []
      );
      subscriptions.forEach(async (subscription) => {
        const { subscriptionId } = subscription;
        const providerRes = await ApiCall.get(
          `/tenants/${activeTenant?.tenantId}/subscriptions/${subscriptionId}/suppliers`,
          {
            headers: {
              Accept: 'application/json',
              Tenantkey: tenantKey,
            },
          }
        );
        const providers: Provider[] = get(
          providerRes,
          'data.supplierSubscriptions',
          []
        );
        providers.forEach(async (provider) => {
          const { data } = await ApiCall.get(
            `/tenants/${activeTenant?.tenantId}/subscriptions/${provider?.supplierSubscriptionID}/suppliers/${provider?.supplierID}`,
            {
              headers: {
                Accept: 'application/json',
                Tenantkey: tenantKey,
              },
            }
          );
          setSubscriptions([
            {
              ...subscription,
              providers: providers.map((provider) => ({
                ...provider,
                configurations: data.configurations,
              })),
            },
          ]);
        });
      });
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

  return { subscriptions, isLoading, error };
}
