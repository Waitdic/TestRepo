import React, { useState } from 'react';

type Props = {};

const AccountPanel: React.FC<Props> = ({}) => {
  const [formFields, setFormFields] = useState({
    name: '',
    businessId: '',
    location: '',
    email: '',
    sync: false,
  });

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    if (e.target.type === 'checkbox') {
      setFormFields({
        ...formFields,
        [name]: e.target.checked,
      });
      return;
    }
    setFormFields({ ...formFields, [name]: value });
  };

  return (
    <div className='grow'>
      {/* Panel body */}
      <div className='p-6 space-y-6'>{/* placeholder */}</div>
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
