import { prettifyData } from '@/utils/prettifyData';

const getLogDetailsPayloadPopupModel = (logDetails : any) => {

    const { requestLog, responseLog } = logDetails;
    
    const url = requestLog.split('\r\n\r\n').filter((x : string) => x.includes('URL:'))[0].substring(5);

    const [urlPath, urlQuery] = url.split('?');    
    const urlParams =  JSON.stringify(Object.fromEntries(new URLSearchParams(urlQuery)), null, 2);

    let requestBody = requestLog.split('**REQUEST**')[1].trim(); 
    requestBody = requestBody.length > 0
        ? prettifyData(requestBody)
        : "[Body is empty]";

    
    let responseBody = responseLog.split('**RESPONSE**')[1].trim();
    responseBody = responseBody.length > 0
        ? prettifyData(responseBody)
        : "[Body is empty]";


    let model = 
    {
        urlPath,
        urlParams,
        requestBody,
        responseBody
    };

    return model;
};

export default getLogDetailsPayloadPopupModel;