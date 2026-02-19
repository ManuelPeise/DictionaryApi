using Data.Accessor;
using Data.Accessor.Interfaces;
using Data.Database;
using Data.Database.Entities;
using Data.Database.Entities.User;
using Data.Database.Entities.Vocabulary;
using Logic.Administration.Interfaces;
using Logic.Shared;
using Microsoft.AspNetCore.Http;
using Shared.Enums;
using Shared.Models.User.Sync;
using Shared.Models.Vocabulary.Sync;

namespace Logic.Administration
{
    public class PushSyncronization : LogicBase, IPushSyncronization
    {
        private readonly Logger<PushSyncronization> _logger;
        public PushSyncronization(DatabaseContext dbContext, IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork) :
            base(dbContext, httpContextAccessor, unitOfWork)
        {
            _logger = new Logger<PushSyncronization>(dbContext);
        }

        public async Task<UserDataSyncModel?> SyncUserData(UserDataSyncModel model)
        {
            var result = model;

            try
            {
                var currentUser = GetCurrentUser();
                var userDataEntity = await UnitOfWork.UserRepository.FirstOrDefaultByIdExternalAsync(model.IdExternal);

                if (userDataEntity == null)
                {
                    throw new Exception($"Could not find user with IdExternal: {model.IdExternal}");
                }

                await UnitOfWork.UserCredentialsRepository.FirstOrDefaultByIdAsync(userDataEntity.UserCredentialsId);
                await UnitOfWork.UserSettingsRepository.FirstOrDefaultByIdAsync(userDataEntity.UserSettingsId);

                if (userDataEntity.UpdatedAt < model.UpdatedAt)
                {
                    await UpdateUserEntity(userDataEntity, model, currentUser.EmailAddress);

                    await UnitOfWork.SaveChangesAsync(model.UpdatedBy);
                }

                return GetExportSyncModel(userDataEntity);
            }
            catch (Exception exception)
            {
                await _logger.LogMessageAsync(
                    $"Error during user data synchronization.",
                    LogMessageTypeEnum.Error,
                    exception.Message,
                    exception.StackTrace);

                return null;
            }
        }

        public async Task<List<VocabularySessionSyncModel>?> SyncVocabularySessions(List<VocabularySessionSyncModel> models)
        {
            var result = new List<VocabularySessionSyncModel>();

            try
            {
                var dbChanged = false;
                var currentUser = GetCurrentUser();
                var remoteVocabularySessionEntities = await UnitOfWork.VocabularyUnitOfWork.VocabularySessionRepository.GetAllByAsync(x => x.UserId == currentUser.Id);

                if (!remoteVocabularySessionEntities.Any() && !models.Any())
                {
                    throw new Exception("No vocabulary sessions found for synchronization.");
                }

                var remoteVocabularySessionMap = remoteVocabularySessionEntities.ToDictionary(x => x.IdExternal, x => x);
                var deviceVocabularySessionMap = models.ToDictionary(x => x.IdExternal, x => x);

                var hasAddedEntries = deviceVocabularySessionMap.Count() > remoteVocabularySessionMap.Count();

                if (hasAddedEntries)
                {
                    var newEntities = deviceVocabularySessionMap
                        .Where(x => !remoteVocabularySessionMap.ContainsKey(x.Key))
                        .Select(x => new VocabularySessionEntity
                        {
                            IdExternal = x.Value.IdExternal,
                            UserId = x.Value.UserId,
                            CategoryId = x.Value.CategoryId,
                            SessionType = x.Value.SessionType,
                            IsDirty = false,
                            CreatedAt = x.Value.CreatedAt,
                            CreatedBy = x.Value.CreatedBy,
                            UpdatedAt = x.Value.UpdatedAt,
                            UpdatedBy = x.Value.UpdatedBy,
                        }).ToList();

                    await AddNewEntities(UnitOfWork.VocabularyUnitOfWork.VocabularySessionRepository, newEntities);

                    dbChanged = true;
                }

                var hasDeletedEntries = deviceVocabularySessionMap.Count() < remoteVocabularySessionMap.Count();

                if (hasDeletedEntries)
                {
                    var entitiesToDelete = remoteVocabularySessionMap
                        .Where(x => !deviceVocabularySessionMap.ContainsKey(x.Key))
                        .Select(x => x.Value).ToList();

                    await DeleteEntities(UnitOfWork.VocabularyUnitOfWork.VocabularySessionRepository, entitiesToDelete);

                    dbChanged = true;
                }

                var outDatedVocabularySessionsMap = models
                   .Where(e => remoteVocabularySessionMap.TryGetValue(e.IdExternal, out var remoteEntity) && e.UpdatedAt > remoteEntity.UpdatedAt)
                   .ToDictionary(e => e.IdExternal);

                if (outDatedVocabularySessionsMap.Any())
                {
                    await UpdateVocabularySessionEntities(outDatedVocabularySessionsMap.ToList(), remoteVocabularySessionMap.Values.ToList());

                    dbChanged = true;
                }

                if (dbChanged)
                {
                    await UnitOfWork.SaveChangesAsync(currentUser.EmailAddress);
                }

                return remoteVocabularySessionEntities.Select(x => new VocabularySessionSyncModel
                {
                    IdExternal = x.IdExternal,
                    UserId = x.UserId,
                    CategoryId = x.CategoryId,
                    SessionType = x.SessionType,
                    IsDirty = false,
                    CreatedAt = x.CreatedAt,
                    CreatedBy = x.CreatedBy,
                    UpdatedAt = x.UpdatedAt,
                    UpdatedBy = x.UpdatedBy,
                }).ToList();

            }
            catch (Exception exception)
            {
                await _logger.LogMessageAsync(
                    $"Error during vocabulary session synchronization.",
                    LogMessageTypeEnum.Error,
                    exception.Message,
                    exception.StackTrace);

                return null;
            }
        }

