import React from 'react';
import './App.css';
import AppRouter from './lib/router/AppRouter';
import AuthContextProvider from './context/AuthContextProvider';

const App: React.FC = () => {
  return (
    <AuthContextProvider>
      <AppRouter />
    </AuthContextProvider>
  );
};

export default App;
