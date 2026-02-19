using Data.Accessor.Interfaces;
using Data.Database.Entities.Vocabulary;
using Shared.Enums;
using Shared.Models.Vocabulary;

namespace Logic.Import
{
    internal class VocabularyStorage
    {
        private readonly IUnitOfWork _unitOfWork;

        internal VocabularyStorage(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        internal async Task StoreVocabularies(List<Vocabulary> vocabularies, string category, TranslationEnum sourceLanguage, string user = "System")
        {
            var languageIdMap = await GetLanguageIdMap();
            var partOfSpeechIdMap = await GetPartOfSpeechIdMap();

            var vocabularyCategoryMap = await GetVocabularyCategoryMap(category, sourceLanguage);

            var existingTranslatedWordEntities = await _unitOfWork.VocabularyUnitOfWork.VocabularyRepository.GetAllAsync(false, e => e.VocabulatyToCategoryEntities);

            var existingTranslatedWords = existingTranslatedWordEntities.Select(x => x.Word).Distinct().ToList();

            var databaseIsChanged = false;

            foreach (var vocabulary in vocabularies)
            {
                var groupGuid = Guid.NewGuid();

                foreach (var translation in vocabulary.Translations)
                {
                    if (existingTranslatedWords.Contains(translation.Word))
                    {
                        var entity = existingTranslatedWordEntities.FirstOrDefault(x => x.Word == translation.Word);

                        if (entity == null)
                        {
                            continue;
                        }

                        var categoryEntity = vocabularyCategoryMap[category];

                        var isAlreadyLinked = entity.VocabulatyToCategoryEntities?
                            .Any(c => c.CategoryId == categoryEntity.Id) ?? false;

                        if (isAlreadyLinked)
                        {
                            continue;
                        }

                        await _unitOfWork.VocabularyUnitOfWork.VocabularyToCategoryRepository.AddAsync(new VocabularyToCategoryEntity
                        {
                            IdExternal = Guid.NewGuid(),
                            VocabularyId = entity.Id,
                            CategoryId = categoryEntity.Id,
                            IsDirty = false
                        });

                        databaseIsChanged = true;
                    }
                    else
                    {
                        await _unitOfWork.VocabularyUnitOfWork.VocabularyToCategoryRepository.AddAsync(new VocabularyToCategoryEntity
                        {
                            Vocabulary = new VocabularyEntity
                            {
                                IdExternal = Guid.NewGuid(),
                                GroupGuid = groupGuid,
                                Word = translation.Word,
                                Article = translation.Article,
                                ExampleSentence = translation.Sentence,
                                Ipa = translation.Ipa,
                                IsReviewRequired = true,
                                LanguageId = languageIdMap[translation.Language],
                                PartOfSpeechId = partOfSpeechIdMap[translation.PartOfSpeech],
                                IsDirty = false
                            },
                            Category = vocabularyCategoryMap[category],
                        });

                        databaseIsChanged = true;
                    }
                }
            }

            if (databaseIsChanged)
            {
                await _unitOfWork.SaveChangesAsync(user);
            }
        }

        internal async Task<List<VocabularyExportModel>> UpdateVocabularies(List<VocabularyExportModel> vocabularies, string user)
        {
           if(!vocabularies.Any())
            {
                return new List<VocabularyExportModel>();
            }

            var languageIdMap = await GetLanguageIdMap();
            var partOfSpeechIdMap = await GetPartOfSpeechIdMap();
            var existingEntities = await _unitOfWork.VocabularyUnitOfWork.VocabularyRepository.GetAllAsync();
            
            foreach (var vocabulary in vocabularies)
            {
                var entity = existingEntities.FirstOrDefault(e => e.Id == vocabulary.Id);
                
                if (entity == null)
                {
                    continue;
                }

                entity.Word = vocabulary.Word;
                entity.Article = vocabulary.Article;
                entity.ExampleSentence = vocabulary.Sentence;
                entity.Ipa = vocabulary.Ipa;
                entity.IsReviewRequired = !vocabulary.IsValidated;
                entity.LanguageId = languageIdMap[vocabulary.Language];
                entity.PartOfSpeechId = partOfSpeechIdMap[vocabulary.PartOfSpeech];
                entity.IsDirty = false;
            }

            await _unitOfWork.SaveChangesAsync(user);

            return existingEntities
                .Where(e => vocabularies.Any(v => v.Id == e.Id)).Select(e => new VocabularyExportModel
            {
                Id = e.Id,
                VocabularyGroupGuid = e.GroupGuid,
                Word = e.Word,
                Article = e.Article,
                PartOfSpeech = partOfSpeechIdMap.FirstOrDefault(x => x.Value == e.PartOfSpeechId).Key,
                Sentence = e.ExampleSentence,
                Ipa = e.Ipa,
                Language = languageIdMap.FirstOrDefault(x => x.Value == e.LanguageId).Key,
                IsValidated = !e.IsReviewRequired,
                LastUpdatedAtBy = user,
            }).ToList();
        }
        
        private async Task<Dictionary<TranslationEnum, int>> GetLanguageIdMap()
        {
            var entities = await _unitOfWork.VocabularyUnitOfWork.LanguageRepository.GetAllAsync();

            return entities.ToDictionary(e => e.TranslationType, e => e.Id);
        }

        private async Task<Dictionary<string, int>> GetPartOfSpeechIdMap()
        {
            var entities = await _unitOfWork.VocabularyUnitOfWork.PartOfSpeechRepository.GetAllAsync();
            return entities.ToDictionary(e => e.Name, e => e.Id);
        }

        private async Task<Dictionary<string, VocabularyCategoryEntity>> GetVocabularyCategoryMap(string topic, TranslationEnum sourceLanguage)
        {
            var entities = await _unitOfWork.VocabularyUnitOfWork.VocabularyCategoryRepository.GetAllAsync(true, e => e.VocabulariesToCategoryEntities);

            if (!entities.Any(e => e.Name == topic && e.SourceLanguage == sourceLanguage))
            {
                await _unitOfWork.VocabularyUnitOfWork.VocabularyCategoryRepository.AddAsync(new VocabularyCategoryEntity
                {
                    GroupGuid = Guid.NewGuid(),
                    Name = topic,
                    SourceLanguage = sourceLanguage,
                });

                await _unitOfWork.SaveChangesAsync("System");

                entities = await _unitOfWork.VocabularyUnitOfWork.VocabularyCategoryRepository.GetAllAsync();
            }

            return entities.ToDictionary(e => e.Name, e => e);
        }
    }
}
