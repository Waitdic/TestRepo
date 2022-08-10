import {
  FC,
  memo,
  forwardRef,
  LegacyRef,
  Fragment,
  useState,
  useEffect,
  FormEvent,
  useCallback,
} from 'react';
import { ChangeHandler } from 'react-hook-form';
import { ExclamationCircleIcon } from '@heroicons/react/solid';
import classnames from 'classnames';

type Props = {
  id: string;
  name: string;
  labelText?: string | null;
  onChange: ChangeHandler;
  onBlur: ChangeHandler;
  isDirty?: boolean;
  errorMsg?: string | null;
  hasLabel?: boolean;
  rows?: number;
  defaultValue?: string;
  description?: string;
  required?: boolean;
};

const TextArea: FC<Props> = forwardRef(
  (
    {
      id,
      name,
      labelText = null,
      defaultValue = '',
      onChange,
      onBlur,
      isDirty = false,
      errorMsg = null,
      hasLabel = true,
      description = null,
      rows = 4,
      required = true,
    },
    ref
  ) => {
    const [value, setValue] = useState(defaultValue);

    const handleChange = useCallback(
      (e: FormEvent<HTMLTextAreaElement>): void => {
        setValue(e.currentTarget.value);
        onChange(e);
      },
      [onChange]
    );

    useEffect(() => {
      if (defaultValue !== '') {
        setValue(defaultValue);
      }
    }, [defaultValue]);

    return (
      <>
        {hasLabel && (
          <label htmlFor={id} className='block text-sm font-medium text-dark'>
            {labelText}{' '}
            {required ? <span className='text-md text-red-500'>*</span> : ''}
          </label>
        )}
        {description && (
          <p className='block text-sm text-gray-500'>{description}</p>
        )}
        <div className='relative mt-1 mb-4'>
          <textarea
            id={id}
            name={name}
            rows={rows}
            className={classnames(
              'py-2 px-3 block text-sm w-full border border-gray-200 focus:ring-blue-700 focus:border-blue-700 rounded-md resize-none',
              isDirty ? 'border-red-500 bg-red-100' : 'border-transparent'
            )}
            defaultValue={value}
            onChange={handleChange}
            onBlur={onBlur}
            ref={ref as LegacyRef<HTMLTextAreaElement>}
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
                className='absolute top-full text-xs text-red-600'
                data-testid='textarea-error'
              >
                {errorMsg}
              </span>
            </Fragment>
          )}
        </div>
      </>
    );
  }
);

export default memo(TextArea);
