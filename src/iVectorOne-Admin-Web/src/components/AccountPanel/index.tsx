import React, { useMemo } from 'react';
import { useSelector } from 'react-redux';
//
import { RootState } from '@/store';

type Props = {};

const AccountPanel: React.FC<Props> = ({}) => {
  const user = useSelector((state: RootState) => state.app.user);

  const activeTenant = useMemo(
    () => user?.tenants?.find((tenant) => tenant.isSelected),
    [user]
  );

  return (
    <div className='grow'>
      {/* Panel body */}
      <div className='p-6 space-y-6'>
        <section>
          <h2 className='text-xl leading-snug text-slate-800 font-bold mb-1'>
            Tenant Profile
          </h2>
          <div className='sm:flex sm:flex-col space-y-4 mt-5'>
            <div>
              <h4 className='block text-sm font-medium mb-1'>Name</h4>
              <p className='text-sm'>{activeTenant?.name}</p>
            </div>
            <div>
              <h4 className='block text-sm font-medium mb-1'>Contact Name</h4>
              <p className='text-sm'>{activeTenant?.contactName}</p>
            </div>
            <div>
              <h4 className='block text-sm font-medium mb-1'>Contact Email</h4>
              <p className='text-sm'>{activeTenant?.contactEmail}</p>
            </div>
            <div>
              <h4 className='block text-sm font-medium mb-1'>Active</h4>
              <p className='text-sm'>{activeTenant?.isActive ? 'Yes' : 'No'}</p>
            </div>
          </div>
        </section>
      </div>
      {/* Panel footer */}
      {/* <footer>
        <div className='flex flex-col px-6 py-5 border-t border-slate-200'>
          <div className='flex self-end'>
            <button className='btn border-slate-200 hover:border-slate-300 text-slate-600'>
              Cancel
            </button>
            <button className='btn bg-indigo-500 hover:bg-indigo-600 text-white ml-3'>
              Save Changes
            </button>
          </div>
        </div>
      </footer> */}
    </div>
  );
};

export default React.memo(AccountPanel);
