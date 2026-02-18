import React from 'react';
import CircularProgress from '@mui/material/CircularProgress';
import Box from '@mui/material/Box';

interface LoadingSpinnerProps {
  size?: number;
  thickness?: number;
  color?: 'primary' | 'secondary' | 'inherit' | 'success' | 'error' | 'info' | 'warning';
  message?: string;
}

const overlayStyle: React.CSSProperties = {
  position: 'fixed',
  top: 0,
  left: 0,
  width: '100vw',
  height: '100vh',
  background: 'rgba(0,0,0,0.5)', // darker overlay
  zIndex: 1300,
  display: 'flex',
  flexDirection: 'column',
  alignItems: 'center',
  justifyContent: 'center',
};

const LoadingSpinner: React.FC<LoadingSpinnerProps> = ({
  size = 40,
  thickness = 4,
  color = 'primary',
  message,
}) => (
  <div style={overlayStyle}>
    <CircularProgress size={size} thickness={thickness} color={color} />
    {message && (
      <Box mt={2}>
        <span>{message}</span>
      </Box>
    )}
  </div>
);

export default LoadingSpinner;
