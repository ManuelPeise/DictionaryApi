import { Dialog, DialogActions, DialogContent, DialogTitle, Grid, Typography } from '@mui/material';
import React from 'react';
import { IVocabularyFileUpload } from '../models/IVocabularyFileUpload';
import VocabularyImportDialogContent from './VocabularyImportDialogContent';
import ActionButton from '../../../components/input/ActionButton';

interface IProps {
  isLoading: boolean;
  isImportVocabularyDialogOpen: boolean;
  toggleImportVocabularyDialog: (open: boolean) => void;
  uploadVocabularyFile: (model: IVocabularyFileUpload) => Promise<void>;
}

const AddVocabularyDialog: React.FC<IProps> = (props) => {
  const {
    isLoading,
    isImportVocabularyDialogOpen,
    toggleImportVocabularyDialog,
    uploadVocabularyFile,
  } = props;

  return (
    <Dialog
      open={isImportVocabularyDialogOpen}
      onClose={toggleImportVocabularyDialog.bind(null, false)}
      maxWidth="md"
      keepMounted
      slotProps={{ paper: { sx: { padding: '1rem' } } }}
    >
      <DialogTitle>
        <Grid size={12}>
          <Typography variant="h5" align="center">
            Vokabeln importieren
          </Typography>
        </Grid>
      </DialogTitle>
      <DialogContent sx={{ padding: '1.5rem' }}>
        <VocabularyImportDialogContent isLoading={isLoading} uploadFile={uploadVocabularyFile} />
      </DialogContent>
      <DialogActions>
        <ActionButton
          disabled={isLoading}
          label="Schließen"
          onClick={toggleImportVocabularyDialog.bind(null, false)}
        />
      </DialogActions>
    </Dialog>
  );
};

export default AddVocabularyDialog;
