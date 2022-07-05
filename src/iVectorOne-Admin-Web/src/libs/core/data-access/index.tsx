import { useState, useEffect, useCallback } from 'react';
import { get, uniqBy } from 'lodash';
import { useDispatch, useSelector } from 'react-redux';
//
import { RootState } from '@/store';
import ApiCall from '@/axios';
import { User, Tenant, Module, Provider, Subscription } from '@/types';

export function useCoreFetching() {
  const dispatch = useDispatch();
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
        tenants: userData?.tenants.map((tenant: Tenant, idx: number) => ({
          ...tenant,
          isSelected: idx === 0,
        })),
      };
      setUser(user);
      dispatch.app.updateUser(user);

      if (user?.tenants.length > 0) {
        setTenantList(user?.tenants);
        dispatch.app.updateTenantList(user?.tenants);
        setError(null);
      } else {
        setTenantList([]);
        dispatch.app.updateTenantList([]);
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

export function useIvoFetching() {
  const dispatch = useDispatch();

  const user = useSelector((state: RootState) => state.app.user);

  const [subscriptions, setSubscriptions] = useState<Subscription[]>([]);
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  const fetch = useCallback(async (user: User) => {
    try {
      const activeTenant = user?.tenants.find((tenant) => tenant.isSelected);
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
            `/tenants/${activeTenant?.tenantId}/subscriptions/${subscription.subscriptionId}/suppliers/${provider?.supplierID}`,
            {
              headers: {
                Accept: 'application/json',
                Tenantkey: tenantKey,
              },
            }
          );
          setSubscriptions((prevState) =>
            uniqBy(
              [
                ...prevState,
                {
                  ...subscription,
                  providers: uniqBy(
                    providers.map((provider) => ({
                      ...provider,
                      configurations: data.configurations,
                    })),
                    'supplierID'
                  ),
                },
              ],
              'subscriptionId'
            )
          );
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
      setSubscriptions([]);
      fetch(user);
    } else {
      setSubscriptions([]);
      setIsLoading(false);
    }
  }, [fetch, user]);

  useEffect(() => {
    dispatch.app.updateSubscriptions(subscriptions);
  }, [subscriptions]);

  return { isLoading, error };
}
