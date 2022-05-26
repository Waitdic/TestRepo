import { FormattedMessage } from 'react-intl';

export const translateMessage = (
  msgId: string | undefined,
  textVal?: string,
  numberVal?: number
) => <FormattedMessage id={msgId} values={{ textVal, numberVal }} />;
