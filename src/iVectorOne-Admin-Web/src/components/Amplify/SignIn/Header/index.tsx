import { Heading, useTheme } from '@aws-amplify/ui-react';

const SignInHeader = () => {
  const { tokens } = useTheme();

  return (
    <Heading
      level={5}
      textAlign='center'
      padding={`${tokens.space.xl} ${tokens.space.xl} 0`}
    >
      Sign in to your Account with email and password
    </Heading>
  );
};

export default SignInHeader;
