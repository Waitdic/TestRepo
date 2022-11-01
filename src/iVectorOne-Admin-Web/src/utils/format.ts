import { capitalize } from 'lodash';
//
export const pad2 = (n: number): string => (n < 10 ? `0${n}` : `${n}`);

export const replaceWithSpace = (str: string, isCapitalize = false) => {
  const words = str.split('_');
  const result = words
    .map((word) => {
      if (isCapitalize) {
        return capitalize(word);
      }
      return word + word.slice(1);
    })
    .join(' ');
  return result;
};
