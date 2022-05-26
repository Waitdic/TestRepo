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
  const tenantName =
    currentTenant?.name && currentTenant?.name.length > 20
      ? `${currentTenant?.name.substring(0, 20)}...`
      : currentTenant?.name || '';
  const moduleName = currentModule?.name || '';
  const headerTitle = title || `${tenantName} / ${moduleName}` || '';

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
      {/* Layout Inner */}
      <div className='min-h-screen h-full flex flex-col flex-1'>
        {/* Masthead */}
        <Navigation
          title={headerTitle as string}
          showSidebar={showSidebar}
          setShowSidebar={setShowSidebar}
          handleChangeModule={handleChangeModule}
        />
        {/* Main */}
        <div className='relative flex-1 max-w-screen w-full h-full max-w-3xl mx-auto px-3 sm:px-6 lg:max-w-7xl lg:px-8 lg:grid lg:grid-cols-12 lg:gap-8 py-6'>
          {/* Static sidebar for desktop */}
          <Sidebar />
          <main className='main-content lg:col-span-10'>
            {/* Main Content */}
            {children}
          </main>
        </div>
      </div>
    </>
  );
};

export default memo(MainLayout);
