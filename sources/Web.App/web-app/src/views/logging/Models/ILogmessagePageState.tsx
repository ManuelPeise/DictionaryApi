import { ILogMessageDetailsDialogProps } from '../Components/LogMessageDetailsDialog';
import { ILogMessage } from './ILogMessage';

export interface ILogMessagePageState {
  isLoading: boolean;
  selectedMessageIds: number[];
  logMessages: ILogMessage[];
  deleteMessagesDialogOpen: boolean;
  logMessageDetailsDialogProps: ILogMessageDetailsDialogProps;
}
