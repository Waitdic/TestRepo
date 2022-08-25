import { ConfigurationFormFieldTypes, NotificationStatus } from '@/constants';
import { NetworkInterfaceInfoIPv4 } from 'os';
import { ComponentProps } from 'react';

export type SelectOption = { id: number | string; name: string };
export type Tenant = {
  contactEmail: string;
  companyName: string;
  contactName: string;
  contactTelephone: string;
  isActive: boolean;
  name: string;
  tenantId: number;
  tenantKey: string;
  isSelected?: boolean;
  status: 'active' | 'inactive';
  isDeleted: boolean;
};
export type User = {
  userId: number;
  fullName: string;
  tenants: Tenant[];
  authorisations: {
    user: string;
    relationship: string;
    object: string;
  }[];
  success: boolean;
} | null;
export type UserResponse = {
  userId: number;
  userName: string;
  key: string;
};
export type Role = {
  name: string;
};
export type Console = {
  name: string;
  roles: Role[];
  uri: string;
};
export type Module = {
  isActive: boolean;
  moduleId: string;
  name: string;
  uri: string;
  consoles: Console[];
};
export type Account = {
  accountId: number;
  userName: string;
  password: string;
  dummyResponses: boolean;
  propertyTprequestLimit: number;
  searchTimeoutSeconds: number;
  logMainSearchError: boolean;
  currencyCode: string;
  environment: string;
  suppliers: Supplier[];
  isActive: boolean;
  isDeleted: boolean;
  status: 'active' | 'inactive';
};
export type Supplier = {
  name?: string;
  supplierName?: string;
  supplierID?: number;
  accountSupplierID?: number;
  configurations?: SupplierConfiguration[];
  isSelected?: boolean;
  enabled?: boolean;
};
export type SupplierConfiguration = {
  supplierAttributeID?: number;
  accountSupplierAttributeID: number;
  key: string;
  name?: string;
  order: number;
  type: ConfigurationFormFieldTypes;
  value?: number | string | boolean;
  defaultValue?: number | string | boolean;
  description?: string;
  minimum?: number;
  maximum?: number;
  maxLength?: number;
  minLength?: number;
  minItems?: number;
  maxItems?: number;
  editPresentation?: 'multilineText' | 'singlelineText';
  dropdownOptions?: SelectOption[];
  pattern?: string;
  patternErrorMessage?: string;
  format?: Date | NetworkInterfaceInfoIPv4;
  required?: boolean;
};
export type SupplierFormFields = {
  account: number;
  supplier: number;
  configurations: any[];
};
export type FormErrorMessage = {
  [key: string]: { message: string };
};
export type NotificationState = {
  title?: string;
  status: NotificationStatus;
  message: string;
};
export type NavigationProps = {
  name: string;
  href: string;
  pathname: string;
  icon?: (props: ComponentProps<'svg'>) => JSX.Element;
};
export type DropdownNavigationProps = {
  name: string;
  href?: string;
  action?: () => void;
};
export type DropdownFilterProps = {
  name: string;
  value: boolean;
};

//* App State
export type AppState = {
  lang: string;
  theme: string;
  user: User;
  awsAmplify: { username: string | null; jwtToken: string | null | undefined };
  modules: Module[];
  accounts: Account[];
  isLoading: boolean;
  error: null | string | Error;
  incompleteSetup: boolean;
};
