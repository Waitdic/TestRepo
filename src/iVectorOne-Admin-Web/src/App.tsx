import { memo, FC } from 'react';
import { Amplify } from 'aws-amplify';
import { withAuthenticator } from '@aws-amplify/ui-react';
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
type AmplifyProps = {
  user: any;
};
type Props = StateProps & AmplifyProps;

const App: FC<Props> = ({ user, app }) => {
  return <AppProvider app={app} user={user} />;
};

export default connect(mapState)(
  withAuthenticator(memo(App), {
    components: {
      Header,
      SignIn: {
        Header: SignInHeader,
        Footer: SignInFooter,
      },
      Footer,
    },
  })
);
