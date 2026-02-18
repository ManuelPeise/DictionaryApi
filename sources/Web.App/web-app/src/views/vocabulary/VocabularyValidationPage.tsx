import { AddRounded } from '@mui/icons-material';
import { Grid, IconButton, Tooltip, Typography } from '@mui/material';
import React from 'react';
import VocabularyGroupList from './components/VocabularyGroupList';
import VocabularyDetails from './components/VocabularyDetails';
import LoadingSpinner from '../../components/shared/LoadingSpinner';
import { useApiRequestHandler } from '../../hooks/useApiRequestHandler';
import { IVocabularyValidationModel } from './models/IVocabularyValidationModel';
import { IDropdownItem } from '../../lib/interfaces/IDropdownItem';
import { IVocabularyExportModel } from './models/IVocabularyExportModel';
import { groupBy, IGroupByResult } from '../../lib/utils';
import { IVocabularyFileUpload } from './models/IVocabularyFileUpload';
import AddVocabularyDialog from './components/AddVocabularyDialog';

interface IProps {
  isLoading: boolean;
  categoryDropdownItems: IDropdownItem[];
  vocabularyCategoryGroups: IGroupByResult<IVocabularyExportModel>[];
  uploadVocabularyFile: (model: IVocabularyFileUpload) => Promise<void>;
  handleSaveVocabularies: (vocabularies: IVocabularyExportModel[]) => Promise<void>;
}

const loadInitialModel = async (
  callback: () => Promise<IVocabularyValidationModel>,
  stateSetter: (model: IVocabularyValidationModel | null) => void,
) => {
  const response = await callback();

  if (response) {
    stateSetter(response);
  }
};

const VocabularyValidationPageContainer: React.FC = () => {
  const [isLoading, setIsLoading] = React.useState(false);
  const [initialData, setInitialData] = React.useState<IVocabularyValidationModel | null>(null);

  const vovabularyApi = useApiRequestHandler();

  const fetchInitialDataCallback = React.useCallback(async () => {
    const response = await vovabularyApi.sendGetRequest<IVocabularyValidationModel>(
      `${process.env.REACT_APP_API_URL}vocabularyvalidation/getinitialdata`,
    );

    return response;
  }, [vovabularyApi]);

  const uploadVocabularyFile = React.useCallback(
    async (model: IVocabularyFileUpload) => {
      await vovabularyApi
        .sendPostRequest<void>(
          `${process.env.REACT_APP_API_URL}vocabularyimport/importvocabularyfile`,
          JSON.stringify(model),
        )
        .then(async () => {
          await loadInitialModel(fetchInitialDataCallback, setInitialData);
        });
    },
    [fetchInitialDataCallback, vovabularyApi],
  );

  const handleSaveVocabularies = React.useCallback(
    async (vocabularies: IVocabularyExportModel[]): Promise<void> => {
      if (initialData == null) return;

      const response = await vovabularyApi.sendPostRequest<IVocabularyExportModel[]>(
        `${process.env.REACT_APP_API_URL}vocabularyvalidation/updatevalidatedvocabularies`,
        vocabularies,
      );

      const newState: IVocabularyValidationModel = {
        ...initialData,
        vocabularies: initialData.vocabularies.map((v) => {
          const updatedVocabulary = response?.find((rv) => rv.id === v.id);
          if (updatedVocabulary) {
            return updatedVocabulary;
          }
          return v;
        }),
      };

      setInitialData(newState);
    },
    [initialData, vovabularyApi],
  );

  const vocabularyCategoryGroupListItems = React.useMemo(() => {
    const groups = groupBy(initialData?.vocabularies || [], (v) => v.vocabularyCategoryName);
    return groups;
  }, [initialData?.vocabularies]);

  React.useEffect(() => {
    setIsLoading(true);
    loadInitialModel(fetchInitialDataCallback, setInitialData).then(() => setIsLoading(false));
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  if (isLoading || !initialData) {
    return <LoadingSpinner />;
  }

  return (
    <VocabularyValidationPage
      isLoading={isLoading}
      categoryDropdownItems={initialData.categoryDropdownItems}
      vocabularyCategoryGroups={vocabularyCategoryGroupListItems}
      uploadVocabularyFile={uploadVocabularyFile}
      handleSaveVocabularies={handleSaveVocabularies}
    />
  );
};

const VocabularyValidationPage: React.FC<IProps> = (props) => {
  const {
    isLoading,
    categoryDropdownItems,
    vocabularyCategoryGroups,
    uploadVocabularyFile,
    handleSaveVocabularies,
  } = props;

  const [selectedCategory, setSelectedCategory] = React.useState<IDropdownItem | null>(null);
  const [importVocabularyDialogOpen, setImportVocabularyDialogOpen] = React.useState(false);
  const [selectedVocabularyGroupGuid, setSelectedVocabularyGroupGuid] = React.useState<
    string | null
  >(null);

  const handleSelectedCategoryChanged = React.useCallback(
    (id: number) => {
      const category = categoryDropdownItems.find((c) => c.id === id);

      setSelectedCategory(category || null);
    },
    [categoryDropdownItems],
  );

  const vocabularyGroups = React.useMemo((): IGroupByResult<IVocabularyExportModel>[] => {
    const selectedGroup = vocabularyCategoryGroups?.find((g) => g.key === selectedCategory?.label);

    if (!selectedGroup) return [];
    return groupBy(
      selectedGroup.items || [],
      (v) => v.vocabularyGroupGuid,
      (v) => v.word,
    );
  }, [selectedCategory, vocabularyCategoryGroups]);

  const selectedVocabularyGroup = React.useMemo(() => {
    const grp = vocabularyGroups.find((g) => g.key === selectedVocabularyGroupGuid) || null;

    grp?.items.sort((a, b) => (a.language < b.language ? -1 : 1));
    return grp;
  }, [selectedVocabularyGroupGuid, vocabularyGroups]);

  return (
    <Grid size={12} rowSpacing={2} padding={6}>
      <Grid
        size={12}
        display="flex"
        justifyContent="space-between"
        alignItems="center"
        columnSpacing={2}
      >
        <Typography variant="h5" sx={{ paddingLeft: '2rem' }}>
          Vokabelvalidierung
        </Typography>
        <IconButton
          color="primary"
          component="span"
          onClick={setImportVocabularyDialogOpen.bind(null, true)}
        >
          <Tooltip title="Vokabeln hinzufügen">
            <AddRounded className="icon" />
          </Tooltip>
        </IconButton>
      </Grid>
      <Grid container size={12} columnSpacing={4} mt={2} height={'700px'}>
        <VocabularyGroupList
          categoryDropdownItems={categoryDropdownItems}
          selectedCategoryId={selectedCategory?.id || null}
          vocabularyGroups={vocabularyGroups}
          selectedVocabularyGroupId={selectedVocabularyGroupGuid}
          handleSelectedCategoryChanged={handleSelectedCategoryChanged}
          handleSelectedVocabularyGroupChanged={setSelectedVocabularyGroupGuid}
        />
        <VocabularyDetails
          isLoading={isLoading}
          group={selectedVocabularyGroup}
          handleSaveVocabulary={handleSaveVocabularies}
        />
      </Grid>
      <AddVocabularyDialog
        isLoading={isLoading}
        isImportVocabularyDialogOpen={importVocabularyDialogOpen}
        toggleImportVocabularyDialog={setImportVocabularyDialogOpen}
        uploadVocabularyFile={uploadVocabularyFile}
      />
      {isLoading && <LoadingSpinner />}
    </Grid>
  );
};

export default React.memo(VocabularyValidationPageContainer);
