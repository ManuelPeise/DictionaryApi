import { Grid, Paper, Typography } from '@mui/material';
import React from 'react';

const VocabularyContentPlaceholder: React.FC = () => {
  return (
    <Grid size={9} boxSizing="border-box" height="100%">
      <Paper elevation={4} sx={{ padding: 2, height: '100%' }}>
        <Grid
          size={12}
          padding={4}
          display="flex"
          flexDirection="column"
          justifyContent="center"
          alignItems="center"
          height="100%"
        >
          <Typography variant="h6">
            Keine Vokabeln gefunden, bitte wählen Sie eine Kategorie und eine Vokabel aus.
          </Typography>
        </Grid>
      </Paper>
    </Grid>
  );
};

export default VocabularyContentPlaceholder;
