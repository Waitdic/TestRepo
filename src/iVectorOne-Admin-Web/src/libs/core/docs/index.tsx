import Main from '@/layouts/Main';
import React, { useCallback, useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';

const Docs: React.FC = () => {
  const params = useParams();
  const navigate = useNavigate();

  const { id: docId } = params;

  const [doc, setDoc] = useState<{
    component: React.FC | null;
  }>({
    component: null,
  });

  const fetchDoc = useCallback(async () => {
    const { ReactComponent } = await import(`../../../docs/${docId}.md`);

    if (!!ReactComponent) {
      setDoc({ component: ReactComponent });
    } else {
      console.error(`Doc ${docId} not found`);
      navigate('/404');
    }
  }, [docId]);

  useEffect(() => {
    fetchDoc();
  }, [fetchDoc]);

  return (
    <Main>
      <div className='prose'>{!!doc.component && <doc.component />}</div>
    </Main>
  );
};

export default React.memo(Docs);
