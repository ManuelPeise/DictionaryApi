using Data.Accessor.Interfaces;
using Data.Database;
using Data.Database.Entities.Files;
using Logic.Import.Interfaces;
using Logic.Shared;
using Logic.Shared.Interfaces;
using Microsoft.AspNetCore.Http;
using Shared.Enums;
using Shared.Models.Import;
using Shared.Models.Scheduler;

namespace Logic.Import
{
    public class FileImporter : LogicBase, IFileImporter
    {
        private readonly Logger<FileImporter> _logger;

        private readonly IScheduledTasks _scheduledTasks;

        public FileImporter(
            DatabaseContext dbContext,
            IHttpContextAccessor httpContextAccessor,
            IUnitOfWork unitOfWork,
            IScheduledTasks scheduledTasks)
            : base(dbContext, httpContextAccessor, unitOfWork)
        {
            _logger = new Logger<FileImporter>(dbContext);
            _scheduledTasks = scheduledTasks;
        }

        /// <summary>
        /// Schedules an asynchronous task to import a vocabulary file using the provided upload model.
        /// </summary>
        /// <remarks>If an error occurs during scheduling, the exception is logged before being rethrown.
        /// The import operation is scheduled and not performed immediately; callers should monitor task status
        /// separately if needed.</remarks>
        /// <param name="fileUploadModel">The model containing information about the vocabulary file to be imported. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation of scheduling the vocabulary import.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="fileUploadModel"/> is null.</exception>
        public async Task ImportVocabularyFileAsync(VocabularyFileUpload fileUploadModel, bool isPending, bool isCompleted)
        {
            try
            {
                if (fileUploadModel == null)
                {
                    throw new ArgumentNullException(nameof(fileUploadModel));
                }

                var currentUser = GetCurrentUser();

                var importFileEntity = CreateImportFileEntity(fileUploadModel, isPending, isCompleted);

                await UnitOfWork.ImportFileRepository.AddAsync(importFileEntity);

                if (isPending)
                {
                    await _scheduledTasks.ScheduleTask(new SceduledTaskRequest
                    {
                        Type = ScheduledTaskType.VocabularyImportService,
                        Message = $"Vocabulary import scheduled with status pending.",
                        FireTime = null,
                        Interval = ScheduleInterval.None,
                    });
                }

                await UnitOfWork.SaveChangesAsync(currentUser.EmailAddress);

            }
            catch (Exception exception)
            {
                await _logger.LogMessageAsync(
                    "Error importing vocabulary file",
                    LogMessageTypeEnum.Error,
                    exception.Message,
                    exception.StackTrace);
                throw;
            }
        }

        private ImportFileEntity CreateImportFileEntity(VocabularyFileUpload fileUploadModel, bool isPending, bool isCompleted)
        {

            var importFileEntity = new ImportFileEntity
            {
                FileName = fileUploadModel.FileName,
                FileBytes = fileUploadModel.File,
                Key = fileUploadModel.Topic,
                SourceLanguage = fileUploadModel.SourceLanguage,
                Translations = fileUploadModel.Translations,
                Status = isPending ? FileImportStatus.Pending : isCompleted ? FileImportStatus.Completed : FileImportStatus.Failed,
                IsImportedSuccessful = isCompleted
            };

            return importFileEntity;

        }

    }
}
