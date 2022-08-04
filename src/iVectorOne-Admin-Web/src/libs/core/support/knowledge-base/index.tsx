import React from 'react';
//
import Main from '@/layouts/Main';

type Props = {};

const KnowledgeBase: React.FC<Props> = () => {
  return (
    <Main padding='pb-8'>
      {/* Search area */}
      <div className='relative flex flex-col items-center justify-center px-4 sm:px-6 lg:px-8 py-8 lg:py-16 bg-indigo-500 overflow-hidden'>
        {/* Glow */}
        <div className='absolute pointer-events-none' aria-hidden='true'>
          <svg width='512' height='512' xmlns='http://www.w3.org/2000/svg'>
            <defs>
              <radialGradient
                cx='50%'
                cy='50%'
                fx='50%'
                fy='50%'
                r='50%'
                id='ill-a'
              >
                <stop stopColor='#FFF' offset='0%' />
                <stop stopColor='#FFF' stopOpacity='0' offset='100%' />
              </radialGradient>
            </defs>
            <circle
              style={{ mixBlendMode: 'overlay' }}
              cx='588'
              cy='650'
              r='256'
              transform='translate(-332 -394)'
              fill='url(#ill-a)'
              fillRule='evenodd'
              opacity='.48'
            />
          </svg>
        </div>
        {/* Illustration */}
        <div className='absolute pointer-events-none' aria-hidden='true'>
          <svg
            width='1280'
            height='361'
            xmlns='http://www.w3.org/2000/svg'
            xmlnsXlink='http://www.w3.org/1999/xlink'
          >
            <defs>
              <linearGradient x1='50%' y1='0%' x2='50%' y2='100%' id='ill2-b'>
                <stop stopColor='#A5B4FC' offset='0%' />
                <stop stopColor='#818CF8' offset='100%' />
              </linearGradient>
              <linearGradient
                x1='50%'
                y1='24.537%'
                x2='50%'
                y2='100%'
                id='ill2-c'
              >
                <stop stopColor='#4338CA' offset='0%' />
                <stop stopColor='#6366F1' stopOpacity='0' offset='100%' />
              </linearGradient>
              <path id='ill2-a' d='m64 0 64 128-64-20-64 20z' />
              <path id='ill2-e' d='m40 0 40 80-40-12.5L0 80z' />
            </defs>
            <g fill='none' fillRule='evenodd'>
              <g transform='rotate(51 -92.764 293.763)'>
                <mask id='ill2-d' fill='#fff'>
                  <use xlinkHref='#ill2-a' />
                </mask>
                <use fill='url(#ill2-b)' xlinkHref='#ill2-a' />
                <path
                  fill='url(#ill2-c)'
                  mask='url(#ill2-d)'
                  d='M64-24h80v152H64z'
                />
              </g>
              <g transform='rotate(-51 618.151 -940.113)'>
                <mask id='ill2-f' fill='#fff'>
                  <use xlinkHref='#ill2-e' />
                </mask>
                <use fill='url(#ill2-b)' xlinkHref='#ill2-e' />
                <path
                  fill='url(#ill2-c)'
                  mask='url(#ill2-f)'
                  d='M40.333-15.147h50v95h-50z'
                />
              </g>
            </g>
          </svg>
        </div>
        <div className='relative w-full max-w-2xl mx-auto text-center'>
          <div className='mb-5'>
            <h1 className='text-2xl md:text-3xl text-white font-bold'>
              👋 What Can We Help You Find?
            </h1>
          </div>
        </div>
      </div>

      <div className='px-4 sm:px-6 lg:px-8 py-8 w-full max-w-9xl mx-auto'>
        {/* Sections */}
        <div className='space-y-8'>
          {/* Popular Topics */}
          <div>
            <div className='mb-5'>
              <h2 className='text-xl text-slate-800 font-bold'>Quick Links</h2>
            </div>
            {/* Grid */}
            <div className='grid sm:grid-cols-2 lg:grid-cols-4 lg:sidebar-expanded:grid-cols-2 xl:sidebar-expanded:grid-cols-4 gap-6'>
              {/* Item */}
              <div className='bg-slate-100 rounded-sm text-center p-5'>
                <div className='flex flex-col h-full'>
                  <div className='grow mb-2'>
                    {/* Icon */}
                    <div className='inline-flex w-12 h-12 rounded-full bg-indigo-400'>
                      <svg
                        className='w-12 h-12'
                        viewBox='0 0 48 48'
                        xmlns='http://www.w3.org/2000/svg'
                      >
                        <defs>
                          <linearGradient
                            x1='50%'
                            y1='0%'
                            x2='50%'
                            y2='100%'
                            id='icon1-a'
                          >
                            <stop stopColor='#FFF' offset='0%' />
                            <stop stopColor='#A5B4FC' offset='100%' />
                          </linearGradient>
                        </defs>
                        <g fillRule='nonzero' fill='none'>
                          <path
                            d='M19.236 21.995h-3.333c-.46 0-.833.352-.833.786v9.428c0 .434.373.786.833.786h4.167V22.78c0-.434-.374-.786-.834-.786Z'
                            fill='#4F46E5'
                            opacity='.88'
                          />
                          <path
                            d='M34.234 20.073a2.393 2.393 0 0 0-.735-.116h-5v-2.609c0-3.325-2.157-4.297-3.298-4.347a.828.828 0 0 0-.611.24.888.888 0 0 0-.257.63v4.032L21 22.077v10.924h10.19c1.1.005 2.073-.744 2.392-1.842l2.308-7.826a2.711 2.711 0 0 0-.181-1.988 2.528 2.528 0 0 0-1.475-1.272Z'
                            fill='url(#icon1-a)'
                            transform='translate(-.93 -.005)'
                          />
                        </g>
                      </svg>
                    </div>
                    {/* Content */}
                    <h3 className='text-lg font-semibold text-slate-800 mb-1'>
                      Integration Support
                    </h3>
                    <div className='text-sm text-slate-800'>
                      For help and support during your integration, please email
                      the team and we’ll do our best to assist.
                    </div>
                  </div>
                  {/* Link */}
                  <div>
                    <a
                      className='text-sm font-medium text-indigo-500 hover:text-indigo-600'
                      href='mailto:helpdesk@ivectorone.com'
                    >
                      helpdesk@ivectorone.com
                    </a>
                  </div>
                </div>
              </div>
              <div className='bg-slate-100 rounded-sm text-center p-5'>
                <div className='flex flex-col h-full'>
                  <div className='grow mb-2'>
                    {/* Icon */}
                    <div className='inline-flex w-12 h-12 rounded-full bg-indigo-400'>
                      <svg
                        className='w-12 h-12'
                        viewBox='0 0 48 48'
                        xmlns='http://www.w3.org/2000/svg'
                      >
                        <defs>
                          <linearGradient
                            x1='50%'
                            y1='0%'
                            x2='50%'
                            y2='100%'
                            id='icon1-a'
                          >
                            <stop stopColor='#FFF' offset='0%' />
                            <stop stopColor='#A5B4FC' offset='100%' />
                          </linearGradient>
                        </defs>
                        <g fillRule='nonzero' fill='none'>
                          <path
                            d='M19.236 21.995h-3.333c-.46 0-.833.352-.833.786v9.428c0 .434.373.786.833.786h4.167V22.78c0-.434-.374-.786-.834-.786Z'
                            fill='#4F46E5'
                            opacity='.88'
                          />
                          <path
                            d='M34.234 20.073a2.393 2.393 0 0 0-.735-.116h-5v-2.609c0-3.325-2.157-4.297-3.298-4.347a.828.828 0 0 0-.611.24.888.888 0 0 0-.257.63v4.032L21 22.077v10.924h10.19c1.1.005 2.073-.744 2.392-1.842l2.308-7.826a2.711 2.711 0 0 0-.181-1.988 2.528 2.528 0 0 0-1.475-1.272Z'
                            fill='url(#icon1-a)'
                            transform='translate(-.93 -.005)'
                          />
                        </g>
                      </svg>
                    </div>
                    {/* Content */}
                    <h3 className='text-lg font-semibold text-slate-800 mb-1'>
                      Swagger Documentation
                    </h3>
                    <div className='text-sm text-slate-800'>
                      To create a client SDK, please download the YAML file,
                      which can then be loaded into the Swagger Editor and then
                      transformed into your preferred language
                    </div>
                  </div>
                  {/* Link */}
                  <div>
                    <a
                      className='text-sm font-medium text-indigo-500 hover:text-indigo-600'
                      target='_blank'
                      href='https://app.swaggerhub.com/apis-docs/Intuitivelimited/iVectorOne/v1'
                    >
                      View
                    </a>
                  </div>
                </div>
              </div>
              <div className='bg-slate-100 rounded-sm text-center p-5'>
                <div className='flex flex-col h-full'>
                  <div className='grow mb-2'>
                    {/* Icon */}
                    <div className='inline-flex w-12 h-12 rounded-full bg-indigo-400'>
                      <svg
                        className='w-12 h-12'
                        viewBox='0 0 48 48'
                        xmlns='http://www.w3.org/2000/svg'
                      >
                        <defs>
                          <linearGradient
                            x1='50%'
                            y1='0%'
                            x2='50%'
                            y2='100%'
                            id='icon1-a'
                          >
                            <stop stopColor='#FFF' offset='0%' />
                            <stop stopColor='#A5B4FC' offset='100%' />
                          </linearGradient>
                        </defs>
                        <g fillRule='nonzero' fill='none'>
                          <path
                            d='M19.236 21.995h-3.333c-.46 0-.833.352-.833.786v9.428c0 .434.373.786.833.786h4.167V22.78c0-.434-.374-.786-.834-.786Z'
                            fill='#4F46E5'
                            opacity='.88'
                          />
                          <path
                            d='M34.234 20.073a2.393 2.393 0 0 0-.735-.116h-5v-2.609c0-3.325-2.157-4.297-3.298-4.347a.828.828 0 0 0-.611.24.888.888 0 0 0-.257.63v4.032L21 22.077v10.924h10.19c1.1.005 2.073-.744 2.392-1.842l2.308-7.826a2.711 2.711 0 0 0-.181-1.988 2.528 2.528 0 0 0-1.475-1.272Z'
                            fill='url(#icon1-a)'
                            transform='translate(-.93 -.005)'
                          />
                        </g>
                      </svg>
                    </div>
                    {/* Content */}
                    <h3 className='text-lg font-semibold text-slate-800 mb-1'>
                      Postman Collection
                    </h3>
                    <div className='text-sm text-slate-800'>
                      This contains examples of each call referenced in the API
                      documentation, but with examples relevant to your account
                    </div>
                  </div>
                  {/* Link */}
                  <div>
                    <a
                      className='text-sm font-medium text-indigo-500 hover:text-indigo-600'
                      target='_blank'
                      href='https://documenter.getpostman.com/view/2159662/UyxqD49x'
                    >
                      View
                    </a>
                  </div>
                </div>
              </div>
              <div className='bg-slate-100 rounded-sm text-center p-5'>
                <div className='flex flex-col h-full'>
                  <div className='grow mb-2'>
                    {/* Icon */}
                    <div className='inline-flex w-12 h-12 rounded-full bg-indigo-400'>
                      <svg
                        className='w-12 h-12'
                        viewBox='0 0 48 48'
                        xmlns='http://www.w3.org/2000/svg'
                      >
                        <defs>
                          <linearGradient
                            x1='50%'
                            y1='0%'
                            x2='50%'
                            y2='100%'
                            id='icon1-a'
                          >
                            <stop stopColor='#FFF' offset='0%' />
                            <stop stopColor='#A5B4FC' offset='100%' />
                          </linearGradient>
                        </defs>
                        <g fillRule='nonzero' fill='none'>
                          <path
                            d='M19.236 21.995h-3.333c-.46 0-.833.352-.833.786v9.428c0 .434.373.786.833.786h4.167V22.78c0-.434-.374-.786-.834-.786Z'
                            fill='#4F46E5'
                            opacity='.88'
                          />
                          <path
                            d='M34.234 20.073a2.393 2.393 0 0 0-.735-.116h-5v-2.609c0-3.325-2.157-4.297-3.298-4.347a.828.828 0 0 0-.611.24.888.888 0 0 0-.257.63v4.032L21 22.077v10.924h10.19c1.1.005 2.073-.744 2.392-1.842l2.308-7.826a2.711 2.711 0 0 0-.181-1.988 2.528 2.528 0 0 0-1.475-1.272Z'
                            fill='url(#icon1-a)'
                            transform='translate(-.93 -.005)'
                          />
                        </g>
                      </svg>
                    </div>
                    {/* Content */}
                    <h3 className='text-lg font-semibold text-slate-800 mb-1'>
                      API Documentation
                    </h3>
                    <div className='text-sm text-slate-800'>
                      All of the iVectorOne API requests and responses explained
                      in detail
                    </div>
                  </div>
                  {/* Link */}
                  <div>
                    <a
                      className='text-sm font-medium text-indigo-500 hover:text-indigo-600'
                      target='_blank'
                      href='https://ivectorone.com/wp-content/uploads/2021/09/iVectorOne-API-Documentation.pdf'
                    >
                      View
                    </a>
                  </div>
                </div>
              </div>
            </div>
          </div>

          {/* Popular Guides */}
          <div>
            <div className='mb-5'>
              <h2 className='text-xl text-slate-800 font-bold'>Guides</h2>
            </div>
            {/* Grid */}
            <div className='grid sm:grid-cols-2 gap-6'>
              {/* Item */}
              <div className='w-full p-3 rounded-sm text bg-white border border-slate-200'>
                <div className='flex h-full'>
                  {/* Icon */}
                  <svg
                    className='w-4 h-4 shrink-0 fill-indigo-400 mt-[3px] mr-3'
                    viewBox='0 0 16 16'
                  >
                    <path d='M8 0C3.6 0 0 3.6 0 8s3.6 8 8 8 8-3.6 8-8-3.6-8-8-8zm1 12H7V7h2v5zM8 6c-.6 0-1-.4-1-1s.4-1 1-1 1 .4 1 1-.4 1-1 1z' />
                  </svg>
                  <div className='flex flex-col h-full'>
                    {/* Content */}
                    <div className='grow mb-2'>
                      <div className='font-semibold text-slate-800 mb-1'>
                        API Documentation
                      </div>
                      <div className='text-sm text-slate-800'>
                        All of the iVectorOne API requests and responses
                        explained in detail
                      </div>
                    </div>
                    {/* Link */}
                    <div>
                      <a
                        className='text-sm font-medium text-indigo-500 hover:text-indigo-600'
                        target='_blank'
                        href='https://ivectorone.com/wp-content/uploads/2021/09/iVectorOne-API-Documentation.pdf'
                      >
                        View
                      </a>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </Main>
  );
};

export default React.memo(KnowledgeBase);
