using Data.Accessor.Interfaces;
using Data.Database;
using Data.Database.Entities.Files;
using Logic.Import.Interfaces;
using Logic.Shared;
using Logic.Shared.Interfaces;
using Microsoft.AspNetCore.Http;
using Shared.Enums;
using Shared.Models.Import;

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
        public async Task ImportVocabularyFileAsync(VocabularyFileUpload fileUploadModel)
        {
            try
            {
                if (fileUploadModel == null)
                {
                    throw new ArgumentNullException(nameof(fileUploadModel));
                }

                await _scheduledTasks.ScheduleTask(
                    ScheduledTaskType.VocabularyImportService, 
                    $"Vocabulary import scheduled with status pending.");
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


        private ImportFileEntity CreateImportFileEntity(VocabularyFileUpload fileUploadModel)
        {

            var importFileEntity = new ImportFileEntity
            {
                FileName = fileUploadModel.FileName,
                FileBytes = fileUploadModel.File,
                Topic = fileUploadModel.Topic,
                SourceLanguage = fileUploadModel.SourceLanguage,
                Translations = fileUploadModel.Translations,
                IsImportedSuccessful = false
            };

            return importFileEntity;

        }

        private List<VocabularyImportWordModel> ParseVocabularyFileAsync(VocabularyFileUpload fileUploadModel)
        {
            var vocabularyEntries = new List<VocabularyImportWordModel>();

            if (!fileUploadModel.File.Any())
            {
                return vocabularyEntries;
            }

            var lines = ReadFileLines(fileUploadModel.File);

            if (!lines.Any())
            {
                return vocabularyEntries;
            }

            var vocabularyModels = ParseLines(lines, GetMappedColums(lines.First()), fileUploadModel);
           
            return vocabularyModels;
        }

        private List<string> ReadFileLines(List<byte> fileBytes)
        {
            var lines = new List<string>();
            using (var stream = new MemoryStream(fileBytes.ToArray()))
            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (!string.IsNullOrEmpty(line))
                    {
                        lines.Add(line);
                    }
                }
            }
            return lines;
        }

        private Dictionary<string, int> GetMappedColums(string headerRow)
        {
            var columnDefinition = GetColumnDefinition();

            var headerColumns = headerRow.Split(',');

            foreach (var column in headerColumns)
            {
                if (columnDefinition.TryGetValue(column, out var entry))
                {
                    columnDefinition[column] = Array.IndexOf(headerColumns, column);
                }
            }

            return columnDefinition;
        }

        private Dictionary<string, int> GetColumnDefinition()
        {
            return new Dictionary<string, int>
            {
                { "Word", -1 },
                { "Language", -1  },
                { "PartOfSpeech", -1 },
                { "Sentence", -1 }
            };
        }

        private List<VocabularyImportWordModel> ParseLines(List<string> lines, Dictionary<string, int> columnMapping, VocabularyFileUpload uploadModel)
        {
            var vocabularyEntries = new List<VocabularyImportWordModel>();

            foreach (var line in lines.Skip(1))
            {
                var columns = line.Split(',');

                var language = (TranslationEnum)Enum.Parse(typeof(TranslationEnum), columns[columnMapping["Language"]] ?? uploadModel.SourceLanguage.ToString());

                var wordModel = new VocabularyImportWordModel
                {
                    Word = columns[columnMapping["Word"]],
                    Language = language,
                    PartOfSpeach = columns[columnMapping["PartOfSpeach"]],
                    ExampleSentence = columns[columnMapping["Sentence"]],
                    Ipa = string.Empty,
                    Translations = new List<VocabularyImportWordModel>()
                };

                vocabularyEntries.Add(wordModel);
            }
            return vocabularyEntries;
        }

        private KaikkiExtractionTypeEnum GetExtratctionType(TranslationEnum translationType)
        {
            switch (translationType)
            {
                case TranslationEnum.En:
                    return KaikkiExtractionTypeEnum.EnglishExtractions;
                case TranslationEnum.De:
                    return KaikkiExtractionTypeEnum.GermanExtractions;
                case TranslationEnum.Da:
                    return KaikkiExtractionTypeEnum.DanishExtractions;
                default:
                    throw new ArgumentOutOfRangeException(nameof(translationType), $"Unsupported translation type: {translationType}");
            }
        }
    }
}
