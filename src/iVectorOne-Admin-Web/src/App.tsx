import { memo, FC } from 'react';
import { Amplify } from 'aws-amplify';
import { Authenticator, useAuthenticator } from '@aws-amplify/ui-react';
import { connect } from 'react-redux';
//
import '@aws-amplify/ui-react/styles.css';
//
import { RootState } from './store';
import awsExports from './aws-exports';
import Header from './components/Amplify/Header';
import Footer from './components/Amplify/Footer';
import SignInHeader from './components/Amplify/SignIn/Header';
import SignInFooter from './components/Amplify/SignIn/Footer';
import AppProvider from './components/AppProvider';
import NotFoundUser from './components/NotFoundUser';
Amplify.configure(awsExports);

const mapState = (state: RootState) => ({
  app: state.app,
});

type StateProps = ReturnType<typeof mapState>;
type Props = StateProps;

const App: FC<Props> = ({ app }) => {
  const { user, signOut } = useAuthenticator();

  return (
    <Authenticator
      loginMechanisms={['email']}
      components={{
        Header,
        SignIn: {
          Header: SignInHeader,
          Footer: SignInFooter,
        },
        Footer,
      }}
    >
      {() =>
        !!user?.username ? (
          <AppProvider app={app} user={user} signOut={signOut} />
        ) : (
          <NotFoundUser />
        )
      }
    </Authenticator>
  );
};

export default connect(mapState)(memo(App));
