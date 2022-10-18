import { get } from 'lodash';
//
import ApiCall from '@/axios';
import type { ApiError, Tenant } from '@/types';
import handleApiError from '@/utils/handleApiError';

//* Fetch tenant list
export async function getTenants(
  userKey: string,
  tenantKey: string,
  onInit: () => void,
  onSuccess: (tenants: Tenant[]) => void,
  onFailed: (error: string | null) => void
) {
  onInit();
  try {
    const res = await ApiCall.get(`/tenants`, {
      headers: {
        Tenantkey: tenantKey,
        UserKey: userKey,
      },
    });
    const data = get(res, 'data.tenants', null);
    onSuccess(data);
  } catch (err: any) {
    const errorMessage = handleApiError(err as ApiError);
    onFailed?.(errorMessage);
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
  } catch (err: any) {
    const errorMessage = handleApiError(err as ApiError);
    onFailed?.(errorMessage);
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
  } catch (err: any) {
    const errorMessage = handleApiError(err as ApiError);
    onFailed?.(errorMessage);
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
      method: 'POST',
      url: `/tenants/${tenantId}/${status ? 'enable' : 'disable'}`,
      headers: {
        Accept: 'application/json',
        Tenantkey: userTenantKey,
        UserKey: userKey,
      },
    });
    onSuccess();
  } catch (err: any) {
    const errorMessage = handleApiError(err as ApiError);
    onFailed?.(errorMessage);
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
  } catch (err: any) {
    const errorMessage = handleApiError(err as ApiError);
    onFailed?.(errorMessage);
  }
}

//* Create tenant
export async function createTenant(
  userTenantKey: string,
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
        Tenantkey: userTenantKey,
        UserKey: userKey,
      },
      data,
    });
    onSuccess(newTenant.data);
  } catch (err: any) {
    const errorMessage = handleApiError(err as ApiError);
    onFailed?.(errorMessage);
  }
}
