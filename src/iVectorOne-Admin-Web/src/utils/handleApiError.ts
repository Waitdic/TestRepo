import type { ApiError } from '@/types';
import { get } from 'lodash';

const handleApiError = (
  error: ApiError
): { message: string; instance?: string } => {
  const errorMessage = get(error, 'response.data', null);
  if (errorMessage) {
    const { title, detail, instance } = errorMessage;
    const message = `${title} ${detail} ${instance}`;
    console.log(message);

    return {
      message: message.trim() !== '' ? message : 'Something went wrong',
      instance,
    };
  }
  return { message: 'Something went wrong' };
};

export default handleApiError;
