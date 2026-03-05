using Logic.Shared.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Service.Api.Controllers.Vovabulary
{
    public class TranslationController: ApiControllerBase
    {
        private readonly ILibreTranslateClient _libreTranslateClient;

        public TranslationController(ILibreTranslateClient libreTranslateClient)
        {
            _libreTranslateClient = libreTranslateClient;
        }

        [HttpGet(Name = "TranslateWord")]
        public async Task<string?> TranslateWord(string word, string sourceLanguage, string targetLanguage)
        {
            return await _libreTranslateClient.TranslateWord(word, sourceLanguage, targetLanguage);
        }
    }
}
