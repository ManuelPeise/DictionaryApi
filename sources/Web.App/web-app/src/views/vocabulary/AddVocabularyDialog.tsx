import { Dialog, DialogActions, DialogContent, DialogTitle } from '@mui/material';
import React from 'react';
import ActionButton from '../../components/input/ActionButton';
import VocabularyImportDialogContent from './VocabularyImportDialogContent';

interface IProps {
  open: boolean;
  onClose: () => void;
}

const AddVocabularyDialog: React.FC<IProps> = (props) => {
  const { open, onClose } = props;

  return (
    <Dialog
      open={open}
      onClose={onClose}
      maxWidth="md"
      keepMounted
      slotProps={{ paper: { sx: { padding: '1rem' } } }}
    >
      <DialogTitle>Add Vocabulary</DialogTitle>
      <DialogContent sx={{ padding: '1.5rem' }}>
        <VocabularyImportDialogContent />
      </DialogContent>
      <DialogActions>
        <ActionButton label="Close" onClick={onClose} />
      </DialogActions>
    </Dialog>
  );
};

export default AddVocabularyDialog;