        public async Task<List<VocabularyProgressSyncModel>?> SyncVocabularyProgress(List<VocabularyProgressSyncModel> models)
        {
            try
            {
                var dbChanged = false;
                var currentUser = GetCurrentUser();
                var remoteVocabularyProgressEntities = await UnitOfWork.VocabularyUnitOfWork.VocabularyProgressRepository.GetAllByAsync(e => models.Select(m => m.IdExternal).Contains(e.IdExternal));

                if (remoteVocabularyProgressEntities == null || !remoteVocabularyProgressEntities.Any())
                {
                    throw new Exception($"Could not find any vocabulary progress entities with IdExternal: {string.Join(",", models.Select(m => m.IdExternal))}");
                }

                var hasAddedEntries = models.Count() > remoteVocabularyProgressEntities.Count();

                if (hasAddedEntries)
                {
                    var entitiesToAdd = models.Where(m => !remoteVocabularyProgressEntities.Any(e => e.IdExternal == m.IdExternal))
                        .Select(m => new VocabularyProgressEntity
                        {
                            IdExternal = m.IdExternal,
                            UserId = m.UserId,
                            TimesSeen = m.TimesSeen,
                            Success = m.Success,
                            Failed = m.Failed,
                            VocabularyId = m.VocabularyId,
                            IsDirty = false,
                            CreatedAt = m.CreatedAt,
                            CreatedBy = m.CreatedBy,
                            UpdatedAt = m.UpdatedAt,
                            UpdatedBy = m.UpdatedBy,
                        }).ToList();

                    await AddNewEntities(UnitOfWork.VocabularyUnitOfWork.VocabularyProgressRepository, entitiesToAdd);

                    dbChanged = true;
                }

                var hasDeletedEntries = models.Count() < remoteVocabularyProgressEntities.Count();

                if (hasDeletedEntries)
                {
                    var entities = remoteVocabularyProgressEntities.Where(e => !models.Any(m => m.IdExternal == e.IdExternal)).ToList();

                    await DeleteEntities(UnitOfWork.VocabularyUnitOfWork.VocabularyProgressRepository, entities);

                    dbChanged = true;
                }

                var updatedEntries = remoteVocabularyProgressEntities.Where(e => models.Any(m => m.IdExternal == e.IdExternal && m.UpdatedAt > e.UpdatedAt)).ToList();

                if (updatedEntries != null && updatedEntries.Any())
                {
                    updatedEntries.ForEach(e =>
                    {
                        var model = models.First(m => m.IdExternal == e.IdExternal);
                        e.UserId = model.UserId;
                        e.TimesSeen = model.TimesSeen;
                        e.Success = model.Success;
                        e.Failed = model.Failed;
                        e.IsDirty = false;
                        e.VocabularyId = model.VocabularyId;
                        e.UpdatedAt = model.UpdatedAt;
                        e.UpdatedBy = model.UpdatedBy;
                    });

                    await UpdateEntities(UnitOfWork.VocabularyUnitOfWork.VocabularyProgressRepository, updatedEntries);
                    dbChanged = true;
                }


                if (dbChanged)
                {
                    await UnitOfWork.SaveChangesAsync(currentUser.EmailAddress);
                }

                return remoteVocabularyProgressEntities.Select(x => new VocabularyProgressSyncModel
                {
                    IdExternal = x.IdExternal,
                    UserId = x.UserId,
                    TimesSeen = x.TimesSeen,
                    Success = x.Success,
                    Failed = x.Failed,
                    VocabularyId = x.VocabularyId,
                    IsDirty = false,
                    CreatedAt = x.CreatedAt,
                    CreatedBy = x.CreatedBy,
                    UpdatedAt = x.UpdatedAt,
                    UpdatedBy = x.UpdatedBy,
                }).ToList();
            }
            catch (Exception exception)
            {
                await _logger.LogMessageAsync(
                    $"Error during vocabulary progress synchronization.",
                    LogMessageTypeEnum.Error,
                    exception.Message,
                    exception.StackTrace);

                return null;
            }
        }

