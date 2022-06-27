import MainLayout from '@/layouts/Main';
//
import { useSlug } from '@/utils/use-slug';

export const ModuleInfo = () => {
  const { slug } = useSlug();

  return (
    <MainLayout>
      <div className='py-4 col-span-12'>
        <div className='flex flex-col'>
          <div className='-my-2 overflow-x-auto sm:-mx-6 lg:-mx-8'>
            <div className='py-2 align-middle inline-block min-w-full sm:px-6 lg:px-8 bg-white rounded-sm'>
              <h3 className='text-xl text-gray-900'>Module Slug: {slug}</h3>
              <p className='text-sm text-gray-900'>
                This lib just a Test Module getting information about the module
                with fetchAPI use the useSlug hook.
              </p>
            </div>
          </div>
        </div>
      </div>
    </MainLayout>
  );
};
