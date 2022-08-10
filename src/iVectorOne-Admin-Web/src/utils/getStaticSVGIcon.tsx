import TenantIcon from '../icons/Tenant';
import SupplierIcon from '../icons/Suppliers';
import AccountsIcon from '../icons/Accounts';
import ChevronDownIcon from '../icons/ChevronDown';
import DashboardIcon from '../icons/Dashboard';
import SettingsIcon from '../icons/Settings';
import SupportIcon from '../icons/Support';

const getStaticSVGIcon = (name: string, className: string) => {
  switch (name) {
    case 'tenants':
      return <TenantIcon className={className} />;
    case 'suppliers':
      return <SupplierIcon className={className} />;
    case 'accounts':
      return <AccountsIcon className={className} />;
    case 'chevronDown':
      return <ChevronDownIcon className={className} />;
    case 'dashboard':
      return <DashboardIcon className={className} />;
    case 'settings':
      return <SettingsIcon className={className} />;
    case 'support':
      return <SupportIcon className={className} />;
    default:
      console.error(`Undefined static SVG icon name: ${name}`);
      return null;
  }
};

export default getStaticSVGIcon;
