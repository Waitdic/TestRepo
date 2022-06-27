import React, { useState } from 'react';

type Props = {
  children: any;
  activecondition?: boolean;
};

const SidebarLinkGroup: React.FC<Props> = ({
  children,
  activecondition = false,
}) => {
  const [open, setOpen] = useState(activecondition);

  const handleClick = () => {
    setOpen(!open);
  };

  return (
    <li
      className={`px-3 py-2 rounded-sm mb-0.5 last:mb-0 ${
        activecondition && 'bg-slate-900'
      }`}
    >
      {children(handleClick, open)}
    </li>
  );
};

export default React.memo(SidebarLinkGroup);
