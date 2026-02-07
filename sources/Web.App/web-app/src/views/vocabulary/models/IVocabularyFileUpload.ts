import { TranslationEnum } from '../../../lib/enums/translationEnum';

export interface IVocabularyFileUpload {
  topic: string | null;
  sourceLanguage: TranslationEnum | null;
  translations: TranslationEnum[];
  file: number[];
}
