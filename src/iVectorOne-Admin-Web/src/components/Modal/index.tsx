import { useEffect, memo, FC } from 'react';
import { createPortal } from 'react-dom';

const Modal: FC<{ children: any }> = ({ children }) => {
  const modalRoot = document.getElementById('modal-root');
  const modalWrap = document.createElement('div');

  useEffect(() => {
    modalRoot?.appendChild(modalWrap);

    return () => {
      modalRoot?.removeChild(modalWrap);
    };
  });

  return createPortal(children, modalWrap);
};

export default memo(Modal);
