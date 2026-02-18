import { TranslationEnum } from '../../../lib/enums/TranslationEnum';

export interface IVocabularyExportModel {
  id: number;
  vocabularyGroupGuid: string;
  vocabularyCategoryId: number;
  vocabularyCategoryName: string;
  word: string;
  article?: string;
  partOfSpeech: string;
  sentence?: string;
  ipa?: string;
  language: TranslationEnum;
  isValidated: boolean;
  lastUpdatedAtBy?: string;
}
