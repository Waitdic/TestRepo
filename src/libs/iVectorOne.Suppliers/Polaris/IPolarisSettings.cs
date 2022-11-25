using System;
using System.Collections.Generic;
using System.Text;

namespace iVectorOne.Suppliers.Polaris
{
    public interface IPolarisSettings
    {
        string URL(IThirdPartyAttributeSearch tpAttributeSearch);
        string Login(IThirdPartyAttributeSearch tpAttributeSearch);
        string Password(IThirdPartyAttributeSearch tpAttributeSearch);
        bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch);
        bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}
