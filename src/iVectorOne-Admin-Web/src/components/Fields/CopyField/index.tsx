import React, { useState } from 'react';
//
import { FiCopy } from 'react-icons/fi';

type Props = {
  value: string | number;
};

const CopyField: React.FC<Props> = ({ value }) => {
  const [copied, setCopied] = useState(false);

  const handleCopyValueToClipboard = () => {
    navigator.clipboard.writeText(value.toString());
    setCopied(true);
    setTimeout(() => {
      setCopied(false);
    }, 2000);
  };

  return (
    <p
      className='relative text-sm break-words cursor-pointer pr-5'
      onClick={handleCopyValueToClipboard}
    >
      <span className='absolute top-1/2 right-0 -translate-y-1/2'>
        <FiCopy />
      </span>
      {value}
      {copied && (
        <span className='absolute -top-8 right-2 text-xs border border-gray-200 rounded-lg p-1'>
          Copied!
        </span>
      )}
    </p>
  );
};

export default React.memo(CopyField);
