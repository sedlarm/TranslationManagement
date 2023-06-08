import 'bootstrap/dist/css/bootstrap.css';
import React from 'react';
import { createRoot } from 'react-dom/client';
import { BrowserRouter } from 'react-router-dom';
import App from './App';
import * as serviceWorkerRegistration from './serviceWorkerRegistration';
import reportWebVitals from './reportWebVitals';

const baseUrl = document.getElementsByTagName('base')[0].getAttribute('href');
const rootElement = document.getElementById('root');
const root = createRoot(rootElement);

const originalFetch = global.fetch;

export const applyBaseUrlToFetch = (baseUrl) => {
    // replace the global fetch() with our version where we prefix the given URL with a baseUrl
    global.fetch = (url, options) => {
        const finalUrl = baseUrl + url;
        return originalFetch(finalUrl, options);
    };
};

if (process.env.REACT_APP_BACKEND_API_BASE_URL !== undefined && process.env.REACT_APP_BACKEND_API_BASE_URL !== '') {
    console.log(process.env.REACT_APP_BACKEND_API_BASE_URL);
    applyBaseUrlToFetch(process.env.REACT_APP_BACKEND_API_BASE_URL);
}

root.render(
  <BrowserRouter basename={baseUrl}>
    <App />
  </BrowserRouter>);

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://cra.link/PWA
serviceWorkerRegistration.unregister();

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
