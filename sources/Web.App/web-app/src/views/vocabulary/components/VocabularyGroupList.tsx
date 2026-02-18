import React from 'react';
import { Grid, List, ListItemButton, ListItemIcon, ListItemText, Paper } from '@mui/material';
import Dropdown from '../../../components/input/Dropdown';
import { TranslationEnum } from '../../../lib/enums/TranslationEnum';
import { colors } from '../../../lib/style/colors';
import StatefulIcon, { StatefulIcons } from '../../../components/icons/StatefulIcon';
import SwitchWithLabel from '../../../components/input/SwitchWithLabel';
import { IDropdownItem } from '../../../lib/interfaces/IDropdownItem';
import { IGroupByResult } from '../../../lib/utils';
import { IVocabularyExportModel } from '../models/IVocabularyExportModel';

interface IProps {
  categoryDropdownItems: IDropdownItem[];
  selectedCategoryId: number | null;
  vocabularyGroups: IGroupByResult<IVocabularyExportModel>[];
  selectedVocabularyGroupId: string | null;
  handleSelectedCategoryChanged: (id: number) => void;
  handleSelectedVocabularyGroupChanged: (guid: string) => void;
}

const VocabularyGroupList: React.FC<IProps> = (props) => {
  const {
    categoryDropdownItems,
    vocabularyGroups,
    selectedCategoryId,
    selectedVocabularyGroupId,
    handleSelectedCategoryChanged,
    handleSelectedVocabularyGroupChanged,
  } = props;

  const [showAll, setShowAll] = React.useState(true);

  return (
    <Grid size={3} boxSizing="border-box" height="100%">
      <Paper
        elevation={4}
        sx={{
          padding: 2,
          height: '100%',
          display: 'flex',
          flexDirection: 'column',
        }}
      >
        <Grid size={12} padding={4}>
          <Dropdown
            disabled={categoryDropdownItems.length === 0}
            placeholder="Wähle eine Kategorie"
            items={categoryDropdownItems}
            value={selectedCategoryId || 0}
            onChange={handleSelectedCategoryChanged}
          />
        </Grid>
        <Grid size={12} padding={4}>
          <SwitchWithLabel label="Alle Vokabeln anzeigen" value={showAll} onChange={setShowAll} />
        </Grid>
        <Grid size={12} mt={2} sx={{ flex: 1, minHeight: 0 }}>
          <List disablePadding sx={{ height: '100%', overflowY: 'auto', flex: 1 }}>
            {vocabularyGroups.map((item) => {
              if (!showAll && item.items.every((i) => i.isValidated)) {
                return null;
              }
              const label =
                item.items.find((i) => i.language === TranslationEnum.De)?.word || item.label;
              return (
                <ListItemButton
                  key={item.key}
                  selected={item.key === selectedVocabularyGroupId}
                  disabled={item.key === selectedVocabularyGroupId}
                  onClick={
                    handleSelectedVocabularyGroupChanged?.bind(null, item.key as string) ||
                    undefined
                  }
                  sx={{
                    '&.Mui-hover': {
                      backgroundColor: colors.selected,
                    },
                    '&.Mui-selected:hover': {
                      backgroundColor: colors.selected,
                    },
                    '&.Mui-selected': {
                      backgroundColor: colors.selected,
                    },
                  }}
                >
                  <ListItemIcon>
                    <StatefulIcon
                      color={item.items.some((i) => i.isValidated === true) ? 'success' : 'error'}
                      icon={
                        item.items.every((i) => i.isValidated === true)
                          ? StatefulIcons.verified
                          : StatefulIcons.notVerified
                      }
                      size={30}
                    />
                  </ListItemIcon>
                  <ListItemText
                    primary={label}
                    secondary={`${item.items
                      .map((i) => {
                        switch (i.language) {
                          case TranslationEnum.De:
                            return 'DE';
                          case TranslationEnum.En:
                            return 'EN';
                          case TranslationEnum.Da:
                            return 'DA';
                          default:
                            return '';
                        }
                      })
                      .sort()
                      .join(', ')}`}
                  />
                </ListItemButton>
              );
            })}
          </List>
        </Grid>
      </Paper>
    </Grid>
  );
};

export default React.memo(VocabularyGroupList);
