import { UseFormRegister } from 'react-hook-form';
import { sortBy } from 'lodash';
//
import {
  FormErrorMessage,
  ProviderConfiguration,
  ProviderFormFields,
  SelectOption,
} from '@/types';
import { InputTypes } from '@/constants';
import {
  Toggle,
  TextArea,
  Select,
  TextField,
  ErrorBoundary,
} from '@/components';

export const renderConfigurationFormFields = (
  configurations: ProviderConfiguration[],
  register: UseFormRegister<ProviderFormFields>,
  errors: {
    configurations?: FormErrorMessage[];
  }
) => {
  if (!configurations?.length) {
    return <ErrorBoundary title='Not found any configuration' />;
  }
  const configurationsSortedByOrder = sortBy(configurations, ['order']);

  return configurationsSortedByOrder.map(
    (
      {
        key,
        name,
        type,
        description,
        editPresentation,
        dropdownOptions,
        defaultValue,
        minimum,
        maximum,
        maxLength,
        minLength,
        minItems,
        maxItems,
        pattern,
        patternErrorMessage,
        format,
        required,
      },
      idx
    ) => {
      const dirtyField = errors.configurations && errors?.configurations[idx];
      const errorMsg =
        dirtyField && key in dirtyField && dirtyField[key].message;

      return (
        <div key={idx}>
          {type === 'boolean' ? (
            <Toggle
              id={`configurations.${idx}.${key}`}
              {...register(`configurations.${idx}.${key}`, {
                required: {
                  value: required as boolean,
                  message: 'This field is required.',
                },
              })}
              labelText={name}
              description={description}
              required={required}
              defaultValue={defaultValue as boolean}
              isDirty={errorMsg as boolean}
              errorMsg={errorMsg as string}
            />
          ) : type === 'dropdown' && dropdownOptions ? (
            <Select
              id={`configurations.${idx}.${key}`}
              {...register(`configurations.${idx}.${key}`)}
              labelText={name}
              description={description}
              maxItems={maxItems}
              minItems={minItems}
              options={dropdownOptions}
              defaultValue={defaultValue as unknown as SelectOption}
            />
          ) : type === 'string' && editPresentation === 'multilineText' ? (
            <TextArea
              id={`configurations.${idx}.${key}`}
              {...register(`configurations.${idx}.${key}`, {
                required: {
                  value: required as boolean,
                  message: 'This field is required.',
                },
                minLength: {
                  value: minLength as number,
                  message: `Minimum length ${minLength} characters.`,
                },
                maxLength: {
                  value: maxLength as number,
                  message: `Maximum length ${maxLength} characters.`,
                },
              })}
              labelText={name}
              description={description}
              isDirty={errorMsg as boolean}
              errorMsg={errorMsg as string}
              required={required}
            />
          ) : type === 'number' ? (
            <TextField
              id={`configurations.${idx}.${key}`}
              type={InputTypes.NUMBER}
              {...register(`configurations.${idx}.${key}`, {
                required: {
                  value: required as boolean,
                  message: 'This field is required.',
                },
                min: {
                  value: minimum as number,
                  message: `Minimum value ${minimum}.`,
                },
                max: {
                  value: maximum as number,
                  message: `Maximum value ${maximum}.`,
                },
              })}
              labelText={name}
              description={description}
              isDirty={errorMsg as boolean}
              errorMsg={errorMsg as string}
              required={required}
            />
          ) : type === 'uri' ? (
            <TextField
              id={`configurations.${idx}.${key}`}
              {...register(`configurations.${idx}.${key}`, {
                required: {
                  value: required as boolean,
                  message: 'This field is required.',
                },
                pattern: {
                  value:
                    (pattern as unknown as RegExp) ||
                    new RegExp(
                      /(https?:\/\/(?:www\.|(?!www))[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|www\.[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|https?:\/\/(?:www\.|(?!www))[a-zA-Z0-9]+\.[^\s]{2,}|www\.[a-zA-Z0-9]+\.[^\s]{2,})/gi
                    ),
                  message:
                    (patternErrorMessage as string) ||
                    'This is not a valid url pattern.',
                },
              })}
              labelText={name}
              description={description}
              isDirty={errorMsg as boolean}
              errorMsg={errorMsg as string}
              required={required}
            />
          ) : type === 'email' ? (
            <TextField
              id={`configurations.${idx}.${key}`}
              type={InputTypes.EMAIL}
              {...register(`configurations.${idx}.${key}`, {
                required: {
                  value: required as boolean,
                  message: 'This field is required.',
                },
                pattern: {
                  value:
                    (pattern as unknown as RegExp) ||
                    new RegExp(/^([\w.%+-]+)@([\w-]+\.)+([\w]{2,})$/i),
                  message:
                    (patternErrorMessage as string) ||
                    'This is not a valid email address.',
                },
              })}
              labelText={name}
              description={description}
              isDirty={errorMsg as boolean}
              errorMsg={errorMsg as string}
              required={required}
            />
          ) : type === 'password' ? (
            <TextField
              id={`configurations.${idx}.${key}`}
              type={InputTypes.PASSWORD}
              {...register(`configurations.${idx}.${key}`, {
                required: {
                  value: required as boolean,
                  message: 'This field is required.',
                },
                minLength: {
                  value: minLength as number,
                  message: `Minimum length ${minLength} characters.`,
                },
                maxLength: {
                  value: maxLength as number,
                  message: `Maximum length ${maxLength} characters.`,
                },
              })}
              labelText={name}
              description={description}
              isDirty={errorMsg as boolean}
              errorMsg={errorMsg as string}
              required={required}
            />
          ) : (
            <TextField
              id={`configurations.${idx}.${key}`}
              {...register(`configurations.${idx}.${key}`, {
                required: {
                  value: required as boolean,
                  message: 'This field is required.',
                },
                minLength: {
                  value: minLength as number,
                  message: `Minimum length ${minLength} characters.`,
                },
                maxLength: {
                  value: maxLength as number,
                  message: `Maximum length ${maxLength} characters.`,
                },
              })}
              labelText={name}
              description={description}
              isDirty={errorMsg as boolean}
              errorMsg={errorMsg as string}
              required={required}
            />
          )}
        </div>
      );
    }
  );
};
