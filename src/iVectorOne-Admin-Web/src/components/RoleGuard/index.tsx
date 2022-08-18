import React, { useEffect, useMemo } from 'react';
import { useSelector } from 'react-redux';
import { useNavigate } from 'react-router-dom';
//
import { RootState } from '@/store';

type Props = {
  children: React.ReactNode;
  withRedirect?: boolean;
};

const RoleGuard: React.FC<Props> = ({ children, withRedirect = false }) => {
  const navigate = useNavigate();

  const user = useSelector((state: RootState) => state.app.user);

  const isAdmin = useMemo(() => {
    const hasValidRole = user?.authorisations.filter(
      (auth) => auth.object === 'system' && auth.relationship === 'owner'
    );
    return !!hasValidRole;
  }, [user]);

  useEffect(() => {
    if (!isAdmin && withRedirect && !!user) {
      navigate('/');
    }
  }, [isAdmin, withRedirect, user]);

  if (!isAdmin) return null;

  return <>{children}</>;
};

export default React.memo(RoleGuard);
