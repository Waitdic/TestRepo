import { UseFormSetValue } from 'react-hook-form';
//
import { ProviderConfiguration, ProviderFormFields } from '@/types';

export const setDefaultConfigurationFormFields = (
  configurations: ProviderConfiguration[],
  setValue: UseFormSetValue<ProviderFormFields>
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
