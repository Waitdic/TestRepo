import { useEffect, memo, FC } from 'react';
import { createPortal } from 'react-dom';

type Props = {
  children: any;
  transparent?: boolean;
  flex?: boolean;
  onKeyDown?: () => void;
};

const Modal: FC<Props> = ({
  children,
  transparent = false,
  flex = false,
  onKeyDown,
}) => {
  const modalWrapClassNames = `absolute ${
    transparent ? 'bg-dark backdrop-blur-sm bg-opacity-40' : 'bg-white'
  } rounded-lg shadow-lg overflow-auto w-full h-full ${
    flex ? 'flex justify-center items-center' : ''
  }`;
  const modalRoot = document.getElementById('amplify-container');
  const modalWrap = document.createElement('div');

  modalWrap.className = modalWrapClassNames;

  const handleKeyboardEvent = (e: {
    isTrusted: boolean;
    key: string;
    code: string;
    keyCode: number;
  }) => {
    if (!e.isTrusted) return;
    if (e.keyCode === 27) {
      onKeyDown?.();
    }
  };

  useEffect(() => {
    modalRoot?.appendChild(modalWrap);

    document.addEventListener('keydown', handleKeyboardEvent);

    return () => {
      modalRoot?.removeChild(modalWrap);

      document.removeEventListener('keydown', handleKeyboardEvent);
    };
  });

  return createPortal(
    <div className={modalWrapClassNames} onKeyDown={handleKeyboardEvent}>
      {children}
    </div>,
    modalWrap
  );
};

export default memo(Modal);
