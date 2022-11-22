const JSONisValid = (str: string) => {
  try {
    JSON.parse(str);
  } catch (e) {
    return false;
  }
  return true;
};

const getLogDetailsPayloadPopupJSON = (
  rawLog: string,
  variant: 'request' | 'response'
) => {
  let log = rawLog;

  if (variant === 'request') {
    log = log.split('**REQUEST**')[1];
  }
  if (variant === 'response') {
    log = log.split('**RESPONSE**')[1];
  }

  if (!JSONisValid(log)) return 'Unexpected end of JSON input';

  const logJSON = JSON.parse(log);
  return JSON.stringify(logJSON, null, 2);
};

export default getLogDetailsPayloadPopupJSON;
