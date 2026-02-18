import { TranslationEnum } from '../../../lib/enums/TranslationEnum';

export interface IVocabularyFileUpload {
  topic: string | null;
  sourceLanguage: TranslationEnum | null;
  translations: TranslationEnum[];
  file: number[];
  fileName: string;
}
