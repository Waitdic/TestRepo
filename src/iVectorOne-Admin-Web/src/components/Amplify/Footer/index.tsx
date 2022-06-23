import { Flex, Text, useTheme } from '@aws-amplify/ui-react';

const Footer = () => {
  const { tokens } = useTheme();

  return (
    <Flex justifyContent='center' padding={tokens.space.medium}>
      <Text>&copy; Intuitive</Text>
    </Flex>
  );
};

export default Footer;
