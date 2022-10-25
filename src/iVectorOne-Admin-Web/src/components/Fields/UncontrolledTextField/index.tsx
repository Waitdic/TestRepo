import React, { forwardRef, useRef, useState } from 'react';
//
import type { Property } from '@/types';
import useOnClickOutside from '@/utils/useOnClickOutside';

type Props = {
  name: string;
  label?: string;
  type?: 'text' | 'number' | 'email' | 'password';
  onChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
  required?: boolean;
  ref?: React.Ref<HTMLInputElement>;
  value?: string | number;
  placeholder?: string;
  autoComplete?: {
    results: Property[];
    handler: (selectedResult: number) => void;
    cleanup: () => void;
  };
};

const UncontrolledTextField: React.FC<Props> = forwardRef(
  (
    {
      label = '',
      name,
      type = 'text',
      onChange,
      required = false,
      value = '',
      placeholder = '',
      autoComplete,
    },
    ref
  ) => {
    const autoCompleteRef = useRef<HTMLDivElement>(null);

    const [showAutoComplete, setShowAutoComplete] = useState(false);

    useOnClickOutside(autoCompleteRef, () => {
      setShowAutoComplete(false);
    });

    const handleShowAutoComplete = () => {
      if (!!autoComplete) {
        setShowAutoComplete(true);
      }
    };

    const handleAutoComplete = (selectedResult: number) => {
      if (!!autoComplete) {
        setShowAutoComplete(false);
        autoComplete.handler(selectedResult);
        autoComplete.cleanup();
      }
    };

    return (
      <div className='relative' ref={autoCompleteRef}>
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
          placeholder={placeholder}
          onClick={handleShowAutoComplete}
        />
        {showAutoComplete && !!autoComplete && autoComplete.results.length > 0 && (
          <div className='absolute z-50 top-full left-0 w-full max-h-[400px] overflow-auto bg-white border border-slate-200 rounded-sm shadow-lg'>
            {autoComplete.results.map((result) => (
              <div
                key={result.propertyId}
                className='p-2 cursor-pointer hover:bg-slate-100'
                onClick={() => handleAutoComplete(result.propertyId)}
              >
                {result.name}
              </div>
            ))}
          </div>
        )}
      </div>
    );
  }
);

export default React.memo(UncontrolledTextField);
