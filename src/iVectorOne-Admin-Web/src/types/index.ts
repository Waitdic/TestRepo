import { NotificationStatus } from '@/constants';
import { NetworkInterfaceInfoIPv4 } from 'os';
import { ComponentProps } from 'react';

//? TEMP TYPES
export type ProductProps = {
  id: number;
  slug: string;
  name: string;
  description: string;
  status: { id: number; name: string };
  category: { id: number; name: string };
  img: string;
};
export type ProductFormData = {
  name: string;
  description: string;
  status: { id: number; name: string };
  category: { id: number; name: string };
  img: string;
};
//? END TEMP TYPES
export type SelectOption = { id: number | string; name: string };
export type Tenant = {
  tenantId: number | string;
  name: string;
  isActive: boolean;
};
export type User = {
  fullName: string;
  tenants: Tenant[];
} | null;
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
export type Subscription = {
  key: string;
  name: string;
  userName: string;
  password: string;
  PropertyTPRequestLimit: number;
  SearchTimeoutSeconds: number;
  LogMainSearchError: boolean;
  CurrencyCode: string;
  isActive: boolean;
};
export type Provider = {
  name: string;
  providers: { name: string; isActive: boolean }[];
};
export type ProviderConfiguration = {
  key: string;
  name: string;
  order: number;
  type: string;
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
export type ProviderFormFields = {
  subscription: string;
  provider: string;
  configurations: any[];
};
export type FormErrorMessage = {
  [key: string]: { message: string };
};
export type NotificationState = {
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
  href: string;
};

//* App State
export type AppState = {
  lang: string;
  theme: string;
  user: User;
  awsAmplify: { username: string | null; jwtToken: string | null | undefined };
  modules: Module[];
  subscriptions: Subscription[];
  providers: Provider[];
  error: null | string | Error;
  signOut: () => void;
};
