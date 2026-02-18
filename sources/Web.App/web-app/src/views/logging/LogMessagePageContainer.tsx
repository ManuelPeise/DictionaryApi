import {
  Checkbox,
  Grid,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Typography,
} from '@mui/material';
import React from 'react';
import { useComponentInitialization } from '../../hooks/useComponentInitialization';
import LoadingSpinner from '../../components/shared/LoadingSpinner';
import { ApiRequestHandlerResult, useApiRequestHandler } from '../../hooks/useApiRequestHandler';
import { ILogMessage } from './Models/ILogMessage';
import { ILogMessagePageState } from './Models/ILogmessagePageState';
import { ILogMessageTableColumnProps } from './Models/ILogMessageTableColumnProps';
import { LogMessageTypeEnum } from '../../lib/enums/LogMessageTypeEnum';
import { ILogMessageFilter } from './Models/ILogMessafeFilter';
import { IDropdownItem } from '../../lib/interfaces/IDropdownItem';
import Dropdown from '../../components/input/Dropdown';
import ActionButton from '../../components/input/ActionButton';
import DeleteLogMessagesDialog from './Components/DeleteLogMessagesDialog';
import LogMessageDetailsDialog from './Components/LogMessageDetailsDialog';

interface IProps {
  logMessages: ILogMessage[];
  deleteLogmessages: (ids: number[]) => Promise<ILogMessage[]>;
}

const initializeAsync = async (api: ApiRequestHandlerResult): Promise<IProps> => {
  const fetchLogMessages = async (): Promise<ILogMessage[]> => {
    return await api.sendGetRequest<ILogMessage[]>(
      `${process.env.REACT_APP_API_URL}logmessage/getlogmessages`,
    );
  };

  const deleteLogmessages = async (ids: number[]): Promise<ILogMessage[]> => {
    return await api.sendPostRequest<ILogMessage[]>(
      `${process.env.REACT_APP_API_URL}logmessage/deletemessages`,
      ids,
    );
  };
  const [logMessages] = await Promise.all([fetchLogMessages()]);

  return {
    logMessages,
    deleteLogmessages,
  };
};

const LogMessagePageContainer: React.FC = () => {
  const api = useApiRequestHandler();

  const { isInitialized, props } = useComponentInitialization<IProps>(
    initializeAsync.bind(null, api),
  );

  if (!isInitialized) {
    return <LoadingSpinner />;
  }

  return <LogMessagePage {...props} />;
};

