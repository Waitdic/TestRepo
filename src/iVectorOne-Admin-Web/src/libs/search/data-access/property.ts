import ApiCall from '@/axios';
import { Property, SearchRequestData } from '@/types';
import handleApiError from '@/utils/handleApiError';

export async function getPropertiesById({
  userKey,
  tenant,
  accountId,
  query,
  onInit,
  onSuccess,
  onFailed,
}: {
  userKey: string;
  tenant: { id: number; key: string };
  accountId: number;
  query: string;
  onInit: () => void;
  onSuccess: (data: Property[]) => void;
  onFailed: (error: string, instance?: string) => void;
}) {
  onInit();
  try {
    const {
      data: { properties },
    } = await ApiCall.request({
      method: 'GET',
      url: `/tenants/${
        tenant.id
      }/accounts/${accountId}/properties?query=${query.toLowerCase()}`,
      headers: {
        Tenantkey: tenant.key,
        UserKey: userKey,
      },
    });
    onSuccess(properties);
  } catch (error) {
    console.error(error);
    const { message, instance } = handleApiError(error as any);
    onFailed(message, instance);
  }
}

export async function searchByProperty({
  userKey,
  tenant,
  accountId,
  requestData,
  onInit,
  onSuccess,
  onFailed,
}: {
  userKey: string;
  tenant: { id: number; key: string };
  accountId: number;
  requestData: SearchRequestData;
  onInit: () => void;
  onSuccess: (data: any) => void;
  onFailed: (error: string, instance?: string) => void;
}) {
  onInit();
  try {
    const { data } = await ApiCall.request({
      method: 'POST',
      url: `/tenants/${tenant.id}/accounts/${accountId}/search`,
      headers: {
        Tenantkey: tenant.key,
        UserKey: userKey,
      },
      data: requestData,
    });
    onSuccess(data);
    console.log(data);
  } catch (error) {
    console.error(error);
    const { message, instance } = handleApiError(error as any);
    onFailed(message, instance);
  }
}
