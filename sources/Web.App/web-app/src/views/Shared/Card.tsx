import { Container } from '@mui/material';
import React from 'react';

interface IProps extends React.PropsWithChildren {
  padding?: string;
  margin?: string;
  boxShadow?: string;
  borderRadius?: string;
  maxWidth?: 'lg' | 'md' | 'sm' | 'xl' | 'xs' | false;
}

const Card: React.FC<IProps> = (props) => {
  const { padding, margin, boxShadow, borderRadius, children, maxWidth } = props;

  return (
    <Container
      maxWidth={maxWidth ?? 'md'}
      style={{ margin: margin, padding: padding, boxShadow: boxShadow, borderRadius: borderRadius }}
    >
      {children}
    </Container>
  );
};

export default Card;
