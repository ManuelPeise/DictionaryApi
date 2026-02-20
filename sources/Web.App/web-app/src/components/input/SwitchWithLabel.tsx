import { FormLabel, Switch } from '@mui/material';
import React from 'react';

interface IProps {
  label: string;
  value: boolean;
  disabled?: boolean;
  onChange: (value: boolean) => void;
}

const SwitchWithLabel: React.FC<IProps> = (props) => {
  const { label, value, disabled, onChange } = props;

  return (
    <FormLabel
      sx={{ width: '100%', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}
    >
      {label}
      <Switch disabled={disabled} checked={value} onChange={(e) => onChange(e.target.checked)} />
    </FormLabel>
  );
};

export default React.memo(SwitchWithLabel);
