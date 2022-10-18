import { get } from 'lodash';
//
import ApiCall from '@/axios';
import type { Account, ApiError, Supplier } from '@/types';
import handleApiError from '@/utils/handleApiError';

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
    const accountsRes = await ApiCall.get(`/tenants/${tenant.id}/accounts`, {
      headers: {
        Accept: 'application/json',
        Tenantkey: tenant.key,
        UserKey: userKey,
      },
    });
    const accounts: Account[] = get(accountsRes, 'data.accounts', []);
    onSuccess?.(accounts);
  } catch (err: any) {
    const errorMessage = handleApiError(err as ApiError);
    onFailed?.(errorMessage);
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
    const subsRes = await ApiCall.get(`/tenants/${tenant.id}/accounts`, {
      headers: {
        Accept: 'application/json',
        Tenantkey: tenant.key,
        UserKey: userKey,
      },
    });
    const accounts: Account[] = get(subsRes, 'data.accounts', []);
    accounts.forEach(async (account) => {
      const { accountId } = account;
      const supplierRes = await ApiCall.get(
        `/tenants/${tenant.id}/accounts/${accountId}/suppliers`,
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
        'data.supplierAccounts',
        []
      );
      account.suppliers = suppliersData;
      onSuccess?.(accounts);
    });
  } catch (err: any) {
    const errorMessage = handleApiError(err as ApiError);
    onFailed?.(errorMessage);
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
    const subsRes = await ApiCall.get(`/tenants/${tenant.id}/accounts`, {
      headers: {
        Accept: 'application/json',
        Tenantkey: tenant.key,
        UserKey: userKey,
      },
    });
    const accounts: Account[] = get(subsRes, 'data.accounts', []);
    accounts.forEach(async (account) => {
      const { accountId } = account;
      const supplierRes = await ApiCall.get(
        `/tenants/${tenant.id}/accounts/${accountId}/suppliers`,
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
        'data.supplierAccounts',
        []
      );
      suppliersData.forEach(async (supplier) => {
        const { data } = await ApiCall.get(
          `/tenants/${tenant.id}/accounts/${account.accountId}/suppliers/${supplier?.supplierID}`,
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
  } catch (err: any) {
    const errorMessage = handleApiError(err as ApiError);
    onFailed?.(errorMessage);
  }
}

//* Account data fetch with suppliers and configurations
export async function getAccountWithSupplierAndConfigurations(
  tenant: { id: number; key: string },
  userKey: string,
  accountId: number,
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
    const subRes = ApiCall.get(`/tenants/${tenant.id}/accounts/${accountId}`, {
      headers: {
        Accept: 'application/json',
        Tenantkey: tenant.key,
        UserKey: userKey,
      },
    });
    const supplierRes = ApiCall.get(
      `/tenants/${tenant.id}/accounts/${accountId}/suppliers`,
      {
        headers: {
          Accept: 'application/json',
          Tenantkey: tenant.key,
          UserKey: userKey,
        },
      }
    );
    const configurationsRes = ApiCall.get(
      `/tenants/${tenant.id}/accounts/${accountId}/suppliers/${supplierId}`,
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
      accountId: number;
      accountSuppliers: Supplier[];
    } = get(fetchedDataRes[1], 'data', null);
    const supplier = suppliers?.accountSuppliers?.find(
      (supp) => supp?.supplierID === supplierId
    );
    const configurations = get(fetchedDataRes[2], 'data.configurations', []);
    onSuccess?.(account, configurations, supplier);
  } catch (err: any) {
    const errorMessage = handleApiError(err as ApiError);
    onFailed?.(errorMessage);
  }
}

//* Fetch account by ID
export async function getAccountById(
  tenant: { id: number; key: string },
  userKey: string,
  accountId: number,
  onInit: () => void,
  onSuccess: (account: Account) => void,
  onFailed: (error: string | null) => void
) {
  onInit();
  try {
    const res = await ApiCall.get(
      `/tenants/${tenant.id}/accounts/${accountId}`,
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
  } catch (err: any) {
    const errorMessage = handleApiError(err as ApiError);
    onFailed?.(errorMessage);
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
      url: `/tenants/${tenant.id}/accounts`,
      headers: {
        Accept: 'application/json',
        Tenantkey: tenant.key,
        UserKey: userKey,
      },
      data,
    });
    onSuccess();
  } catch (err: any) {
    const errorMessage = handleApiError(err as ApiError);
    onFailed?.(errorMessage);
  }
}

//* Update account
export async function updateAccount(
  tenant: { id: number; key: string },
  userKey: string,
  accountId: number,
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
      url: `/tenants/${tenant.id}/accounts/${accountId}`,
      headers: {
        Accept: 'application/json',
        Tenantkey: tenant.key,
        UserKey: userKey,
      },
      data,
    });
    onSuccess();
  } catch (err: any) {
    const errorMessage = handleApiError(err as ApiError);
    onFailed?.(errorMessage);
  }
}

//* Delete account
export async function deleteAccount(
  tenant: { id: number; key: string },
  userKey: string,
  accountId: number,
  onInit: () => void,
  onSuccess: () => void,
  onFailed: (error: string | null) => void
) {
  onInit();
  try {
    await ApiCall.request({
      method: 'DELETE',
      url: `/tenants/${tenant.id}/accounts/${accountId}`,
      headers: {
        Accept: 'application/json',
        Tenantkey: tenant.key,
        UserKey: userKey,
      },
    });
    onSuccess();
  } catch (err: any) {
    const errorMessage = handleApiError(err as ApiError);
    onFailed?.(errorMessage);
  }
}
