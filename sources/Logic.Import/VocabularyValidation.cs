using Data.Accessor.Interfaces;
using Data.Database;
using Data.Database.Entities.Vocabulary;
using Logic.Import.Interfaces;
using Logic.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.Enums;
using Shared.Models.Vocabulary;
using System.Globalization;

namespace Logic.Import
{
    public class VocabularyValidation : LogicBase, IVocabularyValidation
    {
        private readonly Logger<VocabularyValidation> _logger;

        public VocabularyValidation(DatabaseContext dbContext, IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
            : base(dbContext, httpContextAccessor, unitOfWork)
        {
            _logger = new Logger<VocabularyValidation>(dbContext);
        }

        public async Task<VocabularyValidationPageModel> GetVocabularyValidationPageModel()
        {
            var model = new VocabularyValidationPageModel
            {
                Categories = new List<VocabularyCategoryExportModel>(),
                CategoryDropDownItems = new List<VocabularyCategoryDropdownModel>()
            };

            try
            {
                var categoryEntities = await UnitOfWork.VocabularyUnitOfWork.VocabularyCategoryRepository.GetAllAsync(false, x => x.VocabulariesToCategoryEntities);

                if (!categoryEntities.Any())
                {
                    return model;
                }

                foreach (var ce in categoryEntities.Where(e => e.VocabulariesToCategoryEntities.Any()))
                {
                    foreach (var vc in ce.VocabulariesToCategoryEntities)
                    {
                        await UnitOfWork.VocabularyUnitOfWork.VocabularyRepository.FirstOrDefaultByIdAsync(vc.VocabularyId);
                    }

                    model.Categories.Add(new VocabularyCategoryExportModel
                    {
                        Id = ce.Id,
                        Name = ce.Name,
                        VocabularyGroups = await GetVocabulariesGroupsToReview(ce)
                    });
                }

                model.CategoryDropDownItems = categoryEntities.Select(e => new VocabularyCategoryDropdownModel
                {
                    Id = e.Id,
                    Label = e.Name
                }).ToList();

                return model;
            }
            catch (Exception exception)
            {
                await _logger.LogMessageAsync(
                    "Could not load vocabulary validation page model",
                    LogMessageTypeEnum.Error, exception.Message, exception.StackTrace);

                return model;
            }
        }

        public async Task<VocabularyUpdateResponse?> UpdateValidatedVocabularies(List<VocabularyExportModel> vocabularies)
        {
            try
            {
                var currentUser = GetCurrentUser();

                var vocabularyStore = new VocabularyStorage(UnitOfWork);

                var updatedVocabularies = await vocabularyStore.UpdateVocabularies(vocabularies, currentUser.EmailAddress);

                return new VocabularyUpdateResponse
                {
                    GroupGuid = updatedVocabularies.FirstOrDefault()?.VocabularyGroupGuid.ToString() ?? string.Empty,
                    Vocabularies = updatedVocabularies
                };
            }
            catch (Exception exception)
            {
                await _logger.LogMessageAsync(
                    "Could not update validated vocabularies",
                    LogMessageTypeEnum.Error, exception.Message, exception.StackTrace);

                return null;
            }
        }

        private async Task<List<VocabularyGroupExportModel>> GetVocabulariesGroupsToReview(VocabularyCategoryEntity vocabularyCategory)
        {
            foreach (var vc in vocabularyCategory.VocabulariesToCategoryEntities)
            {
                await UnitOfWork.VocabularyUnitOfWork.LanguageRepository.FirstOrDefaultByIdAsync(vc.Vocabulary.LanguageId);
                await UnitOfWork.VocabularyUnitOfWork.PartOfSpeechRepository.FirstOrDefaultByIdAsync(vc.Vocabulary.PartOfSpeechId);
            }

            return (from ve in vocabularyCategory.VocabulariesToCategoryEntities.Select(e => e.Vocabulary)
                    group ve by ve.GroupGuid into vocabularyEntityGroup
                    select new VocabularyGroupExportModel
                    {
                        VocabularyGroupGuid = vocabularyEntityGroup.Key,
                        Vocabularies = (from v in vocabularyEntityGroup
                                        select new VocabularyExportModel
                                        {
                                            Id = v.Id,
                                            VocabularyGroupGuid = v.GroupGuid,
                                            Word = v.Word,
                                            PartOfSpeech = v.PartOfSpeech.Name,
                                            Sentence = v.ExampleSentence,
                                            Article = v.Article,
                                            Ipa = v.Ipa,
                                            Language = v.Language.TranslationType,
                                            IsValidated = !v.IsReviewRequired,
                                            LastUpdatedAtBy= $"{v.UpdatedBy} / {v.UpdatedAt.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture)}",
                                        }).ToList()
                    }).ToList();
        }
    }
}
