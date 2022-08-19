import ApiCall from '@/axios';
import { Supplier, SupplierConfiguration, SupplierFormFields } from '@/types';
import { get } from 'lodash';

//* Fetch suppliers by Account
export async function getSuppliersByAccount(
  tenant: { id: number; key: string },
  userKey: string,
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
          UserKey: userKey,
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
  userKey: string,
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
        UserKey: userKey,
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
  userKey: string,
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

//* Fetch configurations by Supplier
export async function getConfigurationsBySupplier(
  tenant: { id: number; key: string },
  userKey: string,
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
        UserKey: userKey,
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
  userKey: string,
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
      supplierSubscriptionAttributeId: Number(config[0]),
      value: config[1].toString(),
    }));

  try {
    const updatedSupplierRes = await ApiCall.request({
      method: 'PUT',
      url: `/tenants/${tenant.id}/subscriptions/${subscriptionId}/suppliers/${supplierId}/suppliersubscriptionattributes`,
      headers: {
        Tenantkey: tenant.key,
        UserKey: userKey,
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
  userKey: string,
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
        UserKey: userKey,
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

//* Delete supplier
export async function deleteSupplier(
  tenant: { id: number; key: string },
  userKey: string,
  subscriptionId: number,
  supplierId: number,
  onInit: () => void,
  onSuccess: () => void,
  onFailed: (error: string) => void
) {
  onInit();

  try {
    await ApiCall.request({
      method: 'DELETE',
      url: `/tenants/${tenant.id}/subscriptions/${subscriptionId}/suppliers/${supplierId}`,
      headers: {
        Tenantkey: tenant.key,
        UserKey: userKey,
      },
    });
    onSuccess();
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
