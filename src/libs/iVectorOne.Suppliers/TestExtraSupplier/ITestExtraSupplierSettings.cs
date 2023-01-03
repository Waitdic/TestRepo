using System;
using System.Collections.Generic;
using System.Text;

namespace iVectorOne.Suppliers.TestExtraSupplier
{
    public interface ITestExtraSupplierSettings
    {
        int SearchTimeMilliseconds(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}