        public async Task<List<VocabularySessionResultSyncModel>?> SyncVocabularySessionResults(List<VocabularySessionResultSyncModel> models)
        {
            try
            {
                var dbChanged = false;
                var sessionResultEntities = await UnitOfWork.VocabularyUnitOfWork.VocabularySessionResultRepository.GetAllByAsync(x => models.Select(m => m.IdExternal).Contains(x.IdExternal));

                var hasAddedEntries = models.Count() > sessionResultEntities.Count();

                if (hasAddedEntries)
                {
                    var entitiesToAdd = models.Where(m => !sessionResultEntities.Any(e => e.IdExternal == m.IdExternal))
                        .Select(m => new VocabularySessionResultEntity
                        {
                            IdExternal = m.IdExternal,
                            UserId = m.UserId,
                            Start = m.Start,
                            End = m.End,
                            Success = m.Success,
                            Failed = m.Failed,
                            SessionId = m.SessionId,
                            IsDirty = false,
                            CreatedAt = m.CreatedAt,
                            CreatedBy = m.CreatedBy,
                            UpdatedAt = m.UpdatedAt,
                            UpdatedBy = m.UpdatedBy,
                        }).ToList();

                    await AddNewEntities(UnitOfWork.VocabularyUnitOfWork.VocabularySessionResultRepository, entitiesToAdd);

                    dbChanged = true;
                }

                var hasDeletedEntities = models.Count() < sessionResultEntities.Count();

                if (hasDeletedEntities)
                {
                    var deletedEntities = sessionResultEntities.Where(e => !models.Any(m => m.IdExternal == e.IdExternal)).ToList() ?? new List<VocabularySessionResultEntity>();
                    await DeleteEntities(UnitOfWork.VocabularyUnitOfWork.VocabularySessionResultRepository, deletedEntities);
                    dbChanged = true;
                }

                var updatedEntities = sessionResultEntities.Where(e => models.Any(m => m.IdExternal == e.IdExternal && m.UpdatedAt > e.UpdatedAt)).ToList();

                if (updatedEntities != null && updatedEntities.Any())
                {
                    updatedEntities.ForEach(e =>
                    {
                        var model = models.First(m => m.IdExternal == e.IdExternal);
                        e.UserId = model.UserId;
                        e.Start = model.Start;
                        e.End = model.End;
                        e.Success = model.Success;
                        e.Failed = model.Failed;
                        e.SessionId = model.SessionId;
                        e.IsDirty = false;
                        e.UpdatedAt = model.UpdatedAt;
                        e.UpdatedBy = model.UpdatedBy;
                    });

                    await UpdateEntities(UnitOfWork.VocabularyUnitOfWork.VocabularySessionResultRepository, updatedEntities);
                    dbChanged = true;
                }

                if (dbChanged)
                {
                    await UnitOfWork.SaveChangesAsync(GetCurrentUser().EmailAddress);
                }

                return sessionResultEntities.Select(x => new VocabularySessionResultSyncModel
                {
                    IdExternal = x.IdExternal,
                    UserId = x.UserId,
                    Start = x.Start,
                    End = x.End,
                    Success = x.Success,
                    Failed = x.Failed,
                    SessionId = x.SessionId,
                    IsDirty = false,
                    CreatedAt = x.CreatedAt,
                    CreatedBy = x.CreatedBy,
                    UpdatedAt = x.UpdatedAt,
                    UpdatedBy = x.UpdatedBy,
                }).ToList();
            }
            catch (Exception exception)
            {
                await _logger.LogMessageAsync(
                    $"Error during vocabulary session result synchronization.",
                    LogMessageTypeEnum.Error,
                    exception.Message,
                    exception.StackTrace);

                return null;
            }
        }


        private async Task<bool> UpdateVocabularySessionEntities(List<KeyValuePair<Guid, VocabularySessionSyncModel>> models, List<VocabularySessionEntity> vocabularySessionEntities)
        {
            var dbChanged = false;

            foreach (var entity in vocabularySessionEntities)
            {
                var model = models.FirstOrDefault(x => x.Key == entity.IdExternal);
                if (model.Key == Guid.Empty)
                {
                    continue;
                }

                entity.UserId = model.Value.UserId;
                entity.CategoryId = model.Value.CategoryId;
                entity.SessionType = model.Value.SessionType;
                entity.UpdatedAt = model.Value.UpdatedAt;
                entity.UpdatedBy = model.Value.UpdatedBy;

                dbChanged = true;
            }

            return dbChanged;
        }

