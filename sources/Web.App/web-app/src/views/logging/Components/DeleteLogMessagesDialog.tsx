import { Dialog, DialogActions, DialogContentText } from '@mui/material';
import React from 'react';
import ActionButton from '../../../components/input/ActionButton';

interface IProps {
  open: boolean;
  actionCallback: () => Promise<void>;
  cancelCallback: () => void;
}

const DeleteLogMessagesDialog: React.FC<IProps> = (props) => {
  const { open, actionCallback, cancelCallback } = props;

  return (
    <Dialog open={open} onClose={cancelCallback}>
      <DialogContentText sx={{ padding: 4 }}>
        Möchten Sie die ausgewählten Lognachrichten wirklich löschen?
      </DialogContentText>
      <DialogActions sx={{ padding: 2 }}>
        <ActionButton label="Nein" onClick={cancelCallback} />
        <ActionButton label="Ja" onClick={actionCallback} />
      </DialogActions>
    </Dialog>
  );
};

export default DeleteLogMessagesDialog;
