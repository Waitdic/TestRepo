import { FC, forwardRef, memo, useState, FormEvent, LegacyRef } from 'react';
import { ChangeHandler } from 'react-hook-form';

type Props = {
  id: string;
  name: string;
  labelText: string;
  onChange: ChangeHandler;
  onBlur: ChangeHandler;
  isDirty?: boolean;
  errorMsg?: string | null;
  defaultChecked?: boolean;
  description?: string;
};

const Radio: FC<Props> = forwardRef(
  (
    {
      id,
      name,
      labelText,
      defaultChecked = false,
      isDirty = false,
      errorMsg = null,
      onChange,
      onBlur,
      description = null,
    },
    ref
  ) => {
    const [isChecked, setIsChecked] = useState<{
      id: string;
      checked: boolean;
    }>({
      id,
      checked: defaultChecked,
    });

    const handleChange = (e: FormEvent<HTMLInputElement>): void => {
      setIsChecked((prevState) => ({
        ...prevState,
        checked: !prevState.checked,
      }));
      onChange(e);
    };

    return (
      <div className='relative'>
        <div className='flex items-center'>
          <input
            id={id}
            name={name}
            type='radio'
            value={id}
            checked={isChecked.checked}
            onChange={handleChange}
            onBlur={onBlur}
            className='focus:ring-blue-700 h-4 w-4 text-primary border-gray-300'
            ref={ref as LegacyRef<HTMLInputElement>}
          />
          <div>
            <label
              htmlFor={id}
              className='ml-3 block text-sm font-medium text-dark'
            >
              {labelText}
            </label>
            {description && (
              <p className='block text-sm text-gray-500'>{description}</p>
            )}
          </div>
        </div>

        {isDirty && errorMsg && (
          <span
            className='absolute top-full text-sm text-red-600'
            data-testid='radio-error'
          >
            {errorMsg}
          </span>
        )}
      </div>
    );
  }
);

export default memo(Radio);
