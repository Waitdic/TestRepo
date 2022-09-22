import React, { forwardRef } from 'react';

type Props = {
  name: string;
  label?: string;
  type?: 'text' | 'number' | 'email' | 'password';
  onChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
  required?: boolean;
  ref?: React.Ref<HTMLInputElement>;
  value?: string | number;
};

const UncontrolledTextField: React.FC<Props> = forwardRef(
  (
    { label = '', name, type = 'text', onChange, required = false, value = '' },
    ref
  ) => {
    return (
      <>
        {!!label && (
          <label className='block text-sm font-medium mb-1' htmlFor={name}>
            {label}{' '}
            {required ? <span className='text-md text-red-500'>*</span> : ''}
          </label>
        )}
        <input
          ref={ref}
          type={type}
          id={name}
          name={name}
          value={value}
          className='form-input w-full'
          onChange={onChange}
          onBlur={onChange}
        />
      </>
    );
  }
);

export default React.memo(UncontrolledTextField);
