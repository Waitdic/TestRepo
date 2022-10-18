import { get } from 'lodash';
//
import ApiCall from '@/axios';
import {
  ApiError,
  Supplier,
  SupplierConfiguration,
  SupplierFormFields,
} from '@/types';
import handleApiError from '@/utils/handleApiError';

//* Fetch suppliers by Account
export async function getSuppliersByAccount(
  tenant: { id: number; key: string },
  userKey: string,
  accountId: number,
  onInit: () => void,
  onSuccess: (suppliers: Supplier[]) => void,
  onFailed: (error: string | null, instance?: string) => void
) {
  onInit();
  try {
    const res = await ApiCall.get(
      `/tenants/${tenant.id}/accounts/${accountId}/suppliers`,
      {
        headers: {
          Accept: 'application/json',
          Tenantkey: tenant.key,
          UserKey: userKey,
        },
      }
    );
    const data = get(res, 'data.accountSuppliers', null);
    onSuccess(data);
  } catch (err: any) {
    const { message, instance } = handleApiError(err as ApiError);
    onFailed?.(message, instance);
  }
}

//* Fetch suppliers list
export async function getSuppliers(
  tenant: { id: number; key: string },
  userKey: string,
  onInit: () => void,
  onSuccess: (suppliers: Supplier[]) => void,
  onFailed: (error: string | null, instance?: string) => void
) {
  onInit();
  try {
    const res = await ApiCall.get(`/suppliers`, {
      headers: {
        Accept: 'application/json',
        Tenantkey: tenant.key,
        UserKey: userKey,
      },
    });
    const data = get(res, 'data.suppliers', null);
    onSuccess(data);
  } catch (err: any) {
    const { message, instance } = handleApiError(err as ApiError);
    onFailed?.(message, instance);
  }
}

//* Fetch supplier by ID
export async function getSupplierById(
  tenant: { id: number; key: string },
  userKey: string,
  accountId: number,
  supplierId: number,
  onInit: () => void,
  onSuccess: (supplier: Supplier) => void,
  onFailed: (error: string | null, instance?: string) => void
) {
  onInit();
  try {
    const res = await ApiCall.get(
      `/tenants/${tenant.id}/accounts/${accountId}/suppliers/${supplierId}`,
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
    const { message, instance } = handleApiError(err as ApiError);
    onFailed?.(message, instance);
  }
}

//* Fetch configurations by Supplier
export async function getConfigurationsBySupplier(
  tenant: { id: number; key: string },
  userKey: string,
  supplierId: number,
  onInit: () => void,
  onSuccess: (configurations: SupplierConfiguration[]) => void,
  onFailed: (error: string | null, instance?: string) => void
) {
  onInit();
  try {
    const configurationsRes = await ApiCall.get(`/suppliers/${supplierId}`, {
      headers: {
        Accept: 'application/json',
        Tenantkey: tenant.key,
        UserKey: userKey,
      },
    });
    const configurations = get(configurationsRes, 'data.configurations', []);
    onSuccess(configurations);
  } catch (err: any) {
    const { message, instance } = handleApiError(err as ApiError);
    onFailed?.(message, instance);
  }
}

//* Update supplier data
export async function updateSupplier({
  tenant,
  userKey,
  accountId,
  supplierId,
  data,
  onInit,
  onSuccess,
  onFailed,
}: {
  tenant: { id: number; key: string };
  userKey: string;
  accountId: number;
  supplierId: number;
  data: SupplierFormFields;
  onInit: () => void;
  onSuccess: (updatedSupplier: Supplier) => void;
  onFailed: (error: string, instance?: string) => void;
}) {
  const { configurations } = data;
  onInit();

  const filteredConfigurations = Object.entries(configurations)
    .filter((config) => typeof config[1] !== 'object' && config)
    .map((config) => ({
      accountSupplierAttributeID: Number(config[0]),
      value: config[1].toString(),
    }));

  try {
    const updatedSupplierRes = await ApiCall.request({
      method: 'PUT',
      url: `/tenants/${tenant.id}/accounts/${accountId}/suppliers/${supplierId}/accountsupplierattributes`,
      headers: {
        Tenantkey: tenant.key,
        UserKey: userKey,
      },
      data: filteredConfigurations,
    });
    const updatedSupplier = get(updatedSupplierRes, 'data', null);
    onSuccess(updatedSupplier);
  } catch (err: any) {
    const { message, instance } = handleApiError(err as ApiError);
    onFailed?.(message, instance);
  }
}

//* Create supplier data
export async function createSupplier({
  tenant,
  userKey,
  accountId,
  supplierId,
  data,
  onInit,
  onSuccess,
  onFailed,
}: {
  tenant: { id: number; key: string };
  userKey: string;
  accountId: number;
  supplierId: number;
  data: SupplierFormFields;
  onInit: () => void;
  onSuccess: (updatedSupplier: Supplier) => void;
  onFailed: (error: string, instance?: string) => void;
}) {
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
      url: `/tenants/${tenant.id}/accounts/${accountId}/suppliers/${supplierId}`,
      headers: {
        Tenantkey: tenant.key,
        UserKey: userKey,
      },
      data: filteredConfigurations,
    });
    const newSupplier = get(newSupplierRes, 'data', null);
    onSuccess(newSupplier);
  } catch (err: any) {
    const { message, instance } = handleApiError(err as ApiError);
    onFailed?.(message, instance);
  }
}

//* Delete supplier
export async function deleteSupplier(
  tenant: { id: number; key: string },
  userKey: string,
  accountId: number,
  supplierId: number,
  onInit: () => void,
  onSuccess: () => void,
  onFailed: (error: string, instance?: string) => void
) {
  onInit();

  try {
    await ApiCall.request({
      method: 'DELETE',
      url: `/tenants/${tenant.id}/accounts/${accountId}/suppliers/${supplierId}`,
      headers: {
        Tenantkey: tenant.key,
        UserKey: userKey,
      },
    });
    onSuccess();
  } catch (err: any) {
    const { message, instance } = handleApiError(err as ApiError);
    onFailed?.(message, instance);
  }
}
