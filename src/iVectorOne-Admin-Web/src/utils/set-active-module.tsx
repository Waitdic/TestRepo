import { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
//
import { Module } from '@/types';
import { RootState } from '@/store';

export function useSetActiveModule(pathname: string) {
  const dispatch = useDispatch();
  const modules = useSelector((state: RootState) => state.app.modules);

  const isActiveModule = modules.filter(
    (module) => module.uri.split('/')[1] === pathname.split('/')[1]
  );

  useEffect(() => {
    if (!isActiveModule.length) return;

    if (modules && pathname && !isActiveModule[0]?.isActive) {
      const updatedModuleList: Module[] = modules.map((module) => {
        if (module.uri.split('/')[1] === pathname.split('/')[1]) {
          return {
            ...module,
            isActive: true,
          };
        } else if (module.isActive) {
          return { ...module, isActive: false };
        } else {
          return module;
        }
      });

      dispatch.app.updateModuleList(updatedModuleList);
    }
  }, [pathname, modules, isActiveModule]);
}
