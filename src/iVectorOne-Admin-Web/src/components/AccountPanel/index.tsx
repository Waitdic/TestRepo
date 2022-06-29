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
      <div className='p-6 space-y-6'>
        <h2 className='text-2xl text-slate-800 font-bold mb-5'>My Account</h2>
        {/* Picture */}
        <section>
          <div className='flex items-center'>
            <div className='mr-4'>
              <img
                className='w-20 h-20 rounded-full'
                src='/images/user-avatar-80.png'
                width='80'
                height='80'
                alt='User upload'
              />
            </div>
            <button className='btn-sm bg-indigo-500 hover:bg-indigo-600 text-white'>
              Change
            </button>
          </div>
        </section>
        {/* Business Profile */}
        <section>
          <h2 className='text-xl leading-snug text-slate-800 font-bold mb-1'>
            Business Profile
          </h2>
          <div className='text-sm'>
            Excepteur sint occaecat cupidatat non proident, sunt in culpa qui
            officia deserunt mollit.
          </div>
          <div className='sm:flex sm:items-center space-y-4 sm:space-y-0 sm:space-x-4 mt-5'>
            <div className='sm:w-1/3'>
              <label className='block text-sm font-medium mb-1' htmlFor='name'>
                Business Name
              </label>
              <input
                id='name'
                name='name'
                className='form-input w-full'
                type='text'
                value={formFields.name}
                onChange={handleChange}
                onBlur={handleChange}
              />
            </div>
            <div className='sm:w-1/3'>
              <label
                className='block text-sm font-medium mb-1'
                htmlFor='business-id'
              >
                Business ID
              </label>
              <input
                id='businessId'
                name='businessId'
                className='form-input w-full'
                type='text'
                value={formFields.businessId}
                onChange={handleChange}
                onBlur={handleChange}
              />
            </div>
            <div className='sm:w-1/3'>
              <label
                className='block text-sm font-medium mb-1'
                htmlFor='location'
              >
                Location
              </label>
              <input
                id='location'
                name='location'
                className='form-input w-full'
                type='text'
                value={formFields.location}
                onChange={handleChange}
                onBlur={handleChange}
              />
            </div>
          </div>
        </section>
        {/* Email */}
        <section>
          <h2 className='text-xl leading-snug text-slate-800 font-bold mb-1'>
            Email
          </h2>
          <div className='text-sm'>
            Excepteur sint occaecat cupidatat non proident sunt in culpa qui
            officia.
          </div>
          <div className='flex flex-wrap mt-5'>
            <div className='mr-2'>
              <label className='sr-only' htmlFor='email'>
                Business email
              </label>
              <input
                id='email'
                name='email'
                className='form-input'
                type='email'
                value={formFields.email}
                onChange={handleChange}
                onBlur={handleChange}
              />
            </div>
            <button className='btn border-slate-200 hover:border-slate-300 shadow-sm text-indigo-500'>
              Change
            </button>
          </div>
        </section>
        {/* Password */}
        <section>
          <h2 className='text-xl leading-snug text-slate-800 font-bold mb-1'>
            Password
          </h2>
          <div className='text-sm'>
            You can set a permanent password if you don't want to use temporary
            login codes.
          </div>
          <div className='mt-5'>
            <button className='btn border-slate-200 shadow-sm text-indigo-500'>
              Set New Password
            </button>
          </div>
        </section>
        {/* Smart Sync */}
        <section>
          <h2 className='text-xl leading-snug text-slate-800 font-bold mb-1'>
            Smart Sync update for Mac
          </h2>
          <div className='text-sm'>
            With this update, online-only files will no longer appear to take up
            hard drive space.
          </div>
          <div className='flex items-center mt-5'>
            <div className='form-switch'>
              <input
                type='checkbox'
                id='sync'
                name='sync'
                className='sr-only'
                checked={formFields.sync}
                onChange={handleChange}
                onBlur={handleChange}
              />
              <label className='bg-slate-400' htmlFor='sync'>
                <span className='bg-white shadow-sm' aria-hidden='true'></span>
                <span className='sr-only'>Enable smart sync</span>
              </label>
            </div>
            <div className='text-sm text-slate-400 italic ml-2'>
              {formFields.sync ? 'On' : 'Off'}
            </div>
          </div>
        </section>
      </div>
      {/* Panel footer */}
      <footer>
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
      </footer>
    </div>
  );
};

export default React.memo(AccountPanel);
