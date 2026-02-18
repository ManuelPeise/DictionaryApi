import React from 'react';
import { useAuth } from '../../hooks/useAuth';
import { AppBar, Avatar, FormLabel, Grid, IconButton, Toolbar } from '@mui/material';
import { LogoutOutlined, PersonOutlineOutlined } from '@mui/icons-material';
import { Link } from 'react-router-dom';
import '../../lib/style/global.css';

interface IProps extends React.PropsWithChildren {}

const AppLayout: React.FC<IProps> = (props) => {
  const { children } = props;
  const { userData, onLogout } = useAuth();

  return (
    <Grid size={12}>
      <Grid size={12}>
        <AppBar position="static" sx={{ padding: '0.5rem', backgroundColor: '#000000' }}>
          <Toolbar>
            <Grid
              container
              display="flex"
              justifyContent="space-between"
              alignItems="center"
              width="100%"
            >
              <Grid
                size={2}
                display="flex"
                justifyContent="center"
                alignItems="center"
                padding="0.5rem"
              >
                <Link
                  to="/"
                  className="nav-link"
                  style={{ textDecoration: 'none', color: 'inherit' }}
                >
                  <FormLabel className="nav-link" sx={{ color: '#FFFFFF', fontSize: '1.2rem' }}>
                    MyApp
                  </FormLabel>
                </Link>
              </Grid>
              <Grid container size={8} columnSpacing={4}>
                <Link
                  className="nav-link"
                  to="/vocabulary-validation"
                  style={{ textDecoration: 'none', color: 'inherit' }}
                >
                  <FormLabel className="nav-link" sx={{ color: '#FFFFFF', fontSize: '1.2rem' }}>
                    Vokabelvalidierung
                  </FormLabel>
                </Link>
                <Link
                  className="nav-link"
                  to="/messagelog"
                  style={{ textDecoration: 'none', color: 'inherit' }}
                >
                  <FormLabel className="nav-link" sx={{ color: '#FFFFFF', fontSize: '1.2rem' }}>
                    Nachrichtenprotokoll
                  </FormLabel>
                </Link>
              </Grid>
              <Grid
                size={2}
                display="flex"
                gap={1}
                justifyContent="end"
                alignItems="center"
                padding="0.5rem"
              >
                <Avatar
                  component={PersonOutlineOutlined}
                  sx={{ color: 'primary', bgcolor: '#000000' }}
                ></Avatar>
                {userData && <div>{userData.emailAddress}</div>}
                <IconButton onClick={onLogout} color="primary">
                  <LogoutOutlined className="nav-icon" color="primary" />
                </IconButton>
              </Grid>
            </Grid>
          </Toolbar>
        </AppBar>
      </Grid>
      <Grid size={12} padding={'.5rem'}>
        {children}
      </Grid>
    </Grid>
  );
};

export default AppLayout;