        private async Task UpdateUserEntity(UserEntity userDataEntity, UserDataSyncModel model, string username)
        {
            if (userDataEntity.UserCredentials == null || userDataEntity.UserSettings == null)
            {
                throw new Exception($"UserCredentials or UserSettings for user with IdExternal: {model.IdExternal} not found.");
            }

            userDataEntity.FirstName = model.FirstName;
            userDataEntity.LastName = model.LastName;
            userDataEntity.EmailAddress = model.EmailAddress;
            userDataEntity.ProfileImage = model.ProfileImage;
            userDataEntity.DateOfBirth = model.DateOfBirth;
            userDataEntity.UserRole = model.UserRole;
            userDataEntity.CreatedAt = model.CreatedAt;
            userDataEntity.UpdatedAt = model.UpdatedAt;
            userDataEntity.UpdatedAt = model.UpdatedAt;
            userDataEntity.UpdatedBy = model.UpdatedBy;

            userDataEntity.UserCredentials.PasswordHash = model.UserCredentials.PasswordHash;
            userDataEntity.UserCredentials.ExpireDate = model.UserCredentials.ExpireDate;
            userDataEntity.UserCredentials.CreatedAt = model.UserCredentials.CreatedAt;
            userDataEntity.UserCredentials.CreatedBy = model.UserCredentials.CreatedBy;
            userDataEntity.UserCredentials.UpdatedAt = model.UserCredentials.UpdatedAt;
            userDataEntity.UserCredentials.UpdatedBy = model.UserCredentials.UpdatedBy;

            userDataEntity.UserSettings.IsAutoDataSyncEnabled = model.UserSettings.IsAutoDataSyncEnabled;
            userDataEntity.UserSettings.UseLocalDataStore = model.UserSettings.UseLocalDataStore;
            userDataEntity.UserSettings.CreatedAt = model.UserSettings.CreatedAt;
            userDataEntity.UserSettings.CreatedBy = model.UserSettings.CreatedBy;
            userDataEntity.UserSettings.UpdatedAt = model.UserSettings.UpdatedAt;
            userDataEntity.UserSettings.UpdatedBy = model.UserSettings.UpdatedBy;

            await UnitOfWork.SaveChangesAsync(username);
        }

        private UserDataSyncModel GetExportSyncModel(UserEntity userEntity)
        {
            if (userEntity.UserCredentials == null || userEntity.UserSettings == null)
            {
                throw new Exception($"UserCredentials or UserSettings for user with IdExternal: {userEntity.IdExternal} not found.");
            }

            return new UserDataSyncModel
            {
                IdExternal = userEntity.IdExternal,
                FirstName = userEntity.FirstName,
                LastName = userEntity.LastName,
                EmailAddress = userEntity.EmailAddress,
                ProfileImage = userEntity.ProfileImage,
                DateOfBirth = userEntity.DateOfBirth,
                UserRole = userEntity.UserRole,
                UserCredentials = new UserCredentialsSyncModel
                {
                    IdExternal = userEntity.UserCredentials.IdExternal,
                    PasswordHash = userEntity.UserCredentials.PasswordHash,
                    ExpireDate = userEntity.UserCredentials.ExpireDate,
                    IsDirty = false,
                    CreatedAt = userEntity.UserCredentials.CreatedAt,
                    CreatedBy = userEntity.UserCredentials.CreatedBy,
                    UpdatedAt = userEntity.UserCredentials.UpdatedAt,
                    UpdatedBy = userEntity.UserCredentials.UpdatedBy,
                },
                UserSettings = new UserSettingsSyncModel
                {
                    IdExternal = userEntity.UserSettings.IdExternal,
                    IsAutoDataSyncEnabled = userEntity.UserSettings.IsAutoDataSyncEnabled,
                    UseLocalDataStore = userEntity.UserSettings.UseLocalDataStore,
                    IsDirty = false,
                    CreatedAt = userEntity.UserSettings.CreatedAt,
                    CreatedBy = userEntity.UserSettings.CreatedBy,
                    UpdatedAt = userEntity.UserSettings.UpdatedAt,
                    UpdatedBy = userEntity.UserSettings.UpdatedBy,
                },
                IsDirty = false,
                CreatedAt = userEntity.CreatedAt,
                CreatedBy = userEntity.CreatedBy,
                UpdatedAt = userEntity.UpdatedAt,
                UpdatedBy = userEntity.UpdatedBy
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
