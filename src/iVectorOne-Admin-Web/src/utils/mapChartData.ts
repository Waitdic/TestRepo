import type { ChartData } from '@/types';
import { pad2 } from './format';
import { tailwindConfig } from './Utils';

const mapChartData = (
  data: {
    time: number;
    currentTotal: number | null;
    previousTotal: number | null;
  }[],
  rawColors: string[]
): ChartData => {
  const labels = data.map((item) => pad2(item.time + 1));
  const colors = rawColors.map(
    (color) => tailwindConfig()?.theme?.colors?.[color]?.[500]
  );
  const datasets = [
    {
      label: 'Today',
      data: data.map((item) =>
        item.currentTotal !== null ? item.currentTotal : 0
      ),
      borderColor: colors[0],
      fill: false,
      borderWidth: 2,
      tension: 0,
      pointRadius: 0,
      pointHoverRadius: 3,
      pointBackgroundColor: colors[0],
      clip: 20,
    },
    {
      label: 'Last Week',
      data: data.map((item) =>
        item.previousTotal !== null ? item.previousTotal : 0
      ),
      borderColor: colors[1],
      fill: false,
      borderWidth: 2,
      tension: 0,
      pointRadius: 0,
      pointHoverRadius: 3,
      pointBackgroundColor: colors[1],
      clip: 20,
    },
  ];

  return {
    labels,
    datasets,
  };
};

export default mapChartData;
