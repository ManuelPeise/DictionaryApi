using Data.Accessor;
using Data.Accessor.Interfaces;
using Data.Database;
using Data.Database.Entities;
using Data.Database.Entities.Vocabulary;
using Logic.Administration.Interfaces;
using Logic.Shared;
using Microsoft.AspNetCore.Http;
using Shared.Enums;
using Shared.Models.User;
using Shared.Models.Vocabulary.Sync;

namespace Logic.Administration
{
    public class VocabularySyncronization : LogicBase, IVocabularySyncronization
    {
        private readonly Logger<VocabularySyncronization> _logger;
        public VocabularySyncronization(DatabaseContext dbContext, IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork) :
            base(dbContext, httpContextAccessor, unitOfWork)
        {
            _logger = new Logger<VocabularySyncronization>(dbContext);
        }

        public async Task<VocabularyDataSyncModel> SyncData(VocabularySyncRequestModel requestModel)
        {
            var vocabularySyncModel = GetEmptySyncModel();

            try
            {
                var currentUser = GetCurrentUser();

                await SyncLanguages(vocabularySyncModel);
                await SyncPartOfSpeech(vocabularySyncModel);
                await SyncVocabularyCategories(vocabularySyncModel);
                await SyncVocabulary(vocabularySyncModel, requestModel.CategoryGuids);
                await SyncVocabularyToCategory(vocabularySyncModel, requestModel.CategoryGuids);
                await SyncVocabularyProgress(vocabularySyncModel, requestModel.VocabularyProgressSyncModels, currentUser);
            }
            catch (Exception exception)
            {
                await _logger.LogMessageAsync(
                    $"Error during vocabulary synchronization.",
                    LogMessageTypeEnum.Error,
                    exception.Message,
                    exception.StackTrace);
            }

            return vocabularySyncModel;
        }

        private async Task SyncVocabularyProgress(
            VocabularyDataSyncModel vocabularySyncModel,
            List<VocabularyProgressSyncModel> deviceVocabularyProgressSyncModels,
            UserModel currentUser)
        {
            var remoteVocabularyProgressEntities = await UnitOfWork.VocabularyUnitOfWork.VocabularyProgressRepository.GetAllByAsync(x => x.UserId == currentUser.Id);

            if (!remoteVocabularyProgressEntities.Any() && !deviceVocabularyProgressSyncModels.Any())
            {
                return;
            }

            var remoteVocabularyProgressMap = remoteVocabularyProgressEntities.ToDictionary(x => x.IdExternal, x => x);
            var deviceVocabularyProgressMap = deviceVocabularyProgressSyncModels.ToDictionary(x => x.IdExternal, x => x);

            var hasAddedEntries = deviceVocabularyProgressMap.Count() > remoteVocabularyProgressMap.Count();

            if (hasAddedEntries)
            {
                var newEntities = deviceVocabularyProgressMap
                    .Where(x => !remoteVocabularyProgressMap.ContainsKey(x.Key))
                    .Select(x => new VocabularyProgressEntity
                    {
                        IdExternal = x.Value.IdExternal,
                        UserId = x.Value.UserId,
                        TimesSeen = x.Value.TimesSeen,
                        Success = x.Value.Success,
                        Failed = x.Value.Failed,
                        VocabularyId = x.Value.VocabularyId,
                        CreatedAt = x.Value.CreatedAt,
                        CreatedBy = x.Value.CreatedBy,
                        UpdatedAt = x.Value.UpdatedAt,
                        UpdatedBy = x.Value.UpdatedBy,
                    }).ToList();

                await AddNewEntities(UnitOfWork.VocabularyUnitOfWork.VocabularyProgressRepository, newEntities);

            }

            var hasDeletedEntries = deviceVocabularyProgressMap.Count() < remoteVocabularyProgressMap.Count();

            if (hasDeletedEntries)
            {
                var entitiesToDelete = remoteVocabularyProgressMap
                    .Where(x => !deviceVocabularyProgressMap.ContainsKey(x.Key))
                    .Select(x => x.Value).ToList();

                await DeleteEntities(UnitOfWork.VocabularyUnitOfWork.VocabularyProgressRepository, entitiesToDelete);
            }

            var outDatedVocabularyMap = deviceVocabularyProgressSyncModels
               .Where(e => remoteVocabularyProgressMap.TryGetValue(e.IdExternal, out var remoteEntity) && e.UpdatedAt > remoteEntity.UpdatedAt)
               .ToDictionary(e => e.IdExternal);

            if (outDatedVocabularyMap.Any())
            {
                await UpdateEntities(
                    UnitOfWork.VocabularyUnitOfWork.VocabularyProgressRepository,
                    outDatedVocabularyMap.Select(x => new VocabularyProgressEntity
                    {
                        Id = remoteVocabularyProgressMap[x.Key].Id,
                        IdExternal = x.Value.IdExternal,
                        UserId = x.Value.UserId,
                        TimesSeen = x.Value.TimesSeen,
                        Success = x.Value.Success,
                        Failed = x.Value.Failed,
                        VocabularyId = x.Value.VocabularyId,
                        CreatedAt = x.Value.CreatedAt,
                        CreatedBy = x.Value.CreatedBy,
                        UpdatedAt = x.Value.UpdatedAt,
                        UpdatedBy = x.Value.UpdatedBy,
                    }).ToList());
            }

            await UnitOfWork.SaveChangesAsync(currentUser.EmailAddress);

            remoteVocabularyProgressEntities = await UnitOfWork.VocabularyUnitOfWork.VocabularyProgressRepository.GetAllByAsync(x => x.UserId == currentUser.Id);

            vocabularySyncModel.VocabularyProgressSyncModels = remoteVocabularyProgressEntities
                .Select(x => new VocabularyProgressSyncModel
                {
                    IdExternal = x.IdExternal,
                    UserId = x.UserId,
                    TimesSeen = x.TimesSeen,
                    Success = x.Success,
                    Failed = x.Failed,
                    VocabularyId = x.VocabularyId,
                    CreatedAt = x.CreatedAt,
                    CreatedBy = x.CreatedBy,
                    UpdatedAt = x.UpdatedAt,
                    UpdatedBy = x.UpdatedBy,
                }).ToList();
        }

