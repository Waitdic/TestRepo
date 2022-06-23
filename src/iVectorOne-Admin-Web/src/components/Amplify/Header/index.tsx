import { Image, useTheme } from '@aws-amplify/ui-react';

const Header = () => {
  const { tokens } = useTheme();

  return (
    <Image
      alt='logo'
      src='/iVectorOne_Logo-768x207.png'
      padding={tokens.space.medium}
      style={{ maxWidth: '340px', margin: '0 auto 30px', display: 'block' }}
    />
  );
};

export default Header;
