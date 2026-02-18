import { CheckCircleOutline, HighlightOffOutlined } from '@mui/icons-material';
import React from 'react';

export enum StatefulIcons {
  verified = 0,
  notVerified = 1,
}

interface IProps {
  icon: StatefulIcons;
  size: number;
  color: 'disabled' | 'primary' | 'secondary' | 'action' | 'error' | 'info' | 'success';
}
const StatefulIcon: React.FC<IProps> = (props) => {
  const { icon, size = 24, color } = props;

  const IconComponent = React.useMemo(() => {
    switch (icon) {
      case StatefulIcons.verified:
        return CheckCircleOutline;
      case StatefulIcons.notVerified:
        return HighlightOffOutlined;
      default:
        return null;
    }
  }, [icon]);

  if (!IconComponent) {
    return null;
  }

  return <IconComponent sx={{ width: size, height: size }} color={color} />;
};

export default StatefulIcon;
