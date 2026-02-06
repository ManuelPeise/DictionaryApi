import { Button } from '@mui/material';
import React from 'react';

interface IProps {
  label: string;
  disabled?: boolean;
  color?: 'primary' | 'secondary' | 'success' | 'error' | 'info' | 'warning';
  onClick: () => void | Promise<void>;
}

const ActionButton: React.FC<IProps> = (props) => {
  const { label, disabled, color, onClick } = props;
  return (
    <Button variant="contained" color={color ?? 'primary'} disabled={disabled} onClick={onClick}>
      {label}
    </Button>
  );
};

export default ActionButton;
