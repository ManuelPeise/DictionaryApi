import { LogMessageTypeEnum } from '../../../lib/enums/LogMessageTypeEnum';

export interface ILogMessage {
  id: number;
  message: string;
  exeptionMessage: string;
  stackTrace: string;
  module: string;
  logMessageType: LogMessageTypeEnum;
  timeStamp: string;
}
