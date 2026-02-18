import React from 'react';
import { IVocabularyExportModel } from '../models/IVocabularyExportModel';
import { Checkbox, Grid, IconButton, Paper, Typography } from '@mui/material';
import { ChevronLeft, ChevronRight } from '@mui/icons-material';
import TextInput from '../../../components/input/TextInput';
import Dropdown from '../../../components/input/Dropdown';
import { IDropdownItem } from '../../../lib/interfaces/IDropdownItem';
import { PartOfSpeechEnum } from '../../../lib/enums/PartOfSpeechEnum';
import ActionButton from '../../../components/input/ActionButton';
import VocabularyContentPlaceholder from './VocabularyContentPlaceholder';
import LoadingSpinner from '../../../components/shared/LoadingSpinner';
import { IGroupByResult } from '../../../lib/utils';
import { isEqual } from 'lodash';
import { TranslationEnum } from '../../../lib/enums/translationEnum';

interface IProps {
  isLoading: boolean;
  group: IGroupByResult<IVocabularyExportModel> | null;
  handleSaveVocabulary: (vocabularies: IVocabularyExportModel[]) => Promise<void>;
}
const VocabularyDetails: React.FC<IProps> = (props) => {
  const { isLoading, group, handleSaveVocabulary } = props;
  const originalVocabularies = React.useRef(group?.items || []);
  const [selectedIndex, setSelectedIndex] = React.useState(0);
  const [vocabularies, setVocabularies] = React.useState<IVocabularyExportModel[]>(
    group?.items || [],
  );

  const isModified = React.useMemo(() => {
    console.log('triggered isModified', vocabularies, originalVocabularies.current);
    return !isEqual(vocabularies, originalVocabularies.current);
  }, [vocabularies]);

  const saveDisabled = React.useMemo(() => {
    const canSave = vocabularies.every((v) => {
      return (
        v.word.trim() === '' ||
        v.sentence?.trim() === '' ||
        v.language == null ||
        v.partOfSpeech == null
      );
    });
    return !isModified && !canSave;
  }, [isModified, vocabularies]);

  const languageDropdownItems = React.useMemo<IDropdownItem[]>(() => {
    return [
      {
        id: TranslationEnum.De,
        label: 'German',
      },
      {
        id: TranslationEnum.En,
        label: 'English',
      },
      {
        id: TranslationEnum.Da,
        label: 'Danish',
      },
    ];
  }, []);

  const partOfSpeechDropdownItems = React.useMemo<IDropdownItem[]>(() => {
    return [
      { id: PartOfSpeechEnum.Adjective, label: 'Adjective' },
      { id: PartOfSpeechEnum.Adverb, label: 'Adverb' },
      { id: PartOfSpeechEnum.Article, label: 'Article' },
      { id: PartOfSpeechEnum.Noun, label: 'Noun' },
      { id: PartOfSpeechEnum.Verb, label: 'Verb' },
      { id: PartOfSpeechEnum.Preposition, label: 'Preposition' },
      { id: PartOfSpeechEnum.Pronoun, label: 'Pronoun' },
      { id: PartOfSpeechEnum.ProperNoun, label: 'Proper Noun' },
    ];
  }, []);

  const getPartOdSpeechId = React.useCallback((partOfSpeech: string): number => {
    switch (partOfSpeech?.toLocaleLowerCase()) {
      case 'adjective':
        return PartOfSpeechEnum.Adjective;
      case 'adverb':
        return PartOfSpeechEnum.Adverb;
      case 'article':
        return PartOfSpeechEnum.Article;
      case 'noun':
        return PartOfSpeechEnum.Noun;
      case 'verb':
        return PartOfSpeechEnum.Verb;
      case 'preposition':
        return PartOfSpeechEnum.Preposition;
      case 'pronoun':
        return PartOfSpeechEnum.Pronoun;
      case 'proper noun':
        return PartOfSpeechEnum.ProperNoun;
      default:
        return 0;
    }
  }, []);

  const handleToggleVocabulary = React.useCallback(
    (direction: 'forward' | 'backward') => {
      if (direction === 'forward' && selectedIndex + 1 <= (group?.items ?? []).length) {
        setSelectedIndex(selectedIndex + 1);
      }

      if (direction === 'backward' && selectedIndex - 1 >= 0) {
        setSelectedIndex(selectedIndex - 1);
      }
    },
    [selectedIndex, group?.items],
  );

  const handleChange = React.useCallback(
    (field: keyof IVocabularyExportModel, value: string | boolean | number) => {
      const updatedVocabulary = {
        ...vocabularies?.[selectedIndex],
        [field]: value,
      } as IVocabularyExportModel;

      const updatedVocabularies = [...(vocabularies ?? [])];
      updatedVocabularies[selectedIndex] = updatedVocabulary;

      setVocabularies(updatedVocabularies);
    },
    [vocabularies, selectedIndex],
  );

  const handleReset = React.useCallback(() => {
    setVocabularies(group?.items || []);
  }, [group?.items]);

  const handleSave = React.useCallback(async () => {
    await handleSaveVocabulary(vocabularies);
  }, [handleSaveVocabulary, vocabularies]);

  React.useEffect(() => {
    originalVocabularies.current = group?.items || [];
    setVocabularies(group?.items || []);
  }, [group?.items]);

  if (vocabularies == null || vocabularies.length === 0) {
    return <VocabularyContentPlaceholder />;
  }

  return (
    <Grid size={9} boxSizing="border-box" height="100%">
      {isLoading && <LoadingSpinner message="Bitte warten, Vokabeln werden gespeichert..." />}
      <Paper elevation={4} sx={{ padding: 2, height: '100%' }}>
        <Grid
          size={12}
          padding={4}
          display="flex"
          flexDirection="column"
          justifyContent="center"
          height="100%"
        >
          <Grid
            size={12}
            container
            spacing={4}
            display="flex"
            flexDirection="row"
            justifyContent="start"
            alignItems="center"
            height="100%"
          >
            <Grid container size={12} display="flex" justifyContent="center" alignItems="center">
              <Grid
                width="100%"
                container
                spacing={4}
                display="flex"
                flexDirection="row"
                justifyContent="center"
                alignItems="center"
              >
                <Grid size={6}>
                  <Dropdown
                    placeholder="Select a language"
                    disabled={vocabularies[selectedIndex]?.language != null}
                    items={languageDropdownItems}
                    value={vocabularies[selectedIndex]?.language ?? 0}
                    onChange={(value) =>
                      handleChange(
                        'language',
                        languageDropdownItems.find((i) => i.id === value)?.id || 0,
                      )
                    }
                  />
                </Grid>
                <Grid size={6}>
                  <Dropdown
                    placeholder="Select a part of speech"
                    items={partOfSpeechDropdownItems}
                    value={getPartOdSpeechId(vocabularies[selectedIndex]?.partOfSpeech || '')}
                    onChange={(value) =>
                      handleChange(
                        'partOfSpeech',
                        partOfSpeechDropdownItems.find((i) => i.id === value)?.label || '',
                      )
                    }
                  />
                </Grid>
              </Grid>
              <Grid
                width="100%"
                container
                spacing={4}
                display="flex"
                flexDirection="row"
                justifyContent="center"
                alignItems="center"
              >
                <Grid size={6}>
                  <TextInput
                    label="Article"
                    placeholder="Enter a artilce"
                    value={vocabularies[selectedIndex]?.article || ''}
                    variant="standard"
                    disabled={
                      getPartOdSpeechId(vocabularies[selectedIndex]?.partOfSpeech || '') !==
                        PartOfSpeechEnum.Noun ||
                      getPartOdSpeechId(vocabularies[selectedIndex]?.partOfSpeech || '') ===
                        PartOfSpeechEnum.Pronoun
                    }
                    onChange={(value) => handleChange('article', value)}
                  />
                </Grid>
                <Grid size={6}>
                  <TextInput
                    label="Word"
                    placeholder="Enter a word"
                    value={vocabularies[selectedIndex]?.word || ''}
                    variant="standard"
                    onChange={(value) => handleChange('word', value)}
                  />
                </Grid>
              </Grid>
              <Grid
                width="100%"
                container
                spacing={4}
                display="flex"
                flexDirection="row"
                justifyContent="center"
                alignItems="center"
              >
                <Grid size={3}>
                  <TextInput
                    label="Ipa"
                    placeholder="Enter a IPA"
                    value={vocabularies[selectedIndex]?.ipa || ''}
                    variant="standard"
                    onChange={(value) => handleChange('ipa', value)}
                  />
                </Grid>
                <Grid size={9}>
                  <TextInput
                    label="Example sentence"
                    value={vocabularies[selectedIndex]?.sentence || ''}
                    variant="standard"
                    onChange={(value) => handleChange('sentence', value)}
                  />
                </Grid>
                <Grid
                  width="100%"
                  container
                  spacing={4}
                  display="flex"
                  flexDirection="row"
                  justifyContent="center"
                  alignItems="center"
                >
                  <Grid size={6} display="flex" justifyContent="flex-start" alignItems="center">
                    <Checkbox
                      checked={vocabularies[selectedIndex]?.isValidated ?? false}
                      onChange={(e) => handleChange('isValidated', e.currentTarget.checked)}
                    />
                    <Typography variant="body1">Vokabel wurde validiert</Typography>
                  </Grid>
                  <Grid size={6} display="flex" justifyContent="flex-start" alignItems="center">
                    <Typography variant="body1">{`Letzte Aktualisierung durch  ${vocabularies[selectedIndex]?.lastUpdatedAtBy}`}</Typography>
                  </Grid>
                </Grid>
              </Grid>
            </Grid>
            <Grid
              size={12}
              display="flex"
              flexDirection="row"
              justifyContent="space-between"
              mt={2}
            >
              <IconButton
                disabled={selectedIndex === -1 || selectedIndex - 1 < 0}
                onClick={handleToggleVocabulary.bind(null, 'backward')}
              >
                <ChevronLeft />
              </IconButton>
              <Typography variant="body1">{`${selectedIndex + 1} of ${vocabularies.length}`}</Typography>
              <IconButton
                disabled={selectedIndex === -1 || selectedIndex + 1 === vocabularies.length}
                onClick={handleToggleVocabulary.bind(null, 'forward')}
              >
                <ChevronRight />
              </IconButton>
            </Grid>
            <Grid
              width="100%"
              container
              spacing={4}
              display="flex"
              flexDirection="row"
              justifyContent="flex-end"
              alignItems="center"
            >
              <ActionButton disabled={!isModified} label="Reset" onClick={handleReset} />
              <ActionButton disabled={saveDisabled} label="Save" onClick={handleSave} />
            </Grid>
          </Grid>
        </Grid>
      </Paper>
    </Grid>
  );
};

export default React.memo(VocabularyDetails);
