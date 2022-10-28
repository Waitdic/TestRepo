import ApiCall from '@/axios';
import type {
  Property,
  SearchRequestData,
  SupplierSearchResults,
} from '@/types';
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
  onSuccess: (data: SupplierSearchResults[]) => void;
  onFailed: (error: string, instance?: string) => void;
}) {
  onInit();
  try {
    const {
      data: { requestKey },
    } = await ApiCall.request({
      method: 'POST',
      url: `/tenants/${tenant.id}/accounts/${accountId}/search`,
      headers: {
        Tenantkey: tenant.key,
        UserKey: userKey,
      },
      data: requestData,
    });

    let timerCount = 0;
    const timer = setInterval(async () => {
      const res = await ApiCall.request({
        method: 'GET',
        url: `/tenants/${tenant.id}/accounts/${accountId}/search?q=${requestKey}`,
        headers: {
          Tenantkey: tenant.key,
          UserKey: userKey,
        },
        data: requestData,
      });
      if (res.status === 200) {
        onSuccess(res.data.results);
        console.log(res.data);
        clearInterval(timer);
      }
      if (timerCount >= 24) {
        console.error('Search timeout');
        onFailed('Search timeout');
        clearInterval(timer);
      }
      timerCount++;
    }, 5000);
  } catch (error) {
    console.error(error);
    const { message, instance } = handleApiError(error as any);
    onFailed(message, instance);
  }
}
