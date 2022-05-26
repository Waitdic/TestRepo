import { useParams } from 'react-router-dom';

export function useSlug() {
  const { slug } = useParams();

  return { slug };
}
