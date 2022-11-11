import type { DashboardChartData } from '@/types';
import ApiCall from '@/axios';
import handleApiError from '@/utils/handleApiError';

export async function getDashboardChartData({
  userKey,
  tenant,
  accountId,
  onInit,
  onSuccess,
  onFailed,
}: {
  userKey: string;
  tenant: {
    id: number;
    key: string;
  };
  accountId: number;
  onInit: () => void;
  onSuccess: (data: DashboardChartData) => void;
  onFailed: (message: string, instance?: string) => void;
}) {
  onInit();

  try {
    const { data } = await ApiCall.request({
      method: 'GET',
      url: `/tenants/${tenant.id}/accounts/${accountId}/dashboard`,
      headers: {
        Tenantkey: tenant.key,
        UserKey: userKey,
      },
    });
    onSuccess(data);
  } catch (error) {
    const { message, instance } = handleApiError(error as any);
    onFailed(message, instance);
  }
}
