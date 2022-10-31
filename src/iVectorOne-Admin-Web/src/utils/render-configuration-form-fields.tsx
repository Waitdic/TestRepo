import { FieldErrorsImpl, UseFormRegister } from 'react-hook-form';
//
import {
  SupplierConfiguration,
  SupplierFormFields,
  SelectOption,
} from '@/types';
import {
  ConfigurationFormFieldTypes,
  InputTypes,
  URI_REGEX,
} from '@/constants';
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
  errors: Partial<
    FieldErrorsImpl<{
      account: number;
      supplier: number;
      configurations: any[];
    }>
  >
) => {
  if (!configurations?.length) {
    return <ErrorBoundary title='Not found any configuration' />;
  }

  const renderConfigurationFormField = (
    type: ConfigurationFormFieldTypes,
    fieldConfig: {
      accountSupplierAttributeID: number;
      supplierAttributeID?: number;
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
      accountSupplierAttributeID,
      supplierAttributeID,
      idx: _idx,
      key: _key,
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
      patternErrorMessage: _patternErrorMessage,
      defaultValue,
    } = fieldConfig;

    const supplierID = supplierAttributeID ?? accountSupplierAttributeID;

    switch (type) {
      case ConfigurationFormFieldTypes.BOOLEAN:
        return (
          <Toggle
            id={`configurations.${supplierID}`}
            {...register(`configurations.${supplierID}`, {
              required: {
                value: !!required,
                message: 'This field is required.',
              },
            })}
            labelText={labelText}
            description={description}
            required={required}
            defaultValue={value === 'true' ? true : false}
            isDirty={!!errorMsg}
            errorMsg={errorMsg}
          />
        );
      case ConfigurationFormFieldTypes.DROPDOWN:
        return (
          <Select
            id={`configurations.${supplierID}`}
            {...register(`configurations.${supplierID}`)}
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
            id={`configurations.${supplierID}`}
            type={InputTypes.NUMBER}
            {...register(`configurations.${supplierID}`, {
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
                id={`configurations.${supplierID}`}
                {...register(`configurations.${supplierID}`, {
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
                defaultValue={value as string}
                labelText={labelText}
                description={description}
                isDirty={!!errorMsg}
                errorMsg={errorMsg}
                required={required}
              />
            ) : (
              <TextField
                id={`configurations.${supplierID}`}
                {...register(`configurations.${supplierID}`, {
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
            id={`configurations.${supplierID}`}
            {...register(`configurations.${supplierID}`, {
              required: {
                value: !!required,
                message: 'This field is required.',
              },
              pattern: {
                value: pattern || URI_REGEX,
                message: 'This is not a valid url pattern.',
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
            id={`configurations.${supplierID}`}
            type={InputTypes.EMAIL}
            {...register(`configurations.${supplierID}`, {
              required: {
                value: !!required,
                message: 'This field is required.',
              },
              pattern: {
                value:
                  pattern ||
                  new RegExp(/^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/),
                message: errorMsg || 'This is not a valid email address.',
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
      case ConfigurationFormFieldTypes.PASSWORD:
        return (
          <TextField
            id={`configurations.${supplierID}`}
            type={InputTypes.PASSWORD}
            {...register(`configurations.${supplierID}`, {
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
        accountSupplierAttributeID,
        supplierAttributeID,
      },
      idx
    ) => {
      const error = {
        id: -1,
        type: 'required',
        message: '',
      };
      if (errors?.configurations) {
        const computedErrors = Object.entries(errors.configurations);
        const itemWithError: any = computedErrors?.find(
          (err) =>
            Number(err[0]) ===
            (supplierAttributeID || accountSupplierAttributeID)
        );
        if (itemWithError) {
          error.id = accountSupplierAttributeID;
          error.type = itemWithError?.[1]?.type;
          error.message = itemWithError?.[1]?.message;
        }
      }

      return (
        <div key={idx}>
          {renderConfigurationFormField(type, {
            accountSupplierAttributeID,
            supplierAttributeID,
            idx,
            key,
            labelText: name as string,
            description,
            required,
            value,
            errorMsg: error.message,
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
