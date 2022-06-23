import React from 'react';

const NotFoundUser: React.FC = () => {
  return (
    <div className='bg-white min-h-screen px-4 py-16 sm:px-6 sm:py-24 md:grid md:place-items-center lg:px-8'>
      <div className='max-w-max mx-auto'>
        <main className='sm:flex flex-col text-center'>
          <h1 className='text-4xl font-extrabold text-gray-900 tracking-tight sm:text-5xl'>
            User not found
          </h1>
          <p className='mt-1 text-base text-gray-500'>
            Contact support to complete the setup of your account
          </p>
        </main>
      </div>
    </div>
  );
};

export default React.memo(NotFoundUser);
