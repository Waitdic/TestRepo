import { useState, useEffect, useCallback } from 'react';
import { get } from 'lodash';
import { useDispatch, useSelector } from 'react-redux';
//
import { RootState } from '@/store';
import ApiCall from '@/axios';
import {
  User,
  Tenant,
  Supplier,
  Account,
  SupplierFormFields,
  SupplierConfiguration,
} from '@/types';

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
      const user: User = {
        fullName: userData?.fullName,
        tenants: userData?.tenants.map((tenant: Tenant, idx: number) => ({
          ...tenant,
          isSelected: idx === 0,
        })),
      };
      dispatch.app.updateUser(user);
      if (user?.tenants.length > 0) {
        dispatch.app.updateTenantList(user?.tenants);
        setError(null);
      } else {
        dispatch.app.updateTenantList([]);
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

//* Accounts data fetch
export async function getAccounts(
  tenant: { id: number; key: string },
  onInit?: () => void,
  onSuccess?: (accounts: Account[]) => void,
  onFailed?: (error: string | null) => void
) {
  onInit?.();
  try {
    const subsRes = await ApiCall.get(`/tenants/${tenant.id}/subscriptions`, {
      headers: {
        Accept: 'application/json',
        Tenantkey: tenant.key,
      },
    });
    const accounts: Account[] = get(subsRes, 'data.subscriptions', []);
    onSuccess?.(accounts);
  } catch (err) {
    if (typeof err === 'string') {
      console.error(err.toUpperCase());
      onFailed?.(err.toUpperCase());
    } else if (err instanceof Error) {
      console.error(err.message);
      onFailed?.(err.message);
    }
  }
}

//* Accounts data fetch with suppliers
export async function getAccountsWithSuppliers(
  tenant: { id: number; key: string },
  onInit?: () => void,
  onSuccess?: (accounts: Account[]) => void,
  onFailed?: (error: string | null) => void
) {
  onInit?.();
  try {
    const subsRes = await ApiCall.get(`/tenants/${tenant.id}/subscriptions`, {
      headers: {
        Accept: 'application/json',
        Tenantkey: tenant.key,
      },
    });
    const accounts: Account[] = get(subsRes, 'data.subscriptions', []);
    accounts.forEach(async (account) => {
      const { subscriptionId } = account;
      const supplierRes = await ApiCall.get(
        `/tenants/${tenant.id}/subscriptions/${subscriptionId}/suppliers`,
        {
          headers: {
            Accept: 'application/json',
            Tenantkey: tenant.key,
          },
        }
      );
      const suppliersData: Supplier[] = get(
        supplierRes,
        'data.supplierSubscriptions',
        []
      );
      account.suppliers = suppliersData;
      onSuccess?.(accounts);
    });
  } catch (err) {
    if (typeof err === 'string') {
      console.error(err.toUpperCase());
      onFailed?.(err.toUpperCase());
    } else if (err instanceof Error) {
      console.error(err.message);
      onFailed?.(err.message);
    }
  }
}

//* Accounts data fetch with suppliers and configurations
export async function getAccountsWithSuppliersAndConfigurations(
  tenant: { id: number; key: string },
  onInit?: () => void,
  onSuccess?: (accounts: Account[]) => void,
  onFailed?: (error: string | null) => void
) {
  onInit?.();
  try {
    const subsRes = await ApiCall.get(`/tenants/${tenant.id}/subscriptions`, {
      headers: {
        Accept: 'application/json',
        Tenantkey: tenant.key,
      },
    });
    const accounts: Account[] = get(subsRes, 'data.subscriptions', []);
    accounts.forEach(async (account) => {
      const { subscriptionId } = account;
      const supplierRes = await ApiCall.get(
        `/tenants/${tenant.id}/subscriptions/${subscriptionId}/suppliers`,
        {
          headers: {
            Accept: 'application/json',
            Tenantkey: tenant.key,
          },
        }
      );
      const suppliersData: Supplier[] = get(
        supplierRes,
        'data.supplierSubscriptions',
        []
      );
      suppliersData.forEach(async (supplier) => {
        const { data } = await ApiCall.get(
          `/tenants/${tenant.id}/subscriptions/${account.subscriptionId}/suppliers/${supplier?.supplierID}`,
          {
            headers: {
              Accept: 'application/json',
              Tenantkey: tenant.key,
            },
          }
        );
        account.suppliers = [...(account?.suppliers || []), data];
        onSuccess?.(accounts);
      });
    });
  } catch (err) {
    if (typeof err === 'string') {
      console.error(err.toUpperCase());
      onFailed?.(err.toUpperCase());
    } else if (err instanceof Error) {
      console.error(err.message);
      onFailed?.(err.message);
    }
  }
}

//* Account data fetch with suppliers and configurations
export async function getAccountWithSupplierAndConfigurations(
  tenant: { id: number; key: string },
  subscriptionId: number,
  supplierId: number,
  onInit?: () => void,
  onSuccess?: (
    account: Account,
    supplier: Supplier,
    configurations: any[]
  ) => void,
  onFailed?: (error: string | null) => void
) {
  onInit?.();
  try {
    const subRes = ApiCall.get(
      `/tenants/${tenant.id}/subscriptions/${subscriptionId}`,
      {
        headers: {
          Accept: 'application/json',
          Tenantkey: tenant.key,
        },
      }
    );
    const supplierRes = ApiCall.get(
      `/tenants/${tenant.id}/subscriptions/${subscriptionId}/suppliers`,
      {
        headers: {
          Accept: 'application/json',
          Tenantkey: tenant.key,
        },
      }
    );
    const configurationsRes = ApiCall.get(
      `/tenants/${tenant.id}/subscriptions/${subscriptionId}/suppliers/${supplierId}`,
      {
        headers: {
          Accept: 'application/json',
          Tenantkey: tenant.key,
        },
      }
    );
    const fetchedDataRes = await Promise.all([
      subRes,
      supplierRes,
      configurationsRes,
    ]);
    const account = get(fetchedDataRes[0], 'data.subscription', null);
    const suppliers: Supplier[] = get(
      fetchedDataRes[1],
      'data.supplierSubscriptions',
      null
    );
    const supplier = suppliers?.find((supp) => supp?.supplierID === supplierId);
    const configurations = get(fetchedDataRes[2], 'data.configurations', []);
    if (account && supplier && configurations) {
      onSuccess?.(account, supplier, configurations);
    }
  } catch (err) {
    if (typeof err === 'string') {
      console.error(err.toUpperCase());
      onFailed?.(err.toUpperCase());
    } else if (err instanceof Error) {
      console.error(err.message);
      onFailed?.(err.message);
    }
  }
}

//* Fetch account by ID
export async function getAccountById(
  tenant: { id: number; key: string },
  subscriptionId: number,
  onInit: () => void,
  onSuccess: (account: Account) => void,
  onFailed: (error: string | null) => void
) {
  onInit();
  try {
    const res = await ApiCall.get(
      `/tenants/${tenant.id}/subscriptions/${subscriptionId}`,
      {
        headers: {
          Accept: 'application/json',
          Tenantkey: tenant.key,
        },
      }
    );
    const data = get(res, 'data.subscription', null);
    onSuccess(data);
  } catch (err) {
    if (typeof err === 'string') {
      onFailed(err.toUpperCase());
    } else if (err instanceof Error) {
      onFailed(err.message);
    }
  }
}

//* Fetch suppliers by Account
export async function getSuppliersByAccount(
  tenant: { id: number; key: string },
  subscriptionId: number,
  onInit: () => void,
  onSuccess: (suppliers: Supplier[]) => void,
  onFailed: (error: string | null) => void
) {
  onInit();
  try {
    const res = await ApiCall.get(
      `/tenants/${tenant.id}/subscriptions/${subscriptionId}/suppliers`,
      {
        headers: {
          Accept: 'application/json',
          Tenantkey: tenant.key,
        },
      }
    );
    const data = get(res, 'data.supplierSubscriptions', null);
    onSuccess(data);
  } catch (err) {
    if (typeof err === 'string') {
      onFailed(err.toUpperCase());
    } else if (err instanceof Error) {
      onFailed(err.message);
    }
  }
}

//* Fetch suppliers list
export async function getSuppliers(
  tenant: { id: number; key: string },
  onInit: () => void,
  onSuccess: (suppliers: Supplier[]) => void,
  onFailed: (error: string | null) => void
) {
  onInit();
  try {
    const res = await ApiCall.get(`/suppliers`, {
      headers: {
        Accept: 'application/json',
        Tenantkey: tenant.key,
      },
    });
    const data = get(res, 'data.suppliers', null);
    onSuccess(data);
  } catch (err) {
    if (typeof err === 'string') {
      onFailed(err.toUpperCase());
    } else if (err instanceof Error) {
      onFailed(err.message);
    }
  }
}

//* Fetch supplier by ID
export async function getSupplierById(
  tenant: { id: number; key: string },
  subscriptionId: number,
  supplierId: number,
  onInit: () => void,
  onSuccess: (supplier: Supplier) => void,
  onFailed: (error: string | null) => void
) {
  onInit();
  try {
    const res = await ApiCall.get(
      `/tenants/${tenant.id}/subscriptions/${subscriptionId}/suppliers/${supplierId}`,
      {
        headers: {
          Accept: 'application/json',
          Tenantkey: tenant.key,
        },
      }
    );
    const data = get(res, 'data', null);
    onSuccess(data);
  } catch (err) {
    if (typeof err === 'string') {
      onFailed(err.toUpperCase());
    } else if (err instanceof Error) {
      onFailed(err.message);
    }
  }
}

//* Fetch configurations by Supplier
export async function getConfigurationsBySupplier(
  tenant: { id: number; key: string },
  supplierId: number,
  onInit: () => void,
  onSuccess: (configurations: SupplierConfiguration[]) => void,
  onFailed: (error: string | null) => void
) {
  onInit();
  try {
    const configurationsRes = await ApiCall.get(`/suppliers/${supplierId}`, {
      headers: {
        Accept: 'application/json',
        Tenantkey: tenant.key,
      },
    });
    const configurations = get(configurationsRes, 'data.configurations', []);
    onSuccess(configurations);
  } catch (err) {
    if (typeof err === 'string') {
      onFailed(err.toUpperCase());
    } else if (err instanceof Error) {
      onFailed(err.message);
    }
  }
}

//* Update supplier data
export async function updateSupplier(
  tenant: { id: number; key: string },
  data: SupplierFormFields,
  onInit: () => void,
  onSuccess: (updatedSupplier: Supplier) => void,
  onFailed: (error: string) => void
) {
  const {
    account: subscriptionId,
    supplier: supplierId,
    configurations,
  } = data;
  onInit();

  const filteredConfigurations = Object.entries(configurations)
    .filter((config) => typeof config[1] !== 'object' && config)
    .map((config) => ({
      supplierSubscriptionAttributeId: Number(config[0]),
      value: config[1].toString(),
    }));

  try {
    const updatedSupplierRes = await ApiCall.request({
      method: 'PUT',
      url: `/tenants/${tenant.id}/subscriptions/${subscriptionId}/suppliers/${supplierId}/suppliersubscriptionattributes`,
      headers: {
        Tenantkey: tenant.key,
      },
      data: filteredConfigurations,
    });
    const updatedSupplier = get(updatedSupplierRes, 'data', null);
    onSuccess(updatedSupplier);
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

//* Create supplier data
export async function createSupplier(
  tenant: { id: number; key: string },
  subscriptionId: number,
  supplierId: number,
  data: SupplierFormFields,
  onInit: () => void,
  onSuccess: (updatedSupplier: Supplier) => void,
  onFailed: (error: string) => void
) {
  const { configurations } = data;
  onInit();

  const filteredConfigurations = Object.entries(configurations)
    .filter((config) => typeof config[1] !== 'object' && config)
    .map((config) => ({
      supplierAttributeId: Number(config[0]),
      value: config[1].toString(),
    }));

  try {
    const newSupplierRes = await ApiCall.request({
      method: 'POST',
      url: `/tenants/${tenant.id}/subscriptions/${subscriptionId}/suppliers/${supplierId}`,
      headers: {
        Tenantkey: tenant.key,
      },
      data: filteredConfigurations,
    });
    const newSupplier = get(newSupplierRes, 'data', null);
    onSuccess(newSupplier);
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
