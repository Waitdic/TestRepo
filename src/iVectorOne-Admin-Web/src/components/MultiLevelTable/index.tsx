import React, { useMemo } from 'react';

type Props = {
  data: any;
};

const MultiLevelTable: React.FC<Props> = ({ data }) => {
  console.log(data);

  return <div>MultiLevelTable</div>;
};

export default React.memo(MultiLevelTable);
