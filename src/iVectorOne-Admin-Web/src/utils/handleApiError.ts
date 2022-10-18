import type { ApiError } from '@/types';
import { get } from 'lodash';

const handleApiError = (error: ApiError) => {
  const errorMessage = get(error, 'response.data', null);
  if (errorMessage) {
    const { title, detail, instance } = errorMessage;
    return `${title} ${detail} ${instance}`;
  }
  return 'Something went wrong';
};

export default handleApiError;
