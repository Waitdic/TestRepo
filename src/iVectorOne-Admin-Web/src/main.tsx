import React from 'react';
import ReactDOM from 'react-dom';
import { BrowserRouter } from 'react-router-dom';
import { HelmetProvider } from 'react-helmet-async';
import { Provider as ReduxProvider } from 'react-redux';
import { store } from './store';
//
import './index.scss';
//
import App from './App';

ReactDOM.render(
  <React.StrictMode>
    <BrowserRouter>
      <ReduxProvider store={store}>
        <HelmetProvider>
          <App />
        </HelmetProvider>
      </ReduxProvider>
    </BrowserRouter>
  </React.StrictMode>,
  document.getElementById('root')
);
