import { ILogMessage } from './ILogMessage';

export interface ILogMessageTableColumnProps {
  key: keyof ILogMessage | 'actions';
  width: number;
  headerLabel: string;
  align?: 'left' | 'right' | 'center';
  componentType?: 'checkbox' | 'label' | 'icon';
}
