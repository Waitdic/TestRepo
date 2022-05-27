import { memo, FC, SyntheticEvent, ReactNode } from 'react';
import { Link } from 'react-router-dom';
import classnames from 'classnames';
//
import { ButtonColors, ButtonVariants } from '@/constants';

type Props = {
  text: string;
  type?: ButtonVariants;
  color?: ButtonColors;
  fullWidth?: boolean;
  onClick?: () => void;
  isLink?: boolean;
  externalLink?: boolean;
  href?: string;
  className?: string | null;
  icon?: ReactNode | JSX.Element;
};

const Button: FC<Props> = ({
  text,
  type = ButtonVariants.BUTTON,
  color = ButtonColors.PRIMARY,
  onClick = null,
  fullWidth = false,
  isLink = false,
  externalLink = false,
  href = '',
  className = null,
  icon = null,
}) => {
  const primaryClassNames =
    'bg-primary hover:bg-primaryHover focus:ring-blue-500 text-white border-transparent';
  const dangerClassNames =
    'bg-red-600 hover:bg-red-700 focus:ring-red-500 text-white border-transparent';
  const warningClassNames =
    'bg-yellow-600 hover:bg-yellow-700 focus:ring-yellow-500 text-white border-transparent';
  const infoClassNames =
    'bg-gray-600 hover:bg-gray-700 focus:ring-gray-500 text-white border-transparent';
  const createClassNames =
    'bg-green-600 hover:bg-green-700 focus:ring-green-500 text-white border-transparent';
  const outlineClassNames =
    'bg-white border border-gray-300 text-gray-700 hover:bg-gray-50 focus:ring-blue-500';
  const fullWidthClassName = 'w-full';

  const handleOnClick = (e: SyntheticEvent) => {
    if (!onClick) return;

    e.preventDefault();
    onClick();
  };

  if (isLink && !externalLink) {
    return (
      <Link to={href}>
        <button
          type={ButtonVariants.BUTTON}
          className={classnames(
            'inline-flex items-center justify-center px-4 py-2 border rounded-md shadow-sm text-sm font-medium focus:outline-none focus:ring-2 focus:ring-offset-2',
            {
              [primaryClassNames]: color === ButtonColors.PRIMARY,
              [dangerClassNames]: color === ButtonColors.DANGER,
              [warningClassNames]: color === ButtonColors.WARNING,
              [infoClassNames]: color === ButtonColors.INFO,
              [createClassNames]: color === ButtonColors.CREATE,
              [outlineClassNames]: color === ButtonColors.OUTLINE,
              [fullWidthClassName]: fullWidth,
              [className as string]: className,
            }
          )}
        >
          {icon && icon}
          {text}
        </button>
      </Link>
    );
  } else if (isLink && externalLink) {
    return (
      <a href={href} target='_blank' rel='noreferrer'>
        <button
          type={ButtonVariants.BUTTON}
          className={classnames(
            'inline-flex items-center justify-center px-4 py-2 border rounded-md shadow-sm text-sm font-medium focus:outline-none focus:ring-2 focus:ring-offset-2',
            {
              [primaryClassNames]: color === ButtonColors.PRIMARY,
              [dangerClassNames]: color === ButtonColors.DANGER,
              [warningClassNames]: color === ButtonColors.WARNING,
              [infoClassNames]: color === ButtonColors.INFO,
              [createClassNames]: color === ButtonColors.CREATE,
              [outlineClassNames]: color === ButtonColors.OUTLINE,
              [fullWidthClassName]: fullWidth,
              [className as string]: className,
            }
          )}
        >
          {icon && icon}
          {text}
        </button>
      </a>
    );
  }

  return (
    <button
      type={type}
      className={classnames(
        'inline-flex items-center justify-center px-4 py-2 border rounded-md shadow-sm text-sm font-medium focus:outline-none focus:ring-2 focus:ring-offset-2',
        {
          [primaryClassNames]: color === ButtonColors.PRIMARY,
          [dangerClassNames]: color === ButtonColors.DANGER,
          [warningClassNames]: color === ButtonColors.WARNING,
          [infoClassNames]: color === ButtonColors.INFO,
          [createClassNames]: color === ButtonColors.CREATE,
          [outlineClassNames]: color === ButtonColors.OUTLINE,
          [fullWidthClassName]: fullWidth,
          [className as string]: className,
        }
      )}
      onClick={handleOnClick}
    >
      {icon && icon}
      {text}
    </button>
  );
};

export default memo(Button);