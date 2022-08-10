import { useEffect, memo, FC } from 'react';
import { createPortal } from 'react-dom';

type Props = { children: any; transparent?: boolean };

const Modal: FC<Props> = ({ children, transparent = false }) => {
  const modalRoot = document.getElementById('amplify-container');
  const modalWrap = document.createElement('div');
  modalWrap.className = `absolute ${
    transparent ? 'bg-dark backdrop-blur-sm bg-opacity-40' : 'bg-white'
  } rounded-lg shadow-lg overflow-auto w-full h-full`;

  useEffect(() => {
    modalRoot?.appendChild(modalWrap);

    return () => {
      modalRoot?.removeChild(modalWrap);
    };
  });

  return createPortal(children, modalWrap);
};

export default memo(Modal);
