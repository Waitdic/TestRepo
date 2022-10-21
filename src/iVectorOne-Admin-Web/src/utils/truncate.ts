const truncate = (str: string, length: number) => {
  if (str.length > length) {
    return str.substring(0, length) + '...';
  }
  return str;
};

export default truncate;
