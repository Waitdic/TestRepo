import { useState, useEffect, useCallback } from 'react';
import { get, uniqBy } from 'lodash';
import { useDispatch, useSelector } from 'react-redux';
//
import { RootState } from '@/store';
import ApiCall from '@/axios';
import { User, Tenant, Module, Supplier, Subscription } from '@/types';

export function useCoreFetching() {
  const dispatch = useDispatch();
  const { username } = useSelector((state: RootState) => state.app.awsAmplify);

  const [user, setUser] = useState<User>(null);
  const [moduleList, setModuleList] = useState<Module[]>([]);
  const [tenantList, setTenantList] = useState<Tenant[]>([]);
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

      dispatch.app.setIsLoading(false);
    } catch (error) {
      if (typeof error === 'string') {
        console.error(error.toUpperCase());
        setError(error.toUpperCase());
      } else if (error instanceof Error) {
        console.error(error.message);
        setError(error.message);
      }
      dispatch.app.setIsLoading(false);
    }
  }, []);

  useEffect(() => {
    if (username) {
      fetch(username);
    }
  }, [fetch, username]);

  return { user, moduleList, tenantList, error };
}

export function useIvoFetching() {
  const dispatch = useDispatch();

  const user = useSelector((state: RootState) => state.app.user);

  const [subscriptions, setSubscriptions] = useState<Subscription[]>([]);
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
        const supplierRes = await ApiCall.get(
          `/tenants/${activeTenant?.tenantId}/subscriptions/${subscriptionId}/suppliers`,
          {
            headers: {
              Accept: 'application/json',
              Tenantkey: tenantKey,
            },
          }
        );
        const suppliers: Supplier[] = get(
          supplierRes,
          'data.supplierSubscriptions',
          []
        );
        suppliers.forEach(async (supplier) => {
          const { data } = await ApiCall.get(
            `/tenants/${activeTenant?.tenantId}/subscriptions/${subscription.subscriptionId}/suppliers/${supplier?.supplierID}`,
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
                  suppliers: uniqBy(
                    suppliers.map((supplier) => ({
                      ...supplier,
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
      dispatch.app.setIsLoading(false);
    } catch (error) {
      if (typeof error === 'string') {
        console.error(error.toUpperCase());
        setError(error.toUpperCase());
      } else if (error instanceof Error) {
        console.error(error.message);
        setError(error.message);
      }
      dispatch.app.setIsLoading(false);
    }
  }, []);

  useEffect(() => {
    if (!!user?.tenants?.length) {
      setSubscriptions([]);
      fetch(user);
    } else {
      setSubscriptions([]);
      dispatch.app.setIsLoading(false);
    }
  }, [fetch, user]);

  useEffect(() => {
    dispatch.app.updateSubscriptions(subscriptions);
  }, [subscriptions]);

  return { error };
}
