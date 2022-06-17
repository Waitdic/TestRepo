import { Flex, Text, useTheme } from '@aws-amplify/ui-react';

const Footer = () => {
  const { tokens } = useTheme();

  return (
    <Flex justifyContent='center' padding={tokens.space.medium}>
      <Text>&copy; All Rights Reserved</Text>
    </Flex>
  );
};

export default Footer;
