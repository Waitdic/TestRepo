import TenantIcon from '../icons/Tenant';
import ProviderIcon from '../icons/Provider';
import SubscriptionIcon from '../icons/Subscription';

const getStaticConsoleIcon = (name: string, isActive: boolean) => {
  switch (name) {
    case 'tenant':
      return <TenantIcon isActive={isActive} />;
    case 'provider':
      return <ProviderIcon isActive={isActive} />;
    case 'subscription':
      return <SubscriptionIcon isActive={isActive} />;
    default:
      console.error(`Unknown console item name: ${name}`);
      return null;
  }
};

export default getStaticConsoleIcon;
