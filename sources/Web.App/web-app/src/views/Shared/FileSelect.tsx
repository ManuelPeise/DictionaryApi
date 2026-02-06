import { FileUpload } from '@mui/icons-material';
import { Grid, IconButton, Tooltip } from '@mui/material';
import React from 'react';

interface IProps {
  accept?: '.csv' | '.json';
  multiple?: boolean;
  onSelectedFilesChanged: (files: File[]) => void;
}

const FileSelect: React.FC<IProps> = (props) => {
  const { accept, multiple, onSelectedFilesChanged } = props;
  const fileInputRef = React.useRef<HTMLInputElement>(null);

  const handleFileSelected = React.useCallback(
    (event: React.ChangeEvent<HTMLInputElement>) => {
      const files = event.target.files;

      if (!files || files.length === 0) {
        return;
      }

      onSelectedFilesChanged(multiple ? Array.from(files) : [files[0]]);
    },
    [multiple, onSelectedFilesChanged],
  );

  return (
    <Grid size={12} display="flex" justifyContent="end" alignItems="center">
      <input
        type="file"
        ref={fileInputRef}
        hidden
        onChange={handleFileSelected}
        accept={accept}
        multiple={multiple ?? false}
      />
      <IconButton onClick={() => fileInputRef.current?.click()}>
        <Tooltip title="Select file">
          <FileUpload />
        </Tooltip>
      </IconButton>
    </Grid>
  );
};

export default FileSelect;
