import { memo, forwardRef, LegacyRef, FC } from 'react';
import { ChangeHandler } from 'react-hook-form';
import { BiChevronDown } from 'react-icons/bi';
import classnames from 'classnames';
//
import { SelectOption } from '@/types';

type Props = {
  id: string;
  name: string;
  onChange: ChangeHandler;
  onBlur: ChangeHandler;
  labelText: string;
  options: SelectOption[];
  defaultValue?: SelectOption;
  hasLabel?: boolean;
  onChangeCallback?: () => void;
  disabled?: boolean;
  description?: string;
  maxItems?: number;
  minItems?: number;
  onUncontrolledChange?: (optionId: number) => void;
  isFirstOptionEmpty?: boolean;
};

const Select: FC<Props> = forwardRef(
  (
    {
      id,
      labelText,
      options,
      onChange,
      onBlur,
      name,
      defaultValue = options[0],
      hasLabel = true,
      onChangeCallback = null,
      disabled = false,
      description = null,
      maxItems = 100,
      minItems: _minItems = 1,
      onUncontrolledChange = null,
      isFirstOptionEmpty = false,
    },
    ref
  ) => {
    const slicedOptions = options.slice(0, maxItems);

    const handleChange = (event: { target: any; type: any }) => {
      onChange(event);
      onUncontrolledChange?.(Number(event.target.value));

      if (onChangeCallback) {
        onChangeCallback();
      }
    };

    return (
      <div>
        {hasLabel && (
          <label
            htmlFor={id}
            className='block text-sm font-medium text-gray-700'
          >
            {labelText}
          </label>
        )}
        {description && (
          <p className='block text-sm text-gray-500'>{description}</p>
        )}
        <div className='relative'>
          <select
            id={id}
            name={name}
            ref={ref as LegacyRef<HTMLSelectElement>}
            onChange={handleChange}
            onBlur={onBlur}
            className={classnames(
              'mt-1 block w-full py-2 px-3 pr-10 text-sm border border-gray-200 focus:outline-none focus:ring-blue-700 focus:border-blue-700 rounded-md appearance-none',
              {
                'cursor-not-allowed': disabled,
              }
            )}
            defaultValue={defaultValue?.id}
            disabled={disabled}
            data-testid='select-field'
          >
            {isFirstOptionEmpty && <option value=''></option>}
            {slicedOptions?.map(({ id: optionId, name: optionName }) => (
              <option value={optionId} key={optionId}>
                {optionName}
              </option>
            ))}
          </select>
          <span className='absolute top-2 right-2 pointer-events-none'>
            <BiChevronDown className='w-6 h-6' />
          </span>
        </div>
      </div>
    );
  }
);

export default memo(Select);
