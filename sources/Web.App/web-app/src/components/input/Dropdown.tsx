import { Select } from '@mui/material';
import { IDropdownItem } from '../../lib/interfaces/IDropdownItem';
import React from 'react';

interface IProps {
  value: number;
  placeholder?: string;
  items: IDropdownItem[];
  minwidth?: string;
  fullwidth?: boolean;
  onChange: (value: number) => void;
}

const Dropdown: React.FC<IProps> = (props) => {
  const { value, items, placeholder, minwidth, fullwidth, onChange } = props;

  const options = React.useMemo(() => {
    const options = items.map((item) => (
      <option key={item.id} value={item.id}>
        {item.label}
      </option>
    ));

    return [
      <option value={0} disabled hidden>
        {placeholder}
      </option>,
      ...options,
    ];
  }, [items, placeholder]);

  return (
    <Select
      size="medium"
      fullWidth={fullwidth}
      sx={{ minWidth: minwidth ?? '300px' }}
      value={value}
      variant="standard"
      onChange={(e) => onChange(Number(e.target.value))}
    >
      {options}
    </Select>
  );
};

export default Dropdown;
