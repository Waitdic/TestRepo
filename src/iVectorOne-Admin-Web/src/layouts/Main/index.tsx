import { useState, memo, ReactNode, FC } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import { useDispatch, useSelector } from 'react-redux';
//
import { Module } from '@/types';
import { useSetActiveModule } from '@/utils/set-active-module';
//
import { Sidebar, Navigation } from '@/components';
import { RootState } from '@/store';

type Props = {
  title?: string | ReactNode;
  children: ReactNode;
};

const MainLayout: FC<Props> = ({ title = null, children }) => {
  const navigate = useNavigate();
  const user = useSelector((state: RootState) => state.app.user);
  const modules = useSelector((state: RootState) => state.app.modules);
  const dispatch = useDispatch();
  const { pathname } = useLocation();
  useSetActiveModule(pathname);

  const [showSidebar, setShowSidebar] = useState<boolean>(false);

  const currentTenant = user?.tenants.filter((tenant) => tenant.isActive)[0];
  const currentModule = modules.filter((module) => module.isActive)[0];

  const handleChangeModule = (moduleId: string, uri: string) => {
    const updatedModuleList: Module[] = modules.map((module) => ({
      ...module,
      isActive: module.moduleId === moduleId ? true : false,
    }));
    dispatch.app.updateModuleList(updatedModuleList);
    navigate(uri);
  };

  return (
    <>
      {/* Sidebar */}
      <Sidebar showSidebar={showSidebar} setShowSidebar={setShowSidebar} />
      <div className='md:pl-64 flex flex-col flex-1'>
        {/* Navigation */}
        <Navigation setShowSidebar={setShowSidebar} />
        {/* Main content */}
        <main className='flex-1'>
          <div className='relative py-6'>
            <div className='max-w-7xl mx-auto px-4 sm:px-6 md:px-8'>
              <div className='py-4'>{children}</div>
            </div>
          </div>
        </main>
      </div>
    </>
  );
};

export default memo(MainLayout);
