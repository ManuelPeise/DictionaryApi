import { IDropdownItem } from '../../../lib/interfaces/IDropdownItem';
import { IVocabularyExportModel } from './IVocabularyExportModel';

export interface IVocabularyValidationModel {
  categoryDropdownItems: IDropdownItem[];
  vocabularies: IVocabularyExportModel[];
}
