import { AddRounded } from '@mui/icons-material';
import { Container, Grid, IconButton, Tooltip, Typography } from '@mui/material';
import React from 'react';
import AddVocabularyDialog from './AddVocabularyDialog';

interface IProps {}

const VocabularyValicationPage: React.FC<IProps> = () => {
  const [dialogOpen, setDialogOpen] = React.useState(false);

  const handleCloseDialog = React.useCallback(() => {
    setDialogOpen(false);
  }, []);

  const handleOpenDialog = React.useCallback(() => {
    setDialogOpen(true);
  }, []);

  return (
    <Container>
      <Grid container rowSpacing={4}>
        <Grid size={12}>
          <Typography variant="h5">Vocabulary Validation</Typography>
        </Grid>
        <Grid size={12} display="flex" justifyContent="flex-end" alignItems="center">
          <IconButton
            color="primary"
            aria-label="validate vocabulary"
            component="span"
            onClick={handleOpenDialog}
          >
            <Tooltip title="Add Vocabulary">
              <AddRounded className="icon" />
            </Tooltip>
          </IconButton>
        </Grid>
      </Grid>
      <AddVocabularyDialog open={dialogOpen} onClose={handleCloseDialog} />
    </Container>
  );
};

export default VocabularyValicationPage;
