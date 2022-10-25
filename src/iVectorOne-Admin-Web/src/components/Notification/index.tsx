import { FC, useEffect, Fragment, memo, useState } from 'react';
import { createPortal } from 'react-dom';
import { Transition } from '@headlessui/react';
import { useDispatch, useSelector } from 'react-redux';
//
import { RootState } from '@/store';
import { NotificationStatus } from '@/constants';

type Props = {
  title?: string;
  description: string;
  status?: NotificationStatus;
  autoHide?: boolean;
  duration?: number;
  errorInstance?: string;
};

const Notification: FC<Props> = ({
  title: _title,
  description,
  status = NotificationStatus.SUCCESS,
  autoHide = true,
  duration = 3,
  errorInstance = null,
}) => {
  const dispatch = useDispatch();

  const notification = useSelector(
    (state: RootState) => state.app.notification
  );

  const [copied, setCopied] = useState(false);

  const notificationRoot = document.getElementById('amplify-container');
  const notificationWrap = document.createElement('div');

  const typeIcon = () => {
    switch (status) {
      // case NotificationStatus.WARNING:
      //   return (
      //     <svg className="w-4 h-4 shrink-0 fill-current opacity-80 mt-[3px] mr-3" viewBox="0 0 16 16">
      //       <path d="M8 0C3.6 0 0 3.6 0 8s3.6 8 8 8 8-3.6 8-8-3.6-8-8-8zm0 12c-.6 0-1-.4-1-1s.4-1 1-1 1 .4 1 1-.4 1-1 1zm1-3H7V4h2v5z" />
      //     </svg>
      //   );
      case NotificationStatus.ERROR:
        return (
          <svg
            className='w-4 h-4 shrink-0 fill-current opacity-80 mt-[3px] mr-3'
            viewBox='0 0 16 16'
          >
            <path d='M8 0C3.6 0 0 3.6 0 8s3.6 8 8 8 8-3.6 8-8-3.6-8-8-8zm3.5 10.1l-1.4 1.4L8 9.4l-2.1 2.1-1.4-1.4L6.6 8 4.5 5.9l1.4-1.4L8 6.6l2.1-2.1 1.4 1.4L9.4 8l2.1 2.1z' />
          </svg>
        );
      case NotificationStatus.SUCCESS:
        return (
          <svg
            className='w-4 h-4 shrink-0 fill-current opacity-80 mt-[3px] mr-3'
            viewBox='0 0 16 16'
          >
            <path d='M8 0C3.6 0 0 3.6 0 8s3.6 8 8 8 8-3.6 8-8-3.6-8-8-8zM7 11.4L3.6 8 5 6.6l2 2 4-4L12.4 6 7 11.4z' />
          </svg>
        );
      default:
        return (
          <svg
            className='w-4 h-4 shrink-0 fill-current opacity-80 mt-[3px] mr-3'
            viewBox='0 0 16 16'
          >
            <path d='M8 0C3.6 0 0 3.6 0 8s3.6 8 8 8 8-3.6 8-8-3.6-8-8-8zm1 12H7V7h2v5zM8 6c-.6 0-1-.4-1-1s.4-1 1-1 1 .4 1 1-.4 1-1 1z' />
          </svg>
        );
    }
  };

  const typeColor = () => {
    switch (status) {
      // case NotificationStatus.WARNING:
      //   return 'bg-amber-100 border-amber-200 text-amber-600';
      case NotificationStatus.ERROR:
        return 'bg-rose-100 border-rose-200 text-rose-600';
      case NotificationStatus.SUCCESS:
        return 'bg-emerald-100 border-emerald-200 text-emerald-600';
      default:
        return 'bg-indigo-100 border-indigo-200 text-indigo-500';
    }
  };

  const handleCopyErrorInstance = () => {
    if (errorInstance === null) {
      dispatch.app.resetNotification();
      return;
    }
    setCopied(true);
    navigator.clipboard.writeText(errorInstance);
  };

  useEffect(() => {
    let timer: NodeJS.Timeout;
    notificationRoot?.appendChild(notificationWrap);

    if (autoHide && duration) {
      timer = setTimeout(() => {
        dispatch.app.resetNotification();
      }, duration * 1000);
    }

    return () => {
      clearTimeout(timer);
      dispatch.app.resetNotification();
      notificationRoot?.removeChild(notificationWrap);
      setCopied(false);
    };
  }, [notificationRoot, notificationWrap, duration, autoHide]);

  return createPortal(
    <div
      aria-live='assertive'
      className='fixed z-50 inset-0 pb-16 pt-20 flex sm:items-start'
      onClick={handleCopyErrorInstance}
    >
      <div className='lg:sidebar-expanded:ml-[16rem] w-full flex flex-col items-center space-y-4'>
        <Transition
          show={notification !== null}
          as={Fragment}
          enter='transform ease-out duration-300 transition'
          enterFrom='translate-y-2 opacity-0 sm:translate-y-0 sm:translate-x-2'
          enterTo='translate-y-0 opacity-100 sm:translate-x-0'
          leave='transition ease-in duration-100'
          leaveFrom='opacity-100'
          leaveTo='opacity-0'
        >
          <div
            className={`relative max-w-[60vw] px-4 py-2 rounded-sm text-sm border ${typeColor()}`}
          >
            {copied && (
              <div className='absolute -top-8 right-0 bg-gray-700 p-1 rounded-sm'>
                <p className='text-sm text-gray-100'>Copied</p>
              </div>
            )}
            <div className='flex w-full justify-between items-start'>
              <div className='flex'>
                {typeIcon()}
                <div className='text-center'>{description}</div>
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
