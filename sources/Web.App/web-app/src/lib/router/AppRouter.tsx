import React from 'react';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { useAuth } from '../../hooks/useAuth';
import LoginPage from '../../views/loginPage/LoginPage';
import AppLayout from '../../components/layout/AppLayout';
import HomePage from '../../views/homePage/HomePage';
import VocabularyValicationPage from '../../views/vocabulary/VocabularyValicationPage';

const AppRouter: React.FC = () => {
  const authResult = useAuth();

  if (!authResult.isAuthenticated) {
    return <LoginPage />;
  }

  return (
    <BrowserRouter>
      <AppLayout>
        <Routes>
          <Route path="/" Component={HomePage} />
          <Route path="/vocabulary-validation" Component={VocabularyValicationPage} />
        </Routes>
      </AppLayout>
    </BrowserRouter>
  );
};

export default AppRouter;
