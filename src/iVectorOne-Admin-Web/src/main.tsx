import React from 'react';
import ReactDOM from 'react-dom';
import { BrowserRouter } from 'react-router-dom';
import { Provider as ReduxProvider } from 'react-redux';
import { store } from './store';
import { Authenticator } from '@aws-amplify/ui-react';
//
import './index.scss';
//
import App from './App';

ReactDOM.render(
  <React.StrictMode>
    <BrowserRouter>
      <ReduxProvider store={store}>
        <Authenticator.Provider>
          <App />
        </Authenticator.Provider>
      </ReduxProvider>
    </BrowserRouter>
  </React.StrictMode>,
  document.getElementById('root')
);
