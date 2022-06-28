import React from 'react';

type Props = {
  className?: string;
};

const Dashboard: React.FC<Props> = ({ className = '' }) => {
  return (
    <svg
      xmlns='http://www.w3.org/2000/svg'
      className={className}
      width='36'
      height='36'
      viewBox='0 0 36 36'
    >
      <path
        d='M25.5,19.5V6H42V19.5ZM6,25.5V6H22.5V25.5ZM25.5,42V22.5H42V42ZM6,42V28.5H22.5V42ZM9,22.5H19.5V9H9ZM28.5,39H39V25.5H28.5Zm0-22.5H39V9H28.5ZM9,39H19.5V31.5H9ZM19.5,22.5ZM28.5,16.5ZM28.5,25.5ZM19.5,31.5Z'
        transform='translate(-6 -6)'
      />
    </svg>
  );
};

export default React.memo(Dashboard);
