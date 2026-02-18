using Data.Accessor.Interfaces;
using Data.Database;
using Logic.Import.Interfaces;
using Logic.Shared;
using Microsoft.AspNetCore.Http;
using Shared.Enums;
using Shared.Models;
using Shared.Models.Vocabulary;
using Shared.Models.Vocabulary.Validation;
using System.Globalization;

namespace Logic.Import
{
    public class VocabularyValidationModule : LogicBase, IVocabularyValidationModule
    {
        private readonly Logger<VocabularyValidationModule> _logger;

        public VocabularyValidationModule(
            DatabaseContext dbContext,
            IHttpContextAccessor httpContextAccessor,
            IUnitOfWork unitOfWork) : base(dbContext, httpContextAccessor, unitOfWork)
        {
            _logger = new Logger<VocabularyValidationModule>(dbContext);
        }

        public async Task<VocabularyValidationModel> GetVocabularyValidationModel()
        {
            try
            {
                return new VocabularyValidationModel
                {
                    CategoryDropdownItems = await LoadCategorieDropdownItems(),
                    Vocabularies = await LoadVocabularies(),
                };

            }
            catch (Exception exception)
            {
                await _logger.LogMessageAsync("", LogMessageTypeEnum.Error, exception.Message, exception.StackTrace);

                return new VocabularyValidationModel
                {
                    CategoryDropdownItems = new List<DropdownModel>()
                };
            }
        }

        public async Task<List<VocabularyExportModel>> UpdateValidatedVocabularies(List<VocabularyExportModel> vocabularies)
        {
            try
            {
                var currentUser = GetCurrentUser();

                var vocabularyStore = new VocabularyStorage(UnitOfWork);

                var updatedVocabularies = await vocabularyStore.UpdateVocabularies(vocabularies, currentUser.EmailAddress);
            }
            catch (Exception exception)
            {
                await _logger.LogMessageAsync(
                    "Could not update validated vocabularies",
                    LogMessageTypeEnum.Error, exception.Message, exception.StackTrace);
            }

            return await LoadVocabularies();
        }

        private async Task<List<DropdownModel>> LoadCategorieDropdownItems()
        {
            var categorieEntities = await UnitOfWork.VocabularyUnitOfWork.VocabularyCategoryRepository.GetAllAsync();

            if (!categorieEntities.Any())
            {
                return new List<DropdownModel>();
            }

            return (from c in categorieEntities
                    select new DropdownModel
                    {
                        Id = c.Id,
                        Label = c.Name
                    }).ToList();
        }

        private async Task<List<VocabularyExportModel>> LoadVocabularies()
        {
            var vocabularies = new List<VocabularyExportModel>();

            var categoryEntities = await UnitOfWork.VocabularyUnitOfWork.VocabularyCategoryRepository.GetAllAsync(false, x => x.VocabulariesToCategoryEntities);

            if (!categoryEntities.Any())
            {
                return new List<VocabularyExportModel>();
            }

            foreach (var ce in categoryEntities.Where(e => e.VocabulariesToCategoryEntities.Any()))
            {
                foreach (var vc in ce.VocabulariesToCategoryEntities)
                {
                    await UnitOfWork.VocabularyUnitOfWork.VocabularyRepository.FirstOrDefaultByIdAsync(vc.VocabularyId);
                }

                var vocabularyEntities = ce.VocabulariesToCategoryEntities.Select(e => e.Vocabulary);

                foreach (var ve in vocabularyEntities)
                {
                    await UnitOfWork.VocabularyUnitOfWork.PartOfSpeechRepository.FirstOrDefaultByIdAsync(ve.PartOfSpeechId);
                    await UnitOfWork.VocabularyUnitOfWork.LanguageRepository.FirstOrDefaultByIdAsync(ve.LanguageId);

                    vocabularies.Add(new VocabularyExportModel
                    {
                        Id = ve.Id,
                        VocabularyGroupGuid = ve.GroupGuid,
                        VocabularyCategoryId = ce.Id,
                        VocabularyCategoryName = ce.Name,
                        Ipa = ve.Ipa,
                        Word = ve.Word,
                        Article = ve.Article,
                        Language = ve.Language.TranslationType,
                        PartOfSpeech = ve.PartOfSpeech.Name,
                        Sentence = ve.ExampleSentence,
                        IsValidated = !ve.IsReviewRequired,
                        LastUpdatedAtBy = $"{ve.UpdatedBy} / {ve.UpdatedAt.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture)}"
                    });
                }
            }

            return vocabularies;
        }
    }
}
