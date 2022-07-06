import { FC } from 'react';
import { Amplify } from 'aws-amplify';
import { Authenticator } from '@aws-amplify/ui-react';
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
Amplify.configure(awsExports);

const mapState = (state: RootState) => ({
  app: state.app,
});

type StateProps = ReturnType<typeof mapState>;
type Props = StateProps;

const App: FC<Props> = ({ app }) => {
  return (
    <div id='amplify-container'>
      <Authenticator
        className='mx-auto'
        hideSignUp
        components={{
          Header,
          SignIn: {
            Header: SignInHeader,
            Footer: SignInFooter,
          },
          Footer,
        }}
      >
        {({ user }: { user: { username: string } }) => (
          <AppProvider app={app} user={user} />
        )}
      </Authenticator>
    </div>
  );
};

export default connect(mapState)(App);
