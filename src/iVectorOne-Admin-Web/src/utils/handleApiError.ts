import type { ApiError } from '@/types';

const handleApiError = (error: ApiError) => {
  const { title, detail, instance } = error;

  return `${title} ${detail} ${instance}`;
};

export default handleApiError;
