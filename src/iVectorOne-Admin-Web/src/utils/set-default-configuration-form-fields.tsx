import { UseFormSetValue } from 'react-hook-form';
//
import { SupplierConfiguration, SupplierFormFields } from '@/types';

export const setDefaultConfigurationFormFields = (
  configurations: SupplierConfiguration[],
  setValue: UseFormSetValue<SupplierFormFields>
) => {
  for (let idx = 0; idx < configurations.length; idx++) {
    const currentConfigField = configurations[idx];

    if (currentConfigField.defaultValue) {
      setValue(
        `configurations.${idx}.${currentConfigField.key}`,
        currentConfigField.defaultValue
      );
    }
  }
};
