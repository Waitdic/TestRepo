import { Auth } from 'aws-amplify';
import { get } from 'lodash';

export const getAwsJwtToken = async () => {
  let jwtToken: string | null = null;
  try {
    const res = await Auth.currentSession();
    const token = get(res, 'accessToken.jwtToken', null);
    jwtToken = token;
  } catch (error) {
    console.error(error);
  }

  return jwtToken;
};
