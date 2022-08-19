import { get } from 'lodash';
//
import ApiCall from '@/axios';
import type { Account, Supplier } from '@/types';

//* Accounts data fetch
export async function getAccounts(
  tenant: { id: number; key: string },
  userKey: string,
  onInit?: () => void,
  onSuccess?: (accounts: Account[]) => void,
  onFailed?: (error: string | null) => void
) {
  onInit?.();
  try {
    const accountsRes = await ApiCall.get(
      `/tenants/${tenant.id}/subscriptions`,
      {
        headers: {
          Accept: 'application/json',
          Tenantkey: tenant.key,
          UserKey: userKey,
        },
      }
    );
    const accounts: Account[] = get(accountsRes, 'data.subscriptions', []);
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
  userKey: string,
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
        UserKey: userKey,
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
            UserKey: tenant.key,
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
  userKey: string,
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
        UserKey: userKey,
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
            UserKey: userKey,
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
              UserKey: userKey,
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
  userKey: string,
  subscriptionId: number,
  supplierId: number,
  onInit?: () => void,
  onSuccess?: (
    account: Account,
    configurations: any[],
    supplier?: Supplier
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
          UserKey: userKey,
        },
      }
    );
    const supplierRes = ApiCall.get(
      `/tenants/${tenant.id}/subscriptions/${subscriptionId}/suppliers`,
      {
        headers: {
          Accept: 'application/json',
          Tenantkey: tenant.key,
          UserKey: userKey,
        },
      }
    );
    const configurationsRes = ApiCall.get(
      `/tenants/${tenant.id}/subscriptions/${subscriptionId}/suppliers/${supplierId}`,
      {
        headers: {
          Accept: 'application/json',
          Tenantkey: tenant.key,
          UserKey: userKey,
        },
      }
    );

    const fetchedDataRes = await Promise.all([
      subRes,
      supplierRes,
      configurationsRes,
    ]);
    const account = get(fetchedDataRes[0], 'data', null);
    const suppliers: {
      subscriptionId: number;
      supplierSubscriptions: Supplier[];
    } = get(fetchedDataRes[1], 'data', null);
    const supplier = suppliers?.supplierSubscriptions?.find(
      (supp) => supp?.supplierID === supplierId
    );
    const configurations = get(fetchedDataRes[2], 'data.configurations', []);
    onSuccess?.(account, configurations, supplier);
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
  userKey: string,
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
          UserKey: userKey,
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

//* Create account
export async function createAccount(
  tenant: { id: number; key: string },
  userKey: string,
  data: {
    UserName: string;
    PropertyTpRequestLimit: string;
    SearchTimeoutSeconds: string;
    CurrencyCode: string;
  },
  onInit: () => void,
  onSuccess: () => void,
  onFailed: (error: string | null) => void
) {
  onInit();
  try {
    await ApiCall.request({
      method: 'POST',
      url: `/tenants/${tenant.id}/subscriptions`,
      headers: {
        Accept: 'application/json',
        Tenantkey: tenant.key,
        UserKey: userKey,
      },
      data,
    });
    onSuccess();
  } catch (err) {
    if (typeof err === 'string') {
      onFailed(err.toUpperCase());
    } else if (err instanceof Error) {
      onFailed(err.message);
    }
  }
}

//* Update account
export async function updateAccount(
  tenant: { id: number; key: string },
  userKey: string,
  subscriptionId: number,
  data: {
    UserName: string;
    Password: string;
    PropertyTpRequestLimit: string;
    SearchTimeoutSeconds: string;
    CurrencyCode: string;
  },
  onInit: () => void,
  onSuccess: () => void,
  onFailed: (error: string | null) => void
) {
  onInit();
  try {
    await ApiCall.request({
      method: 'PUT',
      url: `/tenants/${tenant.id}/subscriptions/${subscriptionId}`,
      headers: {
        Accept: 'application/json',
        Tenantkey: tenant.key,
        UserKey: userKey,
      },
      data,
    });
    onSuccess();
  } catch (err) {
    if (typeof err === 'string') {
      onFailed(err.toUpperCase());
    } else if (err instanceof Error) {
      onFailed(err.message);
    }
  }
}

//* Delete account
export async function deleteAccount(
  tenant: { id: number; key: string },
  userKey: string,
  subscriptionId: number,
  onInit: () => void,
  onSuccess: () => void,
  onFailed: (error: string | null) => void
) {
  onInit();
  try {
    await ApiCall.request({
      method: 'DELETE',
      url: `/tenants/${tenant.id}/subscriptions/${subscriptionId}`,
      headers: {
        Accept: 'application/json',
        Tenantkey: tenant.key,
        UserKey: userKey,
      },
    });
    onSuccess();
  } catch (err) {
    if (typeof err === 'string') {
      onFailed(err.toUpperCase());
    } else if (err instanceof Error) {
      onFailed(err.message);
    }
  }
}
