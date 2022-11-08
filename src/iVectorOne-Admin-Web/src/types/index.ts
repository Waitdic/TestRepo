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
  isSelected?: boolean;
};
export type Supplier = {
  name?: string;
  supplierName?: string;
  supplierID?: number;
  supplierAccountID?: number;
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
  status: NotificationStatus;
  message: string;
  title?: string;
  instance?: string;
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
export type ApiError = {
  response: {
    data: {
      type: string;
      title: string;
      status: number;
      detail: string;
      instance: string;
    };
  };
};
export type Property = {
  propertyId: number;
  name: string;
};
export type SearchDetails = {
  isActive: boolean;
  properties: Property[];
  property: Property;
  arrivalDate: Date;
  accountId: number;
  duration: number;
  adults: number;
  children: number;
  childrenAges: number[];
  infants: number;
};
export type SearchRequestData = {
  ArrivalDate: string;
  Duration: number;
  Properties: number[];
  RoomRequests: {
    Adults: number;
    Children: number;
    Infants: number;
    ChildAges: number[];
  }[];
};
export type ChartData = {
  labels: string[];
  datasets: {
    label: string;
    data: number[];
    fill: boolean;
    borderColor: string;
    borderWidth: number;
    tension: number;
    pointRadius: number;
    pointHoverRadius: number;
    pointBackgroundColor: string;
    clip: number;
  }[];
};
export interface BookingsByHour {
  time: number;
  currentTotal: number;
  previousTotal: number;
}

export interface SearchesByHour {
  time: number;
  currentTotal: number;
  previousTotal: number;
}

export interface MultiLevelTableData {
  [key: string]: string | { [key: string]: string };
}

export interface DashboardChartData {
  bookingsByHour: BookingsByHour[];
  searchesByHour: SearchesByHour[];
  summary: MultiLevelTableData[];
  supplier: MultiLevelTableData[];
  success: boolean;
}

export interface SupplierSearchResults {
  supplier: string;
  roomCode: string;
  roomType: string;
  mealBasis: string;
  currency: string;
  totalCost: number;
  nonRefundable: boolean;
}

export type LogViewerFilters = {
  accountId: number;
  supplier: number;
  logDateRange: Date | Date[];
  system: 'all' | 'Live Only' | 'Test Only';
  type: 'all' | 'Prebook Only' | 'Book Only';
  responseSuccess: 'all' | 'Successful Only' | 'Unsuccessful Only';
};

export interface LogEntries {
  environment?: string;
  timestamp: string;
  supplierName: string;
  type: string;
  responseTime: number;
  supplierBookingReference: string;
  leadGuestName: string;
  apiLogId?: number;
}

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
  notification: NotificationState | null;
};
