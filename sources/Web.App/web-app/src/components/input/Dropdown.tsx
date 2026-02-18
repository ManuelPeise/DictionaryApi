import { MenuItem, Select } from '@mui/material';
import { IDropdownItem } from '../../lib/interfaces/IDropdownItem';
import React from 'react';

interface IProps {
  value: number | null;
  placeholder?: string;
  items: IDropdownItem[];
  minwidth?: string;
  fullwidth?: boolean;
  disabled?: boolean;
  onChange: (value: number) => void;
}

const Dropdown: React.FC<IProps> = (props) => {
  const { value, items, placeholder, disabled, fullwidth, onChange } = props;

  const options = React.useMemo(() => {
    const options =
      items?.map((item) => (
        <MenuItem key={item.id} value={item.id}>
          {item.label}
        </MenuItem>
      )) ?? [];

    return [
      <MenuItem value={0} selected={value === 0} key={0} disabled>
        {placeholder}
      </MenuItem>,
      ...options,
    ];
  }, [items, placeholder, value]);

  return (
    <Select
      size="medium"
      fullWidth={fullwidth}
      sx={{ minWidth: '100%' }}
      value={value}
      variant="standard"
      disabled={disabled}
      onChange={(e) => onChange(Number(e.target.value))}
    >
      {options}
    </Select>
  );
};

export default Dropdown;
