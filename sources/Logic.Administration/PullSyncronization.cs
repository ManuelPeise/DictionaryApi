using Data.Accessor.Interfaces;
using Data.Database;
using Logic.Administration.Interfaces;
using Logic.Shared;
using Microsoft.AspNetCore.Http;
using Shared.Enums;
using Shared.Models.Vocabulary.Sync;

namespace Logic.Administration
{
    public class PullSyncronization : LogicBase, IPullSyncronization
    {
        private readonly Logger<PullSyncronization> _logger;
        public PullSyncronization(DatabaseContext dbContext, IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork) :
            base(dbContext, httpContextAccessor, unitOfWork)
        {
            _logger = new Logger<PullSyncronization>(dbContext);
        }

        public async Task<List<VocabularyLanguageSyncModel>> PullVocabularyLanguageSyncModels()
        {
            var result = new List<VocabularyLanguageSyncModel>();

            try
            {
                var languageEntities = await UnitOfWork.VocabularyUnitOfWork.LanguageRepository.GetAllAsync();

                if (languageEntities == null || !languageEntities.Any())
                {
                    return result;
                }

                return languageEntities.Select(x => new VocabularyLanguageSyncModel
                {
                    IdExternal = x.IdExternal,
                    Name = x.Name,
                    ResourceKey = x.ResourceKey,
                    TranslationType = x.TranslationType,
                    IsDirty = false,
                    CreatedAt = x.CreatedAt,
                    CreatedBy = x.CreatedBy,
                    UpdatedAt = x.UpdatedAt,
                    UpdatedBy = x.UpdatedBy
                }).ToList();
            }
            catch (Exception exception)
            {
                await _logger.LogMessageAsync($"An error occurred while pulling vocabulary language sync models.",
                    LogMessageTypeEnum.Error, exception.Message, exception.StackTrace);

                return result;
            }
        }

        public async Task<List<VocabularyPartOfSpeechSyncModel>> PullPartOfSpeechSyncModels()
        {
            var result = new List<VocabularyPartOfSpeechSyncModel>(); ;
            try
            {
                var partOfSpeechEntities = await UnitOfWork.VocabularyUnitOfWork.PartOfSpeechRepository.GetAllAsync();

                if (!partOfSpeechEntities.Any())
                {
                    return result;
                }

                return partOfSpeechEntities.Select(x => new VocabularyPartOfSpeechSyncModel
                {
                    IdExternal = x.IdExternal,
                    Name = x.Name,
                    ResourceKey = x.ResourceKey,
                    IsDirty = false,
                    CreatedAt = x.CreatedAt,
                    CreatedBy = x.CreatedBy,
                    UpdatedAt = x.UpdatedAt,
                    UpdatedBy = x.UpdatedBy,
                }).ToList();
            }
            catch (Exception exception)
            {
                await _logger.LogMessageAsync($"An error occurred while pulling vocabulary part of speech sync models.",
                    LogMessageTypeEnum.Error, exception.Message, exception.StackTrace);

                return result;
            }

        }

        public async Task<List<VocabularyCategorySyncModel>> PullVocabelCategorySyncModels()
        {
            var result = new List<VocabularyCategorySyncModel>();

            try
            {
                var vocabularyCategoryEntities = await UnitOfWork.VocabularyUnitOfWork.VocabularyCategoryRepository.GetAllAsync();

                if (!vocabularyCategoryEntities.Any())
                {
                    return result;
                }

                return vocabularyCategoryEntities.Select(x => new VocabularyCategorySyncModel
                {
                    IdExternal = x.IdExternal,
                    GroupGuid = x.GroupGuid,
                    SourceLanguage = x.SourceLanguage,
                    Name = x.Name,
                    IsDirty = false,
                    CreatedAt = x.CreatedAt,
                    CreatedBy = x.CreatedBy,
                    UpdatedAt = x.UpdatedAt,
                    UpdatedBy = x.UpdatedBy,
                }).ToList();
            }
            catch (Exception exception)
            {
                await _logger.LogMessageAsync($"An error occurred while pulling vocabulary category sync models.",
                    LogMessageTypeEnum.Error, exception.Message, exception.StackTrace);
                return result;
            }
        }

        public async Task<List<VocabularyToCategorySyncModel>> PullVocabularyToCategorySyncModels(List<Guid> categoryExternalGuids)
        {
            var result = new List<VocabularyToCategorySyncModel>();
            try
            {
                var vocabularyToCategoryEntities = await UnitOfWork.VocabularyUnitOfWork.VocabularyToCategoryRepository.GetAllByAsync(e => categoryExternalGuids.Contains(e.IdExternal));

                if (!vocabularyToCategoryEntities.Any())
                {
                    throw new Exception($"No vocabulary to category relations found for the provided category external guids.");
                }

                foreach (var entity in vocabularyToCategoryEntities)
                {
                    await UnitOfWork.VocabularyUnitOfWork.VocabularyRepository.FirstOrDefaultByIdAsync(entity.VocabularyId);
                }

                return vocabularyToCategoryEntities
                    .Where(e => e.Vocabulary != null && !e.Vocabulary.IsReviewRequired)
                    .Select(x => new VocabularyToCategorySyncModel
                    {
                        IdExternal = x.IdExternal,
                        CategoryId = x.CategoryId,
                        VocabularyId = x.VocabularyId,
                        Vocabulary = new VocabularySyncModel
                        {
                            IdExternal = x.Vocabulary.IdExternal,
                            GroupGuid = x.Vocabulary.GroupGuid,
                            Article = x.Vocabulary.Article,
                            Word = x.Vocabulary.Word,
                            Ipa = x.Vocabulary.Ipa,
                            ExampleSentence = x.Vocabulary.ExampleSentence,
                            PartOfSpeechId = x.Vocabulary.PartOfSpeechId,
                            LanguageId = x.Vocabulary.LanguageId,
                            IsReviewRequired = x.Vocabulary.IsReviewRequired,
                            IsDirty = false,
                            CreatedAt = x.Vocabulary.CreatedAt,
                            CreatedBy = x.Vocabulary.CreatedBy,
                            UpdatedAt = x.Vocabulary.UpdatedAt,
                            UpdatedBy = x.Vocabulary.UpdatedBy
                        },
                        IsDirty = false,
                        CreatedAt = x.CreatedAt,
                        CreatedBy = x.CreatedBy,
                        UpdatedAt = x.UpdatedAt,
                        UpdatedBy = x.UpdatedBy,
                    }).ToList();
            }
            catch (Exception exception)
            {
                await _logger.LogMessageAsync($"An error occurred while pulling vocabulary to category sync models.",
                    LogMessageTypeEnum.Error, exception.Message, exception.StackTrace);
                return result;
            }
        }
    }
}
