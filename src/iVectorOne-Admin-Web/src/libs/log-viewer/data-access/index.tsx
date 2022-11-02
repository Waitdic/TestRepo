import type { ApiError, LogEntries } from '@/types';
import ApiCall from '@/axios';
import handleApiError from '@/utils/handleApiError';

export async function getBookingsLogEntries({
  tenant,
  userKey,
  accountId,
  searchQuery,
  onInit,
  onSuccess,
  onFailed,
}: {
  tenant: { id: number; key: string };
  userKey: string;
  accountId: number;
  searchQuery: string;
  onInit: () => void;
  onSuccess: (logEntries: LogEntries[]) => void;
  onFailed: (message: string, instance?: string, title?: string) => void;
}) {
  onInit();
  try {
    const {
      data: { logEntries, success },
    } = await ApiCall.request({
      method: 'GET',
      url: `/tenants/${tenant.id}/accounts/${accountId}/bookings?query=${searchQuery}`,
      headers: {
        Tenantkey: tenant.key,
        UserKey: userKey,
      },
    });
    if (success) {
      const logEntriesFormatted = logEntries.map(
        ({
          timestamp,
          supplierName,
          type,
          responseTime,
          supplierBookingReference,
          leadGuestName,
        }: LogEntries) => ({
          timestamp,
          supplierName,
          type,
          responseTime,
          supplierBookingReference,
          leadGuestName,
        })
      );
      onSuccess(logEntriesFormatted);
    }
  } catch (error) {
    const { message, instance, title } = handleApiError(error as ApiError);
    onFailed(message, instance, title);
  }
}