        private async Task SyncVocabularyToCategory(VocabularyDataSyncModel vocabularySyncModel, List<Guid> categoryGuids)
        {
            var vocabularyToCategoryEntities = await UnitOfWork.VocabularyUnitOfWork.VocabularyToCategoryRepository.GetAllAsync(false, e => e.Category);

            if (!vocabularyToCategoryEntities.Any())
            {
                return;
            }

            vocabularySyncModel.VocabularyToCategorySyncModel = vocabularyToCategoryEntities
                .Where(x => categoryGuids.Contains(x.IdExternal))
                .Select(x => new VocabularyToCategorySyncModel
                {
                    IdExternal = x.IdExternal,
                    CategoryId = x.CategoryId,
                    VocabularyId = x.VocabularyId,
                    CreatedAt = x.CreatedAt,
                    CreatedBy = x.CreatedBy,
                    UpdatedAt = x.UpdatedAt,
                    UpdatedBy = x.UpdatedBy,
                }).ToList();
        }

        private async Task SyncVocabulary(VocabularyDataSyncModel vocabularySyncModel, List<Guid> categoryGuids)
        {
            var vocabularyToCategoryEntities = await UnitOfWork.VocabularyUnitOfWork.VocabularyToCategoryRepository.GetAllAsync(false);

            if (!vocabularyToCategoryEntities.Any())
            {
                return;
            }

            // load all related entities to avoid lazy loading issues during sync
            foreach (var vc in vocabularyToCategoryEntities)
            {
                await UnitOfWork.VocabularyUnitOfWork.VocabularyCategoryRepository.FirstOrDefaultAsync(x => x.Id == vc.CategoryId && categoryGuids.Contains(x.IdExternal));
                await UnitOfWork.VocabularyUnitOfWork.VocabularyRepository.FirstOrDefaultAsync(x => x.Id == vc.VocabularyId);
            }

            var vocabularyEntities = vocabularyToCategoryEntities.Where(x => categoryGuids.Contains(x.Category.IdExternal))
                .Select(x => x.Vocabulary)
                .ToHashSet();

            if (!vocabularyEntities.Any())
            {
                return;
            }

            vocabularySyncModel.VocabularySyncModels = vocabularyEntities
                .Select(x => new VocabularySyncModel
                {
                    IdExternal = x.IdExternal,
                    GroupGuid = x.GroupGuid,
                    Article = x.Article,
                    Word = x.Word,
                    Ipa = x.Ipa,
                    ExampleSentence = x.ExampleSentence,
                    IsReviewRequired = x.IsReviewRequired,
                    LanguageId = x.LanguageId,
                    PartOfSpeechId = x.PartOfSpeechId,
                    CreatedAt = x.CreatedAt,
                    CreatedBy = x.CreatedBy,
                    UpdatedAt = x.UpdatedAt,
                    UpdatedBy = x.UpdatedBy,
                }).ToList();
        }

