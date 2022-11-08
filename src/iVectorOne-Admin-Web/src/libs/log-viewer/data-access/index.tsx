import { isArray } from 'lodash';
//
import type { ApiError, LogEntries, LogViewerFilters } from '@/types';
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

export async function getFilteredLogEntries({
  tenant,
  userKey,
  accountId,
  filters,
  onInit,
  onSuccess,
  onFailed,
}: {
  tenant: { id: number; key: string };
  userKey: string;
  accountId: number;
  filters: LogViewerFilters;
  onInit: () => void;
  onSuccess: (logEntries: LogEntries[]) => void;
  onFailed: (message: string, instance?: string, title?: string) => void;
}) {
  const queryParams = () => {
    const dates = filters.logDateRange;
    let startDate = '';
    let endDate = '';

    if (isArray(dates)) {
      const start = new Date(dates[0]);
      const end = new Date(dates[1] || start);
      startDate = start.toISOString().split('T')[0];
      endDate = end.toISOString().split('T')[0];
    }

    return `Supplier=${filters.supplier}&StartDate=${startDate}&endDate=${endDate}&enviroment=${filters.system}&type=${filters.type}&status=${filters.responseSuccess}`;
  };

  onInit();
  try {
    const {
      data: { logEntries, success },
    } = await ApiCall.request({
      method: 'GET',
      url: `/tenants/${tenant.id}/accounts/${accountId}/logs?${queryParams()}`,
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
          succesful,
          responseTime,
          supplierBookingReference,
          leadGuestName,
        }: LogEntries) => ({
          timestamp,
          supplierName,
          type,
          succesful,
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
