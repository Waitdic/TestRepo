import { FC, useEffect, Fragment, Dispatch, SetStateAction, memo } from 'react';
import { createPortal } from 'react-dom';
import { Transition } from '@headlessui/react';
import { CheckCircleIcon, XCircleIcon } from '@heroicons/react/outline';
import { XIcon } from '@heroicons/react/solid';
//
import { NotificationStatus } from '@/constants';

type Props = {
  title: string;
  description: string;
  status?: NotificationStatus;
  show: boolean;
  setShow: Dispatch<SetStateAction<boolean>>;
  autoHide?: boolean;
  duration?: number;
};

const Notification: FC<Props> = ({
  title,
  description,
  status = NotificationStatus.SUCCESS,
  show,
  setShow,
  autoHide = true,
  duration = 3,
}) => {
  const notificationRoot = document.getElementById('notification-root');
  const notificationWrap = document.createElement('div');

  useEffect(() => {
    let timer: NodeJS.Timeout;
    notificationRoot?.appendChild(notificationWrap);

    if (autoHide && duration) {
      timer = setTimeout(() => {
        setShow(false);
      }, duration * 1000);
    }

    return () => {
      clearTimeout(timer);
      notificationRoot?.removeChild(notificationWrap);
    };
  }, [show, notificationRoot, notificationWrap, duration, autoHide, setShow]);

  return createPortal(
    <div
      aria-live='assertive'
      className='fixed z-10 inset-0 flex items-end px-4 py-6 pointer-events-none sm:p-6 sm:items-start'
    >
      <div className='w-full flex flex-col items-center space-y-4 sm:items-end'>
        <Transition
          show={show}
          as={Fragment}
          enter='transform ease-out duration-300 transition'
          enterFrom='translate-y-2 opacity-0 sm:translate-y-0 sm:translate-x-2'
          enterTo='translate-y-0 opacity-100 sm:translate-x-0'
          leave='transition ease-in duration-100'
          leaveFrom='opacity-100'
          leaveTo='opacity-0'
        >
          <div className='max-w-sm w-full bg-white shadow-lg rounded-lg pointer-events-auto ring-1 ring-black ring-opacity-5 overflow-hidden'>
            <div className='p-4'>
              <div className='flex items-start'>
                <div className='flex-shrink-0'>
                  {status === NotificationStatus.SUCCESS ? (
                    <CheckCircleIcon
                      className='h-6 w-6 text-green-400'
                      aria-hidden='true'
                    />
                  ) : (
                    <XCircleIcon
                      className='h-6 w-6 text-red-500'
                      aria-hidden='true'
                    />
                  )}
                </div>
                <div className='ml-3 w-0 flex-1 pt-0.5'>
                  <p className='text-sm font-medium text-gray-900'>{title}</p>
                  <p className='mt-1 text-sm text-gray-500'>{description}</p>
                </div>
                <div className='ml-4 flex-shrink-0 flex'>
                  <button
                    className='bg-white rounded-md inline-flex text-gray-400 hover:text-gray-500 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500'
                    onClick={() => setShow(false)}
                  >
                    <span className='sr-only'>Close</span>
                    <XIcon className='h-5 w-5' aria-hidden='true' />
                  </button>
                </div>
              </div>
            </div>
          </div>
        </Transition>
      </div>
    </div>,
    notificationWrap
  );
};

export default memo(Notification);
