import React from 'react';
import { useAuth } from '../../hooks/useAuth';

interface IProps {}

const HomePage: React.FC<IProps> = () => {
  const { userData } = useAuth();
  return <div>{userData?.emailAddress}</div>;
};

export default HomePage;
