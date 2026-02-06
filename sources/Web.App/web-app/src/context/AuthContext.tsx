import React from 'react';
import { IAuthContext } from '../lib/interfaces/IAuthContext';

const AuthContext = React.createContext<IAuthContext | null>(null);

export default AuthContext;
