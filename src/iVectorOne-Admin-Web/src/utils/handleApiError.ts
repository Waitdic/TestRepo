import type { ApiError } from '@/types';
import { get } from 'lodash';

const handleApiError = (
  error: ApiError
): { message: string; instance?: string } => {
  const errorMessage = get(error, 'response.data', null);
  if (errorMessage) {
    const { title, detail, instance } = errorMessage;
    return { message: `${title} ${detail} ${instance}`, instance };
  }
  return { message: 'Something went wrong' };
};

export default handleApiError;
