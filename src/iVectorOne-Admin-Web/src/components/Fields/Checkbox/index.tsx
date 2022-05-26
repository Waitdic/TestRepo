import { FC, memo, forwardRef, LegacyRef, FormEvent, useState } from 'react';
import { ChangeHandler } from 'react-hook-form';

type Props = {
  id: string;
  name: string;
  checked?: boolean;
  onChange: ChangeHandler;
  onBlur: ChangeHandler;
  labelText: string;
  description?: string | null;
  isDirty?: boolean;
  errorMsg?: string | null;
  required?: boolean;
};

const Checkbox: FC<Props> = forwardRef(
  (
    {
      id,
      name,
      labelText,
      description,
      isDirty = false,
      errorMsg = null,
      onChange,
      onBlur,
      required = false,
    },
    ref
  ) => {
    const [isChecked, setIsChecked] = useState<boolean>(false);

    const handleChange = (e: FormEvent<HTMLInputElement>): void => {
      onChange(e);
      setIsChecked(e.currentTarget.checked);
    };

    return (
      <div className='relative flex items-start'>
        <div className='flex items-center h-5'>
          <input
            id={id}
            name={name}
            checked={isChecked}
            type='checkbox'
            onChange={handleChange}
            onBlur={onBlur}
            className='focus:ring-blue-700 h-4 w-4 text-primary border-gray-300 rounded'
            ref={ref as LegacyRef<HTMLInputElement>}
          />
        </div>
        <div className='ml-3 text-sm'>
          <label htmlFor={id} className='font-medium text-gray-700'>
            {labelText} {required && '(required)'}
          </label>
          {description && <p className='text-gray-500'>{description}</p>}
        </div>

        {isDirty && errorMsg && (
          <span
            className='absolute top-full text-sm text-red-600'
            data-testid='checkbox-error'
          >
            {errorMsg}
          </span>
        )}
      </div>
    );
  }
);

export default memo(Checkbox);
