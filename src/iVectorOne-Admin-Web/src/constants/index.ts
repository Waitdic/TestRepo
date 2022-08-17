export enum ButtonVariants {
  BUTTON = 'button',
  SUBMIT = 'submit',
}
export enum ButtonColors {
  PRIMARY = 'primary',
  DANGER = 'danger',
  WARNING = 'warning',
  INFO = 'info',
  CREATE = 'create',
  OUTLINE = 'outline',
}
export enum MenuPosition {
  RIGHT = 'right',
  LEFT = 'left',
}
export enum InputTypes {
  TEXT = 'text',
  EMAIL = 'email',
  PASSWORD = 'password',
  NUMBER = 'number',
  PHONE = 'tel',
}
export enum NotificationStatus {
  SUCCESS = 'success',
  ERROR = 'error',
}
export enum ConfigurationFormFieldTypes {
  BOOLEAN = 'Boolean',
  DROPDOWN = 'Dropdown',
  STRING = 'String',
  NUMBER = 'Number',
  URI = 'Uri',
  EMAIL = 'Email',
  PASSWORD = 'Password',
}
export enum ChangeLogFilterTypes {
  ALL = 'View All',
  ANNOUNCEMENT = 'Announcement',
  BUG_FIX = 'Bug Fix',
  PRODUCT = 'Product',
}
export const URI_REGEX = new RegExp(
  /(https?:\/\/(?:www\.|(?!www))[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|www\.[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|https?:\/\/(?:www\.|(?!www))[a-zA-Z0-9]+\.[^\s]{2,}|www\.[a-zA-Z0-9]+\.[^\s]{2,})/gi
);
