import { UseFormRegister } from 'react-hook-form';
import { sortBy } from 'lodash';
//
import {
  FormErrorMessage,
  SupplierConfiguration,
  SupplierFormFields,
  SelectOption,
} from '@/types';
import { ConfigurationFormFieldTypes, InputTypes } from '@/constants';
import {
  Toggle,
  TextArea,
  Select,
  TextField,
  ErrorBoundary,
} from '@/components';

export const renderConfigurationFormFields = (
  configurations: SupplierConfiguration[],
  register: UseFormRegister<SupplierFormFields>,
  errors: {
    configurations?: FormErrorMessage[];
  }
) => {
  if (!configurations?.length) {
    return <ErrorBoundary title='Not found any configuration' />;
  }

  const renderConfigurationFormField = (
    type: ConfigurationFormFieldTypes,
    fieldConfig: {
      supplierSubscriptionAttributeID: number;
      idx: number;
      key: string;
      labelText: string;
      description?: string;
      required?: boolean;
      value?: boolean | string | number;
      errorMsg?: string;
      minLength?: number;
      maxLength?: number;
      minNumberValue?: number;
      maxNumberValue?: number;
      dropdownOptions?: SelectOption[];
      minItems?: number;
      maxItems?: number;
      editPresentation?: 'multilineText' | 'singlelineText';
      pattern?: RegExp;
      patternErrorMessage?: string;
      defaultValue?: string | boolean | number;
    }
  ) => {
    const {
      supplierSubscriptionAttributeID,
      idx,
      key,
      labelText,
      description,
      required,
      value,
      errorMsg,
      minLength = 1,
      maxLength = 999,
      minNumberValue = Number.MIN_VALUE,
      maxNumberValue = Number.MAX_VALUE,
      dropdownOptions = [],
      minItems = 1,
      maxItems = 999,
      editPresentation,
      pattern,
      patternErrorMessage,
      defaultValue,
    } = fieldConfig;

    switch (type) {
      case ConfigurationFormFieldTypes.BOOLEAN:
        return (
          <Toggle
            id={`configurations.${supplierSubscriptionAttributeID}`}
            {...register(`configurations.${supplierSubscriptionAttributeID}`, {
              required: {
                value: !!required,
                message: 'This field is required.',
              },
            })}
            labelText={labelText}
            description={description}
            required={required}
            defaultValue={Boolean(value)}
            isDirty={!!errorMsg}
            errorMsg={errorMsg}
          />
        );
      case ConfigurationFormFieldTypes.DROPDOWN:
        return (
          <Select
            id={`configurations.${supplierSubscriptionAttributeID}`}
            {...register(`configurations.${supplierSubscriptionAttributeID}`)}
            labelText={labelText}
            description={description}
            maxItems={maxItems}
            minItems={minItems}
            options={dropdownOptions}
            defaultValue={value as unknown as SelectOption}
          />
        );
      case ConfigurationFormFieldTypes.NUMBER:
        return (
          <TextField
            id={`configurations.${supplierSubscriptionAttributeID}`}
            type={InputTypes.NUMBER}
            {...register(`configurations.${supplierSubscriptionAttributeID}`, {
              required: {
                value: !!required,
                message: 'This field is required.',
              },
              min: {
                value: minNumberValue,
                message: `Minimum value ${minNumberValue}.`,
              },
              max: {
                value: maxNumberValue,
                message: `Maximum value ${maxNumberValue}.`,
              },
            })}
            defaultValue={value as string}
            labelText={labelText}
            description={description}
            isDirty={!!errorMsg}
            errorMsg={errorMsg}
            required={required}
          />
        );
      case ConfigurationFormFieldTypes.STRING:
        return (
          <>
            {editPresentation === 'multilineText' ? (
              <TextArea
                id={`configurations.${supplierSubscriptionAttributeID}`}
                {...register(
                  `configurations.${supplierSubscriptionAttributeID}`,
                  {
                    required: {
                      value: !!required,
                      message: 'This field is required.',
                    },
                    minLength: {
                      value: minLength,
                      message: `Minimum length ${minLength} characters.`,
                    },
                    maxLength: {
                      value: maxLength,
                      message: `Maximum length ${maxLength} characters.`,
                    },
                  }
                )}
                defaultValue={value as string}
                labelText={labelText}
                description={description}
                isDirty={!!errorMsg}
                errorMsg={errorMsg}
                required={required}
              />
            ) : (
              <TextField
                id={`configurations.${supplierSubscriptionAttributeID}`}
                {...register(
                  `configurations.${supplierSubscriptionAttributeID}`,
                  {
                    required: {
                      value: !!required,
                      message: 'This field is required.',
                    },
                    minLength: {
                      value: minLength,
                      message: `Minimum length ${minLength} characters.`,
                    },
                    maxLength: {
                      value: maxLength,
                      message: `Maximum length ${maxLength} characters.`,
                    },
                  }
                )}
                labelText={labelText}
                defaultValue={value as string}
                description={description}
                isDirty={!!errorMsg}
                errorMsg={errorMsg}
                required={required}
              />
            )}
          </>
        );
      case ConfigurationFormFieldTypes.URI:
        return (
          <TextField
            id={`configurations.${supplierSubscriptionAttributeID}`}
            {...register(`configurations.${supplierSubscriptionAttributeID}`, {
              required: {
                value: !!required,
                message: 'This field is required.',
              },
              pattern: {
                value:
                  pattern ||
                  new RegExp(
                    /(https?:\/\/(?:www\.|(?!www))[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|www\.[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|https?:\/\/(?:www\.|(?!www))[a-zA-Z0-9]+\.[^\s]{2,}|www\.[a-zA-Z0-9]+\.[^\s]{2,})/gi
                  ),
                message:
                  patternErrorMessage || 'This is not a valid url pattern.',
              },
            })}
            labelText={labelText}
            description={description}
            defaultValue={value as string}
            isDirty={!!errorMsg}
            errorMsg={errorMsg}
            required={required}
          />
        );
      case ConfigurationFormFieldTypes.EMAIL:
        return (
          <TextField
            id={`configurations.${supplierSubscriptionAttributeID}`}
            type={InputTypes.EMAIL}
            {...register(`configurations.${supplierSubscriptionAttributeID}`, {
              required: {
                value: !!required,
                message: 'This field is required.',
              },
              pattern: {
                value:
                  pattern || new RegExp(/^([\w.%+-]+)@([\w-]+\.)+([\w]{2,})$/i),
                message:
                  patternErrorMessage || 'This is not a valid email address.',
              },
            })}
            defaultValue={value as string}
            labelText={labelText}
            description={description}
            isDirty={!errorMsg}
            errorMsg={errorMsg}
            required={required}
          />
        );
      case ConfigurationFormFieldTypes.PASSWORD:
        return (
          <TextField
            id={`configurations.${supplierSubscriptionAttributeID}`}
            type={InputTypes.PASSWORD}
            {...register(`configurations.${supplierSubscriptionAttributeID}`, {
              required: {
                value: !!required,
                message: 'This field is required.',
              },
              minLength: {
                value: minLength,
                message: `Minimum length ${minLength} characters.`,
              },
              maxLength: {
                value: maxLength,
                message: `Maximum length ${maxLength} characters.`,
              },
            })}
            labelText={labelText}
            description={description}
            defaultValue={(defaultValue as string) || (value as string)}
            isDirty={!!errorMsg}
            errorMsg={errorMsg}
            required={required}
          />
        );
      default:
        return null;
    }
  };

  return configurations.map(
    (
      {
        key,
        name,
        type,
        description,
        editPresentation,
        dropdownOptions,
        defaultValue,
        value,
        minimum,
        maximum,
        maxLength,
        minLength,
        minItems,
        maxItems,
        pattern,
        patternErrorMessage,
        format: _format,
        required,
        supplierSubscriptionAttributeID,
      },
      idx
    ) => {
      const dirtyField = errors.configurations && errors?.configurations[idx];
      const errorMsg =
        dirtyField && key in dirtyField && dirtyField[key].message;

      return (
        <div key={idx}>
          {renderConfigurationFormField(type, {
            supplierSubscriptionAttributeID,
            idx,
            key,
            labelText: name,
            description,
            required,
            value,
            errorMsg: errorMsg as string,
            minLength,
            maxLength,
            minNumberValue: minimum,
            maxNumberValue: maximum,
            dropdownOptions,
            minItems,
            maxItems,
            editPresentation,
            pattern: pattern as unknown as RegExp,
            patternErrorMessage,
            defaultValue,
          })}
        </div>
      );
    }
  );
};
