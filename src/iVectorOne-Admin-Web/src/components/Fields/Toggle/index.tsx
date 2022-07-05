import { Switch } from '@headlessui/react';
import {
  FC,
  memo,
  forwardRef,
  LegacyRef,
  FormEvent,
  useState,
  useEffect,
} from 'react';
import { ChangeHandler } from 'react-hook-form';
import classnames from 'classnames';

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
  defaultValue?: boolean;
};

const Toggle: FC<Props> = forwardRef(
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
      defaultValue = false,
    },
    ref
  ) => {
    const [isChecked, setIsChecked] = useState<boolean>(defaultValue);

    const handleChange = (e: FormEvent<HTMLInputElement>): void => {
      onChange(e);
      setIsChecked(e.currentTarget.checked);
    };

    useEffect(() => {
      setIsChecked(defaultValue);
    }, [defaultValue]);

    return (
      <div className='relative flex justify-between items-center'>
        <input
          id={id}
          name={name}
          checked={isChecked}
          type='checkbox'
          onChange={handleChange}
          onBlur={onBlur}
          className='focus:ring-blue-700 h-4 w-4 text-primary border-gray-300 rounded hidden'
          ref={ref as LegacyRef<HTMLInputElement>}
          hidden
        />
        <label
          htmlFor={id}
          className='flex items-center justify-between w-full'
        >
          <div className='text-sm font-medium text-gray-700'>
            {labelText} {required && '(required)'}
            {description && <p className='text-gray-500'>{description}</p>}
          </div>
          <Switch
            checked={isChecked}
            onChange={() => void 0}
            className={classnames(
              isChecked ? 'bg-primary' : 'bg-gray-200',
              'pointer-events-none relative inline-flex flex-shrink-0 h-6 w-11 border-2 border-transparent rounded-full cursor-pointer transition-colors ease-in-out duration-200 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500'
            )}
          >
            <span className='sr-only'>Use setting</span>
            <span
              aria-hidden='true'
              className={classnames(
                isChecked ? 'translate-x-5' : 'translate-x-0',
                'pointer-events-none inline-block h-5 w-5 rounded-full bg-white shadow transform ring-0 transition ease-in-out duration-200'
              )}
            />
          </Switch>
        </label>

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

export default memo(Toggle);
