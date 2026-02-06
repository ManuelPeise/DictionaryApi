import React from 'react';
import { FormLabel, Grid, LinearProgress } from '@mui/material';
import TextInput from '../../components/input/TextInput';
import { IAuthenticationRequest } from '../../lib/interfaces/IAuthenticationRequest';
import Card from '../Shared/Card';
import ActionButton from '../../components/input/ActionButton';
import { useAuth } from '../../hooks/useAuth';

const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

const LoginPage: React.FC = () => {
  const { onLogin } = useAuth();

  const [isLoading, setIsLoading] = React.useState(false);
  const [authData, setAuthData] = React.useState<IAuthenticationRequest>({
    email: 'admin.user@app.com',
    password: '',
  });

  const loginDisabled =
    !authData.email ||
    !emailRegex.test(authData.email) ||
    !authData.password ||
    authData.password === '' ||
    authData.password.length < 8;

  const handleLogin = React.useCallback(async () => {
    setIsLoading(true);
    await onLogin(authData);
    setIsLoading(false);
  }, [authData, onLogin]);

  return (
    <Grid justifyContent="center" alignItems="center" style={{ display: 'flex', height: '100vh' }}>
      {isLoading && (
        <LinearProgress
          style={{
            position: 'absolute',
            top: 0,
            left: 0,
            width: '100%',
            backgroundColor: '#e29521',
          }}
        />
      )}
      <Card
        padding="50px"
        margin="50px auto"
        boxShadow="0 4px 8px rgba(0, 0, 0, 0.1)"
        borderRadius="8px"
      >
        <Grid container justifyContent="end" spacing={5} style={{ marginBottom: '20px' }}>
          <FormLabel
            style={{
              width: '100%',
              textAlign: 'center',
              fontSize: '2.5rem',
              fontWeight: 'bold',
              fontStyle: 'italic',
              color: '#3f51b5',
            }}
          >
            User Login
          </FormLabel>
          <TextInput
            value={authData.email}
            onChange={(value) => setAuthData({ ...authData, email: value })}
            label="Email"
            placeholder="Enter email address"
            variant="standard"
          />
          <TextInput
            value={authData.password}
            onChange={(value) => setAuthData({ ...authData, password: value })}
            label="Password"
            type="password"
            placeholder="Enter password"
            variant="standard"
          />
          <ActionButton label="Login" disabled={loginDisabled} onClick={handleLogin} />
        </Grid>
      </Card>
    </Grid>
  );
};

export default LoginPage;
