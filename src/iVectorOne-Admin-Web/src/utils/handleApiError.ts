import { get } from 'lodash';

const handleApiError = (
  error: any
): { message: string; instance?: string; title?: string } => {
  const errorMessage = get(error, 'response.data', null);

  if (errorMessage) {
    const { title, detail, instance } = errorMessage;
    const message = `${title} ${detail} ${instance}`;

    return {
      message: message.trim() !== '' ? message : 'Unexpected API error',
      instance,
      title,
    };
  }
  return { message: error.message || 'Unexpected API error' };
};

export default handleApiError;
