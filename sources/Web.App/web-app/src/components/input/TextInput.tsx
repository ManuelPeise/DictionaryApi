import { TextField } from '@mui/material';
import React from 'react';

interface IProps {
  label: string;
  placeholder?: string;
  value: string;
  disabled?: boolean;
  type?: 'password' | 'text';
  variant?: 'outlined' | 'filled' | 'standard';
  onChange: (value: string) => void;
}

const TextInput: React.FC<IProps> = (props) => {
  return (
    <TextField
      type={props?.type ?? 'text'}
      label={props.label}
      placeholder={props.placeholder}
      value={props.value}
      disabled={props.disabled}
      onChange={(e) => props.onChange(e.target.value)}
      fullWidth
      variant={props.variant || 'outlined'}
    />
  );
};

export default TextInput;
