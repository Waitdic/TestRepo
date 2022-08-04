import React from 'react';

type Props = {
  label: string;
  name: string;
  type?: 'text' | 'number' | 'email' | 'password';
  onChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
  required?: boolean;
};

const UncontrolledTextField: React.FC<Props> = ({
  label,
  name,
  type = 'text',
  onChange,
  required = false,
}) => {
  return (
    <>
      <label className='block text-sm font-medium mb-1' htmlFor={name}>
        {label} {required ? '(*)' : ''}
      </label>
      <input
        type={type}
        id={name}
        name={name}
        className='form-input w-full'
        onChange={onChange}
        onBlur={onChange}
      />
    </>
  );
};

export default React.memo(UncontrolledTextField);
