import ApiCall from '@/axios';
import { UserFormFields } from '../user/create';

//* Create User
export async function createUser(
  userTenantKey: string,
  data: UserFormFields,
  onInit: () => void,
  onSuccess: () => void,
  onFailed: (error: string) => void
) {
  onInit();
  try {
    await ApiCall.request({
      method: 'POST',
      url: `/users`,
      headers: {
        Accept: 'application/json',
        Tenantkey: userTenantKey,
        UserKey: userTenantKey,
      },
      data,
    });
    onSuccess();
  } catch (err) {
    if (typeof err === 'string') {
      console.error(err.toUpperCase());
      onFailed(err.toUpperCase());
    } else if (err instanceof Error) {
      console.error(err.message);
      onFailed(err.message);
    }
  }
}
