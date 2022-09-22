import { ButtonColors } from '@/constants';
import { Transition } from '@headlessui/react';
import React, { Fragment } from 'react';
import Button from '../Button';
import Modal from '../Modal';

type Props = {
  title: string;
  description: React.ReactNode;
  show: boolean;
  setShow: React.Dispatch<React.SetStateAction<boolean>>;
  onConfirm: () => void;
  confirmButtonText?: string;
  onCancel?: () => void;
};

const ConfirmModal: React.FC<Props> = ({
  title,
  description,
  show,
  setShow,
  onConfirm,
  onCancel,
  confirmButtonText = 'Delete',
}) => {
  const handleCancel = () => {
    setShow(false);
    onCancel?.();
  };

  return (
    <>
      {show && (
        <Modal transparent>
          <div
            aria-live='assertive'
            className='fixed z-50 inset-0 py-16 flex sm:items-start'
          >
            <section className='lg:sidebar-expanded:ml-[16rem] w-full flex flex-col items-center space-y-4'>
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
                <div className='bg-white mt-5 p-5 flex flex-col items-center gap-2'>
                  <h3 className='text-lg text-dark font-bold'>{title}</h3>
                  <p className='text-base text-dark font-medium'>
                    {description}
                  </p>
                  <div className='mt-2'>
                    <Button
                      text='Cancel'
                      color={ButtonColors.OUTLINE}
                      className='ml-4'
                      onClick={handleCancel}
                    />
                    <Button
                      text={confirmButtonText}
                      color={ButtonColors.DANGER}
                      className='ml-4'
                      onClick={onConfirm}
                    />
                  </div>
                </div>
              </Transition>
            </section>
          </div>
        </Modal>
      )}
    </>
  );
};

export default React.memo(ConfirmModal);
