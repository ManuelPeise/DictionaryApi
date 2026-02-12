import { AddRounded } from '@mui/icons-material';
import {
  Grid,
  IconButton,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Tooltip,
  Typography,
} from '@mui/material';
import React from 'react';
import AddVocabularyDialog from './AddVocabularyDialog';
import { useApi } from '../../hooks/useApi';
import {
  IVocabularyExportModel,
  IVocabularyCategoryExportModel,
  IVocabularyValidationPageModel,
} from './models/IVocabularyValidationPageModel';
import { IDropdownItem } from '../../lib/interfaces/IDropdownItem';

interface IProps {
  pageModel: IVocabularyValidationPageModel;
  topicDropDownItems: IDropdownItem[];
  rebindData: () => Promise<void>;
}

const VocabularyValidationPageContainer: React.FC = () => {
  const vocabularyValidationApi = useApi.useStatefullApi<IVocabularyValidationPageModel>({
    requestUrl: `${process.env.REACT_APP_API_URL}vocabularyimport/getvocabularyvalidationpagemodel`,
    method: 'GET',
  });

  const topicDropdownItems = React.useMemo<IDropdownItem[]>(() => {
    const items =
      vocabularyValidationApi.data == null || vocabularyValidationApi.data.categories?.length === 0
        ? []
        : vocabularyValidationApi.data.categories.map((category) => ({
            id: category.id,
            label: category.name,
          }));

    return [...items];
  }, [vocabularyValidationApi.data]);

  const rebindData = React.useCallback(async () => {
    await vocabularyValidationApi.rebindData();
  }, [vocabularyValidationApi]);

  if (vocabularyValidationApi.isLoading || vocabularyValidationApi.data === null) {
    return <Typography variant="h6">Loading...</Typography>;
  }

  return (
    <VocabularyValidationPage
      pageModel={vocabularyValidationApi.data}
      topicDropDownItems={topicDropdownItems}
      rebindData={rebindData}
    />
  );
};

const VocabularyValidationPage: React.FC<IProps> = (props) => {
  const { pageModel, rebindData } = props;
  const [dialogOpen, setDialogOpen] = React.useState(false);

  const [selectedCategory, setSelectedCategory] =
    React.useState<IVocabularyCategoryExportModel | null>(pageModel?.categories[0] || null);

  const rows = React.useMemo((): IVocabularyExportModel[] => {
    if (selectedCategory == null) {
      return [];
    }

    const vocabularyRows: IVocabularyExportModel[] = [];

    selectedCategory.vocabularyGroups.forEach((grp) => {
      return grp.vocabularies.forEach((vocab) => {
        vocabularyRows.push(vocab);
      });
    });

    return vocabularyRows;
  }, [selectedCategory]);

  const handleCloseDialog = React.useCallback(async () => {
    setDialogOpen(false);
    await rebindData();
  }, [rebindData]);

  const handleOpenDialog = React.useCallback(() => {
    setDialogOpen(true);
  }, []);

  return (
    <Grid size={12} rowSpacing={4} padding={6}>
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
      <Grid container size={12}>
        <TableContainer component={Grid} size={12}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Id</TableCell>
                <TableCell>Part of speech</TableCell>
                <TableCell>Word</TableCell>
                <TableCell>IPA</TableCell>
                <TableCell>Article</TableCell>
                <TableCell>Language</TableCell>
                <TableCell>Sentence</TableCell>
                <TableCell>Last Update By</TableCell>
                <TableCell>Last Update At</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {rows.map((row) => (
                <TableRow key={row.id}>
                  <TableCell>{row.id}</TableCell>
                  <TableCell>{row.partOfSpeech}</TableCell>
                  <TableCell>{row.word}</TableCell>
                  <TableCell>{row.ipa}</TableCell>
                  <TableCell>{row.article}</TableCell>
                  <TableCell>{row.language}</TableCell>
                  <TableCell>{row.sentence}</TableCell>
                  <TableCell>{row.lastUpdateBy}</TableCell>
                  <TableCell>{row.lastUpdateAt}</TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      </Grid>
      <AddVocabularyDialog open={dialogOpen} onClose={handleCloseDialog} />
    </Grid>
  );
};

export default React.memo(VocabularyValidationPageContainer);
