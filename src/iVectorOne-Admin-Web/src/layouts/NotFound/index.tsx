import { FC } from 'react';
//
import { Button } from '@/components';

const NotFound: FC = () => {
  return (
    <div className='bg-white min-h-screen px-4 py-16 sm:px-6 sm:py-24 md:grid md:place-items-center mx-auto lg:px-8'>
      <div className='max-w-max mx-auto'>
        <main className='flex flex-col items-center'>
          <div className='sm:flex'>
            <div>
              <img
                className='max-w-[400px]'
                src='/logo-black.png'
                alt='iVectorOne powered by Intuitive'
              />
            </div>
            <div className='sm:ml-6'>
              <div className='sm:border-l sm:border-gray-200 sm:pl-6'>
                <h1 className='text-4xl font-extrabold text-dark-heading tracking-tight sm:text-5xl'>
                  Page not found
                </h1>
                <p className='mt-1 text-base'>
                  Please check the URL in the address bar and try again.
                </p>
              </div>
              <div className='mt-10 flex space-x-3 sm:border-l sm:border-transparent sm:pl-6'>
                <Button text='Go back home' isLink href='/' />
              </div>
            </div>
          </div>
        </main>
      </div>
    </div>
  );
};

export default NotFound;
