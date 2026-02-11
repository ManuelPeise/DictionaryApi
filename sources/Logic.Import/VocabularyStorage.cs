using Data.Accessor.Interfaces;
using Data.Database.Entities.Vocabulary;
using Shared.Enums;
using Shared.Models.Words;

namespace Logic.Import
{
    internal class VocabularyStorage
    {
        private readonly IUnitOfWork _unitOfWork;

        internal VocabularyStorage(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        internal async Task StoreVocabularies(List<Vocabulary> vocabularies, string topic, TranslationEnum sourceLanguage, string user = "System")
        {
            var languageIdMap = await GetLanguageIdMap();
            var partOfSpeechIdMap = await GetPartOfSpeechIdMap();

            var vocabularyTopicMap = await GetVocabularyTopicIdMap(topic, sourceLanguage);

            var existingTranslatedWords = vocabularyTopicMap[topic].Vocabularies.Select(v => v.Word).ToList();

            var entitiesToAdd = new List<VocabularyEntity>();

            foreach (var vocabulary in vocabularies)
            {
                var groupGuid = Guid.NewGuid();

                foreach (var translation in vocabulary.Translations)
                {
                    if (!existingTranslatedWords.Contains(translation.Word))
                    {
                        entitiesToAdd.Add(new VocabularyEntity
                        {
                            GroupGuid = groupGuid,
                            Article = translation.Article,
                            Word = translation.Word,
                            Ipa = translation.Ipa,
                            ExampleSentence = translation.Sentence,
                            IsReviewRequired = true,
                            PartOfSpeechId = partOfSpeechIdMap[translation.PartOfSpeech],
                            LanguageId = languageIdMap[translation.Language],
                            TopicId = vocabularyTopicMap[topic].Id

                        });
                    }
                }
            }

            if (entitiesToAdd.Any())
            {
                await _unitOfWork.VocabularyRepository.AddRangeAsync(entitiesToAdd);
                await _unitOfWork.SaveChangesAsync(user);
            }
        }

        private async Task<Dictionary<TranslationEnum, int>> GetLanguageIdMap()
        {
            var entities = await _unitOfWork.LanguageRepository.GetAllAsync();

            return entities.ToDictionary(e => e.TranslationType, e => e.Id);
        }

        private async Task<Dictionary<string, int>> GetPartOfSpeechIdMap()
        {
            var entities = await _unitOfWork.PartOfSpeechRepository.GetAllAsync();
            return entities.ToDictionary(e => e.Name, e => e.Id);
        }

        private async Task<Dictionary<string, VocabularyTopicEntity>> GetVocabularyTopicIdMap(string topic, TranslationEnum sourceLanguage)
        {
            var entities = await _unitOfWork.VocabularyTopicRepository.GetAllAsync(false, e => e.Vocabularies);

            if (!entities.Any(e => e.Name == topic && e.SourceLanguage == sourceLanguage))
            {
                await _unitOfWork.VocabularyTopicRepository.AddAsync(new VocabularyTopicEntity
                {
                    GroupGuid = Guid.NewGuid(),
                    Name = topic,
                    SourceLanguage = sourceLanguage,
                });

                await _unitOfWork.SaveChangesAsync("System");

                entities = await _unitOfWork.VocabularyTopicRepository.GetAllAsync();
            }

            return entities.ToDictionary(e => e.Name, e => e);
        }
    }
}
