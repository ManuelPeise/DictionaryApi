import React from 'react';
import AuthContext from './AuthContext';
import { IAuthenticationState } from '../lib/interfaces/IAuthenticationState';
import { IAuthenticationRequest } from '../lib/interfaces/IAuthenticationRequest';
import { IAuthenticationResponse } from '../lib/interfaces/IAuthenticationResponse';
import { IUserData } from '../lib/interfaces/IUserData';
import { ITokenResponse } from '../lib/interfaces/ITokenResponse';

interface IAuthContextProps extends React.PropsWithChildren {}

const tokenStorageKey = 'authTokenData';
const userDataStorageKey = 'userData';

const AuthContextProvider: React.FC<IAuthContextProps> = (props: IAuthContextProps) => {
  const { children } = props;

  const [authContextProps, setAuthContextProps] = React.useState<IAuthenticationState>({
    isAuthenticated: false,
    userData: null,
    tokenData: {
      accessToken: null,
      refreshToken: null,
    },
  });

  const sendAuthenticationRequest = React.useCallback(
    async (requestModel: IAuthenticationRequest): Promise<IAuthenticationResponse | null> => {
      let data: IAuthenticationResponse | null = null;

      var response = await fetch(
        `${process.env.REACT_APP_API_URL}userauthentication/authenticateuser`,
        {
          method: 'POST',
          mode: 'cors',
          body: JSON.stringify(requestModel),
          headers: {
            'Content-Type': 'application/json',
          },
        },
      );

      if (response.ok) {
        data = await response.json();
      }

      if (data == null || !data.result) {
        return null;
      }

      setAuthContextProps((prev) => ({
        ...prev,
        tokenData: {
          accessToken: data?.accessToken ?? null,
          refreshToken: data?.refreshToken ?? null,
        },
      }));

      return data;
    },
    [],
  );

  const loadCurrentUser = React.useCallback(async (tokenData: ITokenResponse) => {
    let responseData: IUserData | null = null;

    if (tokenData.accessToken == null) {
      console.warn('No token found, cannot load current user data');
      return;
    }

    const headers = new Headers();
    headers.append('Authorization', `Bearer ${tokenData.accessToken}`);
    headers.append('Content-Type', 'application/json');

    const response = await fetch(`${process.env.REACT_APP_API_URL}userservice/getcurrentuser`, {
      method: 'GET',
      mode: 'cors',
      headers: headers,
    });

    if (response.ok) {
      responseData = await response.json();
    }

    if (responseData == null) {
      return;
    }

    window.localStorage.setItem(tokenStorageKey, JSON.stringify(tokenData));
    window.localStorage.setItem(userDataStorageKey, JSON.stringify(responseData));

    setAuthContextProps((prev) => ({
      ...prev,
      isAuthenticated: true,
      tokenData: tokenData,
      userData: responseData,
    }));
  }, []);

  const onLogin = React.useCallback(
    async (authData: IAuthenticationRequest) => {
      const response = await sendAuthenticationRequest(authData);
      if (response?.result) {
        await loadCurrentUser({
          accessToken: response.accessToken,
          refreshToken: response.refreshToken,
        });

        console.log('Login successful');
      }
    },
    [sendAuthenticationRequest, loadCurrentUser],
  );

  const onLogout = React.useCallback(() => {
    window.localStorage.removeItem(tokenStorageKey);

    setAuthContextProps((prev) => ({
      ...prev,
      isAuthenticated: false,
      tokenData: {
        accessToken: null,
        refreshToken: null,
      },
    }));
  }, []);

  const getTokens = React.useCallback((): ITokenResponse | null => {
    if (authContextProps.tokenData.accessToken == null) {
      return null;
    }
    return authContextProps.tokenData;
  }, [authContextProps.tokenData]);

  React.useEffect(() => {
    const tokenDataString = window.localStorage.getItem(tokenStorageKey);

    const userDataString = window.localStorage.getItem(userDataStorageKey);

    if (tokenDataString == null || userDataString == null) {
      return;
    }

    const tokenData = JSON.parse(tokenDataString) as ITokenResponse;
    const userData = JSON.parse(userDataString) as IUserData;

    setAuthContextProps((prev) => ({
      ...prev,
      isAuthenticated: true,
      userData: userData,
      tokenData: tokenData,
    }));
  }, []);

  return (
    <AuthContext.Provider
      value={{
        isAuthenticated: authContextProps?.isAuthenticated ?? false,
        userData: authContextProps?.userData ?? null,
        onLogin,
        onLogout,
        getTokens,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export default AuthContextProvider;