        private async Task SyncVocabularyCategories(VocabularyDataSyncModel vocabularySyncModel)
        {
            var vocabularyCategoryEntities = await UnitOfWork.VocabularyUnitOfWork.VocabularyCategoryRepository.GetAllAsync();

            if (!vocabularyCategoryEntities.Any())
            {
                return;
            }

            vocabularySyncModel.VocabularyCategorySyncModels = vocabularyCategoryEntities.Select(x => new VocabularyCategorySyncModel
            {
                IdExternal = x.IdExternal,
                GroupGuid = x.GroupGuid,
                SourceLanguage = x.SourceLanguage,
                Name = x.Name,
                CreatedAt = x.CreatedAt,
                CreatedBy = x.CreatedBy,
                UpdatedAt = x.UpdatedAt,
                UpdatedBy = x.UpdatedBy,
            }).ToList();
        }

        private async Task SyncPartOfSpeech(VocabularyDataSyncModel vocabularySyncModel)
        {
            var partOfSpeechEntities = await UnitOfWork.VocabularyUnitOfWork.PartOfSpeechRepository.GetAllAsync();

            if (!partOfSpeechEntities.Any())
            {
                return;
            }

            vocabularySyncModel.PartOfSpeechSyncModels = partOfSpeechEntities.Select(x => new VocabularyPartOfSpeechSyncModel
            {
                IdExternal = x.IdExternal,
                Name = x.Name,
                ResourceKey = x.ResourceKey,
                CreatedAt = x.CreatedAt,
                CreatedBy = x.CreatedBy,
                UpdatedAt = x.UpdatedAt,
                UpdatedBy = x.UpdatedBy,
            }).ToList();
        }

        private async Task SyncLanguages(VocabularyDataSyncModel vocabularyRemoteSyncModel)
        {
            var languageEntities = await UnitOfWork.VocabularyUnitOfWork.LanguageRepository.GetAllAsync();

            if (!languageEntities.Any())
            {
                return;
            }

            vocabularyRemoteSyncModel.LanguageSyncModels = languageEntities.Select(x => new VocabularyLanguageSyncModel
            {
                IdExternal = x.IdExternal,
                Name = x.Name,
                ResourceKey = x.ResourceKey,
                CreatedAt = x.CreatedAt,
                CreatedBy = x.CreatedBy,
                UpdatedAt = x.UpdatedAt,
                UpdatedBy = x.UpdatedBy,
            }).ToList();
        }

        private VocabularyDataSyncModel GetEmptySyncModel()
        {
            return new VocabularyDataSyncModel
            {
                LanguageSyncModels = new List<VocabularyLanguageSyncModel>(),
                PartOfSpeechSyncModels = new List<VocabularyPartOfSpeechSyncModel>(),
                VocabularyCategorySyncModels = new List<VocabularyCategorySyncModel>(),
                VocabularySyncModels = new List<VocabularySyncModel>()
            };
        }

        private async Task AddNewEntities<T>(IRepositoryBase<T> repo, List<T> entities) where T : AEntityBase
        {
            if (!entities.Any())
            {
                return;
            }

            await repo.AddRangeAsync(entities);
        }

        private async Task DeleteEntities<T>(IRepositoryBase<T> repo, List<T> entitiesToDelete) where T : AEntityBase
        {
            if (!entitiesToDelete.Any())
            {
                return;
            }

            await repo.BulkDelete(entitiesToDelete);
        }

        private async Task UpdateEntities<T>(IRepositoryBase<T> repo, List<T> entitiesToUpdate) where T : AEntityBase
        {
            if (!entitiesToUpdate.Any())
            {
                return;
            }

            await repo.BulkUpdateAsync(entitiesToUpdate);
        }
    }
}