const LogMessagePage: React.FC<IProps> = (props) => {
  const { logMessages, deleteLogmessages } = props;

  const [pageState, setPageState] = React.useState<ILogMessagePageState>({
    isLoading: false,
    selectedMessageIds: [],
    logMessages,
    deleteMessagesDialogOpen: false,
    logMessageDetailsDialogProps: {
      open: false,
      logMessage: null,
    },
  });

  const [filter, setFilter] = React.useState<ILogMessageFilter>({
    messageType: 0,
    moduleId: 0,
  });

  const handlePageStateChange = React.useCallback((newState: Partial<ILogMessagePageState>) => {
    setPageState((prevState) => ({ ...prevState, ...newState }));
  }, []);

  const handleSelectAll = React.useCallback(() => {
    handlePageStateChange({
      selectedMessageIds:
        pageState.selectedMessageIds.length === pageState.logMessages.length
          ? []
          : pageState.logMessages.map((msg) => msg.id),
    });
  }, [pageState, handlePageStateChange]);

  const handleSelectMessage = React.useCallback(
    (id: number) => {
      const isSelected = pageState.selectedMessageIds.includes(id);

      handlePageStateChange({
        selectedMessageIds: isSelected
          ? pageState.selectedMessageIds.filter((msgId) => msgId !== id)
          : [...pageState.selectedMessageIds, id],
      });
    },
    [pageState, handlePageStateChange],
  );

  const getTypeLabel = React.useCallback((type: LogMessageTypeEnum): string => {
    switch (type) {
      case LogMessageTypeEnum.Info:
        return 'Info';
      case LogMessageTypeEnum.Warning:
        return 'Warning';
      case LogMessageTypeEnum.Error:
        return 'Error';
      default:
        return '';
    }
  }, []);

  const handleDeleteMessages = React.useCallback(async () => {
    if (pageState.selectedMessageIds.length === 0) {
      return;
    }
    handlePageStateChange({ isLoading: true });
    const response = await deleteLogmessages(pageState.selectedMessageIds);
    handlePageStateChange({
      selectedMessageIds: [],
      logMessages: response,
      isLoading: false,
      deleteMessagesDialogOpen: false,
    });
  }, [pageState.selectedMessageIds, deleteLogmessages, handlePageStateChange]);

  const handleFilterChange = React.useCallback((newFilter: Partial<ILogMessageFilter>) => {
    setFilter((prevFilter) => ({ ...prevFilter, ...newFilter }));
  }, []);

  const handleRowClick = React.useCallback(
    (id: number) => {
      const message = pageState.logMessages.find((msg) => msg.id === id);
      if ((message && message.exeptionMessage) || (message && message.stackTrace)) {
        handlePageStateChange({
          logMessageDetailsDialogProps: {
            open: true,
            logMessage: message,
          },
        });
      }
    },
    [pageState.logMessages, handlePageStateChange],
  );

  const handleCloseDetailsDialog = React.useCallback(() => {
    handlePageStateChange({
      logMessageDetailsDialogProps: { open: false, logMessage: null },
    });
  }, [handlePageStateChange]);

  const columnDefinition = React.useMemo((): ILogMessageTableColumnProps[] => {
    return [
      { key: 'id', width: 40, headerLabel: 'Id', align: 'center', componentType: 'checkbox' },
      {
        key: 'timeStamp',
        width: 100,
        headerLabel: 'Timestamp',
        align: 'left',
        componentType: 'label',
      },
      {
        key: 'logMessageType',
        width: 100,
        headerLabel: 'Type',
        align: 'left',
        componentType: 'label',
      },
      { key: 'module', width: 80, headerLabel: 'Module', align: 'left', componentType: 'label' },
      { key: 'message', width: 120, headerLabel: 'Message', align: 'left', componentType: 'label' },
      {
        key: 'exeptionMessage',
        width: 120,
        headerLabel: 'Exception Message',
        align: 'left',
        componentType: 'label',
      },
      {
        key: 'stackTrace',
        width: 120,
        headerLabel: 'Stack Trace',
        align: 'left',
        componentType: 'label',
      },
    ];
  }, []);

  const messageTypeOptions = React.useMemo((): IDropdownItem[] => {
    return [
      { id: 0, label: 'All Types' },
      { id: 1, label: 'Info' },
      { id: 2, label: 'Warning' },
      { id: 3, label: 'Error' },
    ];
  }, []);

  const moduleOptions = React.useMemo((): IDropdownItem[] => {
    const modules: string[] = ['All Modules'];
    pageState.logMessages.forEach((msg) => {
      if (!modules.includes(msg.module)) {
        modules.push(msg.module);
      }
    });
    return modules.map((module, index) => ({ id: index, label: module }));
  }, [pageState.logMessages]);

  const filteredLogMessages = React.useMemo((): ILogMessage[] => {
    return pageState.logMessages.filter((msg) => {
      const matchesType =
        filter.messageType === 0 ||
        msg.logMessageType === ((filter.messageType - 1) as LogMessageTypeEnum);
      const matchesModule =
        filter.moduleId === 0 ||
        msg.module === moduleOptions.find((option) => option.id === filter.moduleId)?.label;
      return matchesType && matchesModule;
    });
  }, [pageState.logMessages, filter, moduleOptions]);

  return (
    <Grid container size={12} rowSpacing={2} padding={6}>
      <Grid
        size={12}
        display="flex"
        justifyContent="space-between"
        alignItems="center"
        columnSpacing={2}
      >
        <Typography variant="h5" sx={{ paddingLeft: '2rem' }}>
          Nachichtenprotokoll
        </Typography>
      </Grid>
      <Grid size={12} mt={4}>
        <Paper elevation={4} sx={{ padding: 2, width: '100%' }}>
          <Grid
            container
            size={12}
            padding={2}
            spacing={8}
            justifyContent="space-between"
            alignItems="center"
          >
            <Grid
              size={4}
              display="flex"
              justifyContent="flex-end"
              columnSpacing={2}
              alignItems="center"
            >
              <Dropdown
                fullwidth
                value={filter.messageType}
                items={messageTypeOptions}
                onChange={(newValue) => handleFilterChange({ messageType: newValue })}
              />
            </Grid>
            <Grid
              display="flex"
              size={4}
              justifyContent="flex-end"
              columnSpacing={2}
              alignItems="center"
            >
              <Dropdown
                fullwidth
                value={filter.moduleId}
                items={moduleOptions}
                onChange={(newValue) => handleFilterChange({ moduleId: newValue })}
              />
            </Grid>
            <Grid
              size={4}
              display="flex"
              justifyContent="flex-end"
              columnSpacing={2}
              alignItems="center"
            >
              <ActionButton
                disabled={pageState.selectedMessageIds.length === 0}
                label="Löschen"
                onClick={() => handlePageStateChange({ deleteMessagesDialogOpen: true })}
              />
            </Grid>
          </Grid>
        </Paper>
      </Grid>
      <Grid size={12} mt={2}>
        <Paper elevation={4} sx={{ padding: 2, width: '100%' }}>
          <TableContainer sx={{ maxWidth: '100%', height: '500px' }}>
            <Table sx={{ minWidth: '100%', maxWidth: '100%' }}>
              <TableHead>
                <TableRow>
                  {columnDefinition.map((col, index) => (
                    <TableCell key={col.key} align={col.align} style={{ maxWidth: col.width }}>
                      {index === 0 ? (
                        <Checkbox
                          indeterminate={
                            pageState.selectedMessageIds.length > 0 &&
                            pageState.selectedMessageIds.length < pageState.logMessages.length
                          }
                          checked={
                            pageState.selectedMessageIds.length === pageState.logMessages.length
                          }
                          onChange={handleSelectAll}
                        />
                      ) : (
                        <Typography variant="subtitle2">{col.headerLabel}</Typography>
                      )}
                    </TableCell>
                  ))}
                </TableRow>
              </TableHead>
              <TableBody>
                {filteredLogMessages.map((msg, rowIndex) => (
                  <TableRow
                    key={`${msg.id}-${rowIndex}`}
                    sx={{
                      maxHeight: 10,
                      '&:hover': msg.exeptionMessage || msg.stackTrace ? { cursor: 'pointer' } : {},
                    }}
                    hover
                    onClick={handleRowClick.bind(null, msg.id)}
                  >
                    {columnDefinition.map((col, colIndex) => (
                      <TableCell
                        key={`${col.key}-${colIndex}`}
                        align={col.align}
                        style={{ maxWidth: col.width, maxHeight: 10, overflow: 'hidden' }}
                      >
                        {colIndex === 0 ? (
                          <Checkbox
                            checked={pageState.selectedMessageIds.includes(msg.id)}
                            onChange={handleSelectMessage.bind(null, msg.id)}
                          />
                        ) : (
                          <Typography
                            variant="body2"
                            sx={{
                              //   textOverflow: 'ellipsis',
                              overflow: 'hidden',
                              whiteSpace: 'nowrap',
                            }}
                          >
                            {col.key === 'logMessageType'
                              ? getTypeLabel(
                                  msg[col.key as keyof ILogMessage] as LogMessageTypeEnum,
                                )
                              : msg[col.key as keyof ILogMessage]}
                          </Typography>
                        )}
                      </TableCell>
                    ))}
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </TableContainer>
        </Paper>
        <DeleteLogMessagesDialog
          open={pageState.deleteMessagesDialogOpen}
          actionCallback={handleDeleteMessages}
          cancelCallback={() => handlePageStateChange({ deleteMessagesDialogOpen: false })}
        />
        <LogMessageDetailsDialog
          onClose={handleCloseDetailsDialog}
          open={pageState.logMessageDetailsDialogProps.open}
          logMessage={pageState.logMessageDetailsDialogProps.logMessage}
        />
      </Grid>
    </Grid>
  );
};
export default LogMessagePageContainer;
