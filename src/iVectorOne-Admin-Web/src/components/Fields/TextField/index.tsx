import { memo, Fragment, forwardRef, LegacyRef, FC } from 'react';
import classnames from 'classnames';
import { ExclamationCircleIcon } from '@heroicons/react/solid';
import { ChangeHandler } from 'react-hook-form';
//
import { InputTypes } from '@/constants';

type Props = {
  id: string;
  name: string;
  labelText?: string;
  type?: InputTypes;
  value?: string;
  onChange: ChangeHandler;
  onBlur: ChangeHandler;
  isDirty?: boolean;
  errorMsg?: string | null;
  hasLabel?: boolean;
  prefix?: string | null;
  prefixPos?: 'left' | 'right';
  placeholder?: string;
  className?: string;
  defaultValue?: string;
  description?: string;
  required?: boolean;
};

const TextField: FC<Props> = forwardRef(
  (
    {
      id,
      name,
      labelText,
      type = InputTypes.TEXT,
      value,
      onChange,
      onBlur,
      isDirty = false,
      errorMsg = null,
      hasLabel = true,
      prefix = null,
      prefixPos = 'left',
      placeholder = undefined,
      className = '',
      description = null,
      required = true,
    },
    ref
  ) => {
    return (
      <Fragment>
        {hasLabel && (
          <>
            <label htmlFor={id} className='block text-sm font-medium mb-1'>
              {`${labelText} ${required ? '(required)' : ''}`}
            </label>
            {description && (
              <p className='block text-sm text-gray-500'>{description}</p>
            )}
          </>
        )}
        <div className={classnames('flex', { 'mt-1': hasLabel })}>
          {prefix && (
            <span
              className={classnames(
                'inline-flex items-center px-3 border border-gray-200 bg-gray-50 text-gray-500 sm:text-sm',
                {
                  'order-1': prefixPos === 'right',
                  'rounded-l-md': prefixPos === 'left',
                  'rounded-r-md': prefixPos === 'right',
                }
              )}
            >
              {prefix}
            </span>
          )}
          <div className='relative flex-1'>
            <input
              id={id}
              name={name}
              type={type}
              value={value}
              onChange={onChange}
              onBlur={onBlur}
              className={classnames('form-input w-full', {
                'border-red-500 bg-red-100': isDirty,
                'border-r-0 rounded-r-none': prefix && prefixPos === 'right',
                'border-l-0 rounded-l-none': prefix && prefixPos === 'left',
                [className]: className !== '',
              })}
              ref={ref as LegacyRef<HTMLInputElement>}
              placeholder={placeholder}
              autoComplete='turnedOff'
            />
            {isDirty && errorMsg && (
              <Fragment>
                <div className='absolute inset-y-0 right-0 pr-3 flex items-center pointer-events-none'>
                  <ExclamationCircleIcon
                    className='h-5 w-5 text-red-500'
                    aria-hidden='true'
                  />
                </div>
                <span
                  className='absolute left-0 top-full text-xs text-red-600'
                  data-testid='input-error'
                >
                  {errorMsg}
                </span>
              </Fragment>
            )}
          </div>
        </div>
      </Fragment>
    );
  }
);

export default memo(TextField);
