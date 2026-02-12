import { TranslationEnum } from '../../../lib/enums/translationEnum';

export interface IVocabularyValidationPageModel {
  categories: IVocabularyCategoryExportModel[];
}

export interface IVocabularyCategoryExportModel {
  id: number;
  name: string;
  vocabularyGroups: IVocabularyGroupExportModel[];
}

export interface IVocabularyGroupExportModel {
  vocabularyGroupGuid: string;
  vocabularies: IVocabularyExportModel[];
}

export interface IVocabularyExportModel {
  id: number;
  vocabularyGroupGuid: string;
  word: string;
  article?: string;
  partOfSpeech: string;
  sentence?: string;
  ipa?: string;
  language: TranslationEnum;
  isValidated: boolean;
  lastUpdateBy?: string;
  lastUpdateAt?: string;
}
