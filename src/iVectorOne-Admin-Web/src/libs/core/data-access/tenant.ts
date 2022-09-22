import { get } from 'lodash';
//
import ApiCall from '@/axios';
import type { Tenant } from '@/types';

//* Fetch tenant list
export async function getTenants(
  userKey: string,
  onInit: () => void,
  onSuccess: (tenants: Tenant[]) => void,
  onFailed: (error: string | null) => void
) {
  onInit();
  try {
    const res = await ApiCall.get(`/tenants`, {
      headers: {
        Tenantkey: userKey,
        UserKey: userKey,
      },
    });
    const data = get(res, 'data.tenants', null);
    onSuccess(data);
  } catch (err) {
    if (typeof err === 'string') {
      onFailed(err.toUpperCase());
    } else if (err instanceof Error) {
      onFailed(err.message);
    }
  }
}

//* Fetch tenant by id
export async function getTenantById(
  tenant: { id: number; key: string },
  userKey: string,
  tenantId: number,
  onInit: () => void,
  onSuccess: (tenant: Tenant) => void,
  onFailed: (error: string | null) => void
) {
  onInit();
  try {
    const res = await ApiCall.get(`/tenants/${tenantId}`, {
      headers: {
        Tenantkey: tenant.key,
        UserKey: userKey,
      },
    });
    const data = get(res, 'data.tenant', null);
    onSuccess(data);
  } catch (err) {
    if (typeof err === 'string') {
      onFailed(err.toUpperCase());
    } else if (err instanceof Error) {
      onFailed(err.message);
    }
  }
}

//* Update tenant data
export async function updateTenant(
  userTenantKey: string,
  userKey: string,
  tenantId: number,
  data: Tenant,
  onInit: () => void,
  onSuccess: (updatedTenant: Tenant) => void,
  onFailed: (error: string) => void
) {
  onInit();

  try {
    const updatedTenantRes = await ApiCall.request({
      method: 'PUT',
      url: `/tenants/${tenantId}`,
      headers: {
        Accept: 'application/json',
        Tenantkey: userTenantKey,
        UserKey: userKey,
      },
      data,
    });
    const updatedTenant = get(updatedTenantRes, 'data', null);
    onSuccess(updatedTenant);
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

//* Update tenant status
export async function updateTenantStatus(
  userTenantKey: string,
  userKey: string,
  tenantId: number,
  status: boolean,
  onInit: () => void,
  onSuccess: () => void,
  onFailed: (error: string) => void
) {
  onInit();
  try {
    await ApiCall.request({
      method: 'PUT',
      url: `/tenants/${tenantId}/${status ? 'enable' : 'disable'}`,
      headers: {
        Accept: 'application/json',
        Tenantkey: userTenantKey,
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

//* Delete tenant by id
export async function deleteTenant(
  userTenantKey: string,
  userKey: string,
  tenantId: number,
  onInit: () => void,
  onSuccess: () => void,
  onFailed: (error: string) => void
) {
  onInit();
  try {
    await ApiCall.request({
      method: 'DELETE',
      url: `/tenants/${tenantId}`,
      headers: {
        Accept: 'application/json',
        Tenantkey: userTenantKey,
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

//* Create tenant
export async function createTenant(
  userKey: string,
  data: Tenant,
  onInit: () => void,
  onSuccess: (newTenant: { tenantId: number; success: boolean }) => void,
  onFailed: (error: string) => void
) {
  onInit();
  try {
    const newTenant = await ApiCall.request({
      method: 'POST',
      url: `/tenants`,
      headers: {
        Accept: 'application/json',
        Tenantkey: userKey,
        UserKey: userKey,
      },
      data,
    });
    onSuccess(newTenant.data);
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
