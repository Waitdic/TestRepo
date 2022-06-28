import TenantIcon from '../icons/Tenant';
import ProvidersIcon from '../icons/Providers';
import SubscriptionsIcon from '../icons/Subscriptions';
import ChevronDownIcon from '../icons/ChevronDown';
import DashboardIcon from '../icons/Dashboard';
import SettingsIcon from '../icons/Settings';
import SupportIcon from '../icons/Support';

const getStaticSVGIcon = (name: string, className: string) => {
  switch (name) {
    case 'tenant':
      return <TenantIcon className={className} />;
    case 'providers':
      return <ProvidersIcon className={className} />;
    case 'subscriptions':
      return <SubscriptionsIcon className={className} />;
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
