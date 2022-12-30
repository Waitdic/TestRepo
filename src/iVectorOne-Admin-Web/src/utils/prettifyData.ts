import xmlFormat from 'xml-formatter';

export function prettifyData(content: string) 
{
    content = content.trim();
    
    try
    {
        let logJSON = JSON.parse(content);
        return JSON.stringify(logJSON, null, 2);
    } catch {}

    try
    {
      var prettyXml = xmlFormat(content);
      return prettyXml;
    } catch {}
  
    return content;
}
