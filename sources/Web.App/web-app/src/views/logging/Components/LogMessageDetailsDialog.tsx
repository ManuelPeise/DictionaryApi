import { Dialog, DialogActions, DialogContent, Grid, Typography } from '@mui/material';
import React from 'react';
import { ILogMessage } from '../Models/ILogMessage';
import ActionButton from '../../../components/input/ActionButton';

export interface ILogMessageDetailsDialogProps {
  open: boolean;
  logMessage: ILogMessage | null;
}

interface IProps extends ILogMessageDetailsDialogProps {
  onClose: () => void;
}

const LogMessageDetailsDialog: React.FC<IProps> = (props) => {
  const { open, logMessage, onClose } = props;

  return (
    <Dialog open={open} maxWidth="md" onClose={onClose}>
      <DialogContent>
        <Grid container direction="column" spacing={2}>
          <Grid size={12}>
            <Typography variant="h5">{logMessage?.message}</Typography>
          </Grid>
          <Grid size={12}>
            <Typography variant="body1">Exception Message</Typography>
          </Grid>
          <Grid size={12}>
            <Typography variant="body2">{logMessage?.exeptionMessage}</Typography>
          </Grid>
          <Grid size={12}>
            <Typography variant="body1">Stack Trace</Typography>
          </Grid>
          <Grid size={12}>
            <Typography variant="body2">{logMessage?.stackTrace}</Typography>
          </Grid>
        </Grid>
      </DialogContent>
      <DialogActions>
        <ActionButton label="OK" onClick={onClose} />
      </DialogActions>
    </Dialog>
  );
};

export default LogMessageDetailsDialog;
