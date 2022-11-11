import React, { forwardRef, useRef, useState } from 'react';
import { v4 as uuid } from 'uuid';
//
import type { Property } from '@/types';
import useOnClickOutside from '@/utils/useOnClickOutside';
import { Spinner } from '@/components';

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
  };
  isLoading?: boolean;
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
      isLoading = false,
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
      autoComplete?.handler?.(selectedResult);
      setShowAutoComplete(false);
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
          onFocus={handleShowAutoComplete}
          placeholder={placeholder}
          autoComplete={!!autoComplete ? 'off' : ''}
        />
        {isLoading && (
          <div className='absolute h-8 bottom-[4px] right-0 pr-3 flex items-center pointer-events-none'>
            <Spinner className='w-5 h-5' />
          </div>
        )}
        {showAutoComplete && !!autoComplete && autoComplete.results.length > 0 && (
          <div className='absolute z-50 top-full left-0 w-full max-h-[400px] overflow-auto bg-white border border-slate-200 rounded-sm shadow-lg'>
            {autoComplete.results.map((result) => {
              const id = uuid();
              return (
                <div
                  key={id}
                  className='p-2 cursor-pointer hover:bg-slate-100'
                  onClick={() => handleAutoComplete(result.propertyId)}
                >
                  {result.name}
                </div>
              );
            })}
          </div>
        )}
      </div>
    );
  }
);

export default React.memo(UncontrolledTextField);
