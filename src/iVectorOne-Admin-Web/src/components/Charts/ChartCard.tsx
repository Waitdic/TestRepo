import React from 'react';
//
import type { ChartData } from '@/types';
import LineChart from './LineChart';

type Props = {
  header?: React.ReactNode;
  title: string;
  chartData: ChartData;
};

const ChartCard: React.FC<Props> = ({ header, chartData, title }) => {
  return (
    <div className='flex flex-col bg-white shadow-lg rounded-sm border border-slate-200'>
      {!!header && (
        <header className='px-5 py-4 border-b border-slate-100 flex items-center'>
          {header}
        </header>
      )}
      {!!chartData && (
        <LineChart title={title} data={chartData} width={200} height={200} />
      )}
    </div>
  );
};

export default React.memo(ChartCard);
