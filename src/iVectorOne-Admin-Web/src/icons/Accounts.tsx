import React from 'react';

const Accounts: React.FC<{ className: string }> = ({ className }) => {
  return (
    <svg
      xmlns='http://www.w3.org/2000/svg'
      width='44'
      height='44'
      viewBox='0 0 44 44'
      className={className}
    >
      <path
        d='M24,29.25,18.75,24,24,18.75,29.25,24Zm-4.25-14.7L15.6,10.4,24,2l8.4,8.4-4.15,4.15L24,10.3ZM10.4,32.4,2,24l8.4-8.4,4.15,4.15L10.3,24l4.25,4.25Zm27.2,0-4.15-4.15L37.7,24l-4.25-4.25L37.6,15.6,46,24ZM24,46l-8.4-8.4,4.15-4.15L24,37.7l4.25-4.25L32.4,37.6Z'
        transform='translate(-2 -2)'
      />
    </svg>
  );
};

export default React.memo(Accounts);
