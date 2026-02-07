import { Checkbox, Container, FormLabel, Grid, Typography } from '@mui/material';
import React from 'react';
import FileSelect from '../Shared/FileSelect';
import { IVocabularyFileUpload } from './models/IVocabularyFileUpload';
import TextInput from '../../components/input/TextInput';
import { TranslationEnum } from '../../lib/enums/translationEnum';
import ActionButton from '../../components/input/ActionButton';
import { useApi } from '../../hooks/useApi';

interface IProps {}

const VocabularyImportDialogContent: React.FC<IProps> = () => {
  const [importModel, setImportModel] = React.useState<IVocabularyFileUpload>({
    topic: null,
    sourceLanguage: TranslationEnum.De,
    translations: [],
    file: [],
  });

  const api = useApi.useStatefullApi<IVocabularyFileUpload, void>({
    requestUrl: `${process.env.REACT_APP_API_URL}vocabularyimport/importvocabulary`,
    method: 'POST',
  });

  const languageCheckboxItems = React.useMemo(() => {
    return [
      {
        label: 'English',
        value: TranslationEnum.En,
      },
      {
        label: 'German',
        value: TranslationEnum.De,
      },
      {
        label: 'Danish',
        value: TranslationEnum.Da,
      },
    ];
  }, []);

  const onSelectedFilesChanged = React.useCallback(async (files: File[]) => {
    if (files.length > 0) {
      var buffer = await files[0].arrayBuffer();
      const uint8Array = new Uint8Array(buffer);
      const byteArray = Array.from(uint8Array);

      setImportModel((prev) => ({
        ...prev,
        file: byteArray,
      }));
    }
  }, []);

  const handleTopicChanged = React.useCallback((topic: string) => {
    setImportModel((prev) => ({
      ...prev,
      topic,
    }));
  }, []);

  const handleSourceLanguageChanged = React.useCallback(
    (event: React.ChangeEvent<HTMLInputElement, Element>, checked: boolean) => {
      if (isNaN(Number(event.target.value))) {
        return;
      }
      const valueAsNumber = Number(event.target.value);
      setImportModel((prev) => ({
        ...prev,
        baseLanguage: valueAsNumber as TranslationEnum,
      }));
    },
    [],
  );

  const handleTranslationChanged = React.useCallback(
    (event: React.ChangeEvent<HTMLInputElement, Element>, checked: boolean) => {
      if (isNaN(Number(event.target.value))) {
        return;
      }
      const valueAsNumber = Number(event.target.value);
      const translations = importModel.translations.includes(valueAsNumber)
        ? importModel.translations.filter((t) => t !== valueAsNumber)
        : [...importModel.translations, valueAsNumber];

      setImportModel((prev) => ({
        ...prev,
        translations: translations,
      }));
    },
    [importModel.translations],
  );

  const handleImportFile = React.useCallback(async () => {
    console.log(importModel);
    await api
      .sendPostRequest(importModel, {
        requestUrl: `${process.env.REACT_APP_API_URL}vocabularyimport/importvocabulary`,
        method: 'POST',
      })
      .then(() => {
        setImportModel({
          topic: null,
          sourceLanguage: TranslationEnum.De,
          translations: [],
          file: [],
        });
      });
  }, [importModel, api]);

  const importDisabled = React.useMemo(() => {
    return (
      importModel.file == null ||
      importModel.topic == null ||
      importModel.sourceLanguage == null ||
      importModel.translations.length === 0
    );
  }, [importModel]);

  return (
    <Container>
      <Grid container spacing={2} gap={5}>
        <Grid size={12} mt="1.5rem" display="flex" flexDirection="row" alignItems="center">
          <Typography variant="h4">Vocabulary Import</Typography>
        </Grid>
        <Grid
          container
          size={12}
          mt="1.5rem"
          display="flex"
          flexDirection="row"
          alignItems="center"
        >
          <Grid size={6}>
            <Typography variant="h6">Select CSV file to import</Typography>
          </Grid>
          <Grid
            size={6}
            display="flex"
            flexDirection="row"
            justifyContent="flex-end"
            alignItems="center"
          >
            <FileSelect accept=".csv" onSelectedFilesChanged={onSelectedFilesChanged} />
          </Grid>
        </Grid>
        <Grid size={12}>
          <TextInput
            value={importModel.topic ?? ''}
            onChange={handleTopicChanged}
            label="Topic"
            variant="standard"
            placeholder="Enter a topic..."
          />
        </Grid>
        <Grid size={12} display="flex" flexDirection="row" alignItems="center">
          <Grid
            size={6}
            display="flex"
            flexDirection="row"
            justifyContent="flex-start"
            alignItems="center"
          >
            <Typography variant="h6">Select source language</Typography>
          </Grid>
          <Grid
            size={6}
            display="flex"
            flexDirection="row"
            justifyContent="flex-end"
            alignItems="flex-start"
            gap={2}
          >
            {languageCheckboxItems.map((item) => (
              <FormLabel key={item.value} sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                <Checkbox
                  sx={{ paddingBottom: 1.5 }}
                  checked={importModel.sourceLanguage === item.value}
                  value={item.value}
                  onChange={handleSourceLanguageChanged}
                />
                {item.label}
              </FormLabel>
            ))}
          </Grid>
        </Grid>
        <Grid size={12} display="flex" flexDirection="row" alignItems="center">
          <Grid
            size={6}
            display="flex"
            flexDirection="row"
            justifyContent="flex-start"
            alignItems="center"
          >
            <Typography variant="h6">Select target languages</Typography>
          </Grid>
          <Grid
            size={6}
            display="flex"
            flexDirection="row"
            justifyContent="flex-end"
            alignItems="flex-start"
            gap={2}
          >
            {languageCheckboxItems.map((item) => (
              <FormLabel key={item.value} sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                <Checkbox
                  sx={{ paddingBottom: 1.5 }}
                  checked={importModel.translations.includes(item.value)}
                  disabled={importModel.sourceLanguage === item.value}
                  value={item.value}
                  onChange={handleTranslationChanged}
                />
                {item.label}
              </FormLabel>
            ))}
          </Grid>
        </Grid>
        <Grid size={12} mt="1.5rem" display="flex" flexDirection="row" justifyContent="flex-end">
          <ActionButton disabled={importDisabled} label="Import" onClick={handleImportFile} />
        </Grid>
      </Grid>
    </Container>
  );
};

export default VocabularyImportDialogContent;
